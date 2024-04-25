using UnityEngine;
using DuloGames.UI.Tweens;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace DuloGames.UI
{
	[DisallowMultipleComponent, ExecuteInEditMode, AddComponentMenu("UI/Window", 58), RequireComponent(typeof(CanvasGroup))]
	public class UIWindow : MonoBehaviour, IEventSystemHandler, ISelectHandler, IPointerDownHandler {
		
		public enum Transition
		{
			Instant,
			Fade
		}
		
		public enum VisualState
		{
			Shown,
			Hidden
		}
		
		public enum EscapeKeyAction
		{
			None,
			Hide,
			HideIfFocused,
			Toggle
		}
		
		[Serializable] public class TransitionBeginEvent : UnityEvent<UIWindow, VisualState, bool> {}
		[Serializable] public class TransitionCompleteEvent : UnityEvent<UIWindow, VisualState> {}
		
		protected static UIWindow m_FucusedWindow;
		public static UIWindow FocusedWindow { get { return m_FucusedWindow; } private set { m_FucusedWindow = value; } }
		
		[SerializeField] private UIWindowID m_WindowId = UIWindowID.None;
		[SerializeField] private int m_CustomWindowId = 0;
		[SerializeField] private VisualState m_StartingState = VisualState.Hidden;
		[SerializeField] private EscapeKeyAction m_EscapeKeyAction = EscapeKeyAction.Hide;
		[SerializeField] private bool m_UseBlackOverlay = false;

        [SerializeField] private bool m_FocusAllowReparent = true;

		[SerializeField] private Transition m_Transition = Transition.Instant;
		[SerializeField] private TweenEasing m_TransitionEasing = TweenEasing.InOutQuint;
		[SerializeField] private float m_TransitionDuration = 0.1f;
		
		protected bool m_IsFocused = false;
		private VisualState m_CurrentVisualState = VisualState.Hidden;
		private CanvasGroup m_CanvasGroup;
		
		/// <summary>
		/// Gets or sets the window identifier.
		/// </summary>
		/// <value>The window identifier.</value>
		public UIWindowID ID
		{
			get { return this.m_WindowId; }
			set { this.m_WindowId = value; }
		}
		
		/// <summary>
		/// Gets or sets the custom window identifier.
		/// </summary>
		/// <value>The custom window identifier.</value>
		public int CustomID
		{
			get { return this.m_CustomWindowId; }
			set { this.m_CustomWindowId = value; }
		}
		
        /// <summary>
        /// Gets or sets the escape key action.
        /// </summary>
		public EscapeKeyAction escapeKeyAction
		{
			get { return this.m_EscapeKeyAction; }
			set { this.m_EscapeKeyAction = value; }
		}
		
        /// <summary>
        /// Gets or sets a value indicating whether this window should use the black overlay.
        /// </summary>
        public bool useBlackOverlay
        {
            get { return this.m_UseBlackOverlay; }
            set
            {
                this.m_UseBlackOverlay = value;
                
                if (Application.isPlaying && this.m_UseBlackOverlay && this.isActiveAndEnabled)
                {
                    UIBlackOverlay overlay = UIBlackOverlay.GetOverlay(this.gameObject);

                    if (overlay != null)
                    {
                        if (value) this.onTransitionBegin.AddListener(overlay.OnTransitionBegin);
                        else this.onTransitionBegin.RemoveListener(overlay.OnTransitionBegin);
                    }
                }
            }
        }

        /// <summary>
        /// Allow re-parenting on focus.
        /// </summary>
        public bool focusAllowReparent
        {
            get { return this.m_FocusAllowReparent; }
            set { this.m_FocusAllowReparent = value; }
        }

		/// <summary>
		/// Gets or sets the transition.
		/// </summary>
		/// <value>The transition.</value>
		public Transition transition
		{
			get { return this.m_Transition; }
			set { this.m_Transition = value; }
		}
		
		/// <summary>
		/// Gets or sets the transition easing.
		/// </summary>
		/// <value>The transition easing.</value>
		public TweenEasing transitionEasing
		{
			get { return this.m_TransitionEasing; }
			set { this.m_TransitionEasing = value; }
		}
		
		/// <summary>
		/// Gets or sets the duration of the transition.
		/// </summary>
		/// <value>The duration of the transition.</value>
		public float transitionDuration
		{
			get { return this.m_TransitionDuration; }
			set { this.m_TransitionDuration = value; }
		}
		
		/// <summary>
		/// The transition begin (invoked when a transition begins).
		/// </summary>
		public TransitionBeginEvent onTransitionBegin = new TransitionBeginEvent();
		
		/// <summary>
		/// The transition complete event (invoked when a transition completes).
		/// </summary>
		public TransitionCompleteEvent onTransitionComplete = new TransitionCompleteEvent();
		
		/// <summary>
		/// Gets a value indicating whether this window is visible.
		/// </summary>
		/// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
		public bool IsVisible
		{
			get { return (this.m_CanvasGroup != null && this.m_CanvasGroup.alpha > 0f) ? true : false; }
		}
		
		/// <summary>
		/// Gets a value indicating whether this window is open.
		/// </summary>
		/// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
		public bool IsOpen
		{
			get { return (this.m_CurrentVisualState == VisualState.Shown); }
		}
		
		/// <summary>
		/// Gets a value indicating whether this instance is focused.
		/// </summary>
		/// <value><c>true</c> if this instance is focused; otherwise, <c>false</c>.</value>
		public bool IsFocused
		{
			get { return this.m_IsFocused; }
		}
		
		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		
		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UIWindow()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected virtual void Awake()
		{
			// Get the canvas group
			this.m_CanvasGroup = this.gameObject.GetComponent<CanvasGroup>();

            // Transition to the starting state
            if (Application.isPlaying)
                this.ApplyVisualState(this.m_StartingState);
        }
		
		protected virtual void Start()
		{
			// Assign new custom ID
			if (this.CustomID == 0)
				this.CustomID = UIWindow.NextUnusedCustomID;
            
            // Make sure we have a window manager in the scene if required
            if (this.m_EscapeKeyAction != EscapeKeyAction.None)
            {
                UIWindowManager manager = Component.FindObjectOfType<UIWindowManager>();

                // Add a manager if not present
                if (manager == null)
                {
                    GameObject newObj = new GameObject("Window Manager");
                    newObj.AddComponent<UIWindowManager>();
                    newObj.transform.SetAsFirstSibling();
                }
            }
        }
		
        protected virtual void OnEnable()
        {
            if (Application.isPlaying && this.m_UseBlackOverlay)
            {
                UIBlackOverlay overlay = UIBlackOverlay.GetOverlay(this.gameObject);

                if (overlay != null)
                    this.onTransitionBegin.AddListener(overlay.OnTransitionBegin);
            }
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying && this.m_UseBlackOverlay)
            {
                UIBlackOverlay overlay = UIBlackOverlay.GetOverlay(this.gameObject);

                if (overlay != null)
                    this.onTransitionBegin.RemoveListener(overlay.OnTransitionBegin);
            }
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
		{
			this.m_TransitionDuration = Mathf.Max(this.m_TransitionDuration, 0f);
		}
#endif
		
		/// <summary>
		/// Determines whether this window is active.
		/// </summary>
		/// <returns><c>true</c> if this instance is active; otherwise, <c>false</c>.</returns>
		protected virtual bool IsActive()
		{
			return (this.enabled && this.gameObject.activeInHierarchy);
		}
		
		/// <summary>
		/// Raises the select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSelect(BaseEventData eventData)
		{
			// Focus the window
			this.Focus();
		}
        
		/// <summary>
		/// Raises the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			// Focus the window
			this.Focus();
		}
		
		/// <summary>
		/// Focuses this window.
		/// </summary>
		public virtual void Focus()
		{
			if (this.m_IsFocused)
				return;
			
			// Flag as focused
			this.m_IsFocused = true;
			
			// Call the static on focused window
			UIWindow.OnBeforeFocusWindow(this);

            // Bring the window forward
            this.BringToFront();
		}

        /// <summary>
        /// Brings the window to the front.
        /// </summary>
        public void BringToFront()
        {
            UIUtility.BringToFront(this.gameObject, this.m_FocusAllowReparent);
        }

        /// <summary>
        /// Toggle the window Show/Hide.
        /// </summary>
        public virtual void Toggle()
		{
			if (this.m_CurrentVisualState == VisualState.Shown)
				this.Hide();
			else
				this.Show();
		}
		
		/// <summary>
		/// Show the window.
		/// </summary>
		public virtual void Show()
		{
			this.Show(false);
		}
		
		/// <summary>
		/// Show the window.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public virtual void Show(bool instant)
		{
			if (!this.IsActive())
				return;
            
			// Focus the window
			this.Focus();
			
			// Check if the window is already shown
			if (this.m_CurrentVisualState == VisualState.Shown)
				return;
            
            // Transition
            this.EvaluateAndTransitionToVisualState(VisualState.Shown, instant);
		}

		/// <summary>
		/// Hide the window.
		/// </summary>
		public virtual void Hide()
		{
			this.Hide(false);
		}
		
		/// <summary>
		/// Hide the window.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public virtual void Hide(bool instant)
		{
			if (!this.IsActive())
				return;
			
			// Check if the window is already hidden
			if (this.m_CurrentVisualState == VisualState.Hidden)
				return;
			
			// Transition
			this.EvaluateAndTransitionToVisualState(VisualState.Hidden, instant);
		}
		
		/// <summary>
		/// Evaluates and transitions to the specified visual state.
		/// </summary>
		/// <param name="state">The state to transition to.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void EvaluateAndTransitionToVisualState(VisualState state, bool instant)
		{
			float targetAlpha = (state == VisualState.Shown) ? 1f : 0f;
			
			// Call the transition begin event
			if (this.onTransitionBegin != null)
				this.onTransitionBegin.Invoke(this, state, (instant || this.m_Transition == Transition.Instant));
			
			// Do the transition
			if (this.m_Transition == Transition.Fade)
			{
				float duration = (instant) ? 0f : this.m_TransitionDuration;
                
                // Tween the alpha
                this.StartAlphaTween(targetAlpha, duration, true);
			}
			else
			{
				// Set the alpha directly
				this.SetCanvasAlpha(targetAlpha);
				
				// Call the transition complete event, since it's instant
				if (this.onTransitionComplete != null)
					this.onTransitionComplete.Invoke(this, state);
			}
			
			// Save the state
			this.m_CurrentVisualState = state;
			
			// If we are transitioning to show, enable the canvas group raycast blocking
			if (state == VisualState.Shown)
			{
				this.m_CanvasGroup.blocksRaycasts = true;
				//this.m_CanvasGroup.interactable = true;
			}
		}

        /// <summary>
        /// Instantly applies the visual state.
        /// </summary>
        /// <param name="state">The state to transition to.</param>
        public virtual void ApplyVisualState(VisualState state)
        {
            float targetAlpha = (state == VisualState.Shown) ? 1f : 0f;

            // Set the alpha directly
            this.SetCanvasAlpha(targetAlpha);

            // Save the state
            this.m_CurrentVisualState = state;

            // If we are transitioning to show, enable the canvas group raycast blocking
            if (state == VisualState.Shown)
            {
                this.m_CanvasGroup.blocksRaycasts = true;
                //this.m_CanvasGroup.interactable = true;
            }
            else
            {
                this.m_CanvasGroup.blocksRaycasts = false;
                //this.m_CanvasGroup.interactable = false;
            }
        }

        /// <summary>
        /// Starts alpha tween.
        /// </summary>
        /// <param name="targetAlpha">Target alpha.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
        public void StartAlphaTween(float targetAlpha, float duration, bool ignoreTimeScale)
		{
			if (this.m_CanvasGroup == null)
				return;
            
            var floatTween = new FloatTween { duration = duration, startFloat = this.m_CanvasGroup.alpha, targetFloat = targetAlpha };
			floatTween.AddOnChangedCallback(SetCanvasAlpha);
			floatTween.AddOnFinishCallback(OnTweenFinished);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			floatTween.easing = this.m_TransitionEasing;
			this.m_FloatTweenRunner.StartTween(floatTween);
		}
		
		/// <summary>
		/// Sets the canvas alpha.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		public void SetCanvasAlpha(float alpha)
		{
			if (this.m_CanvasGroup == null)
				return;
			
			// Set the alpha
			this.m_CanvasGroup.alpha = alpha;
			
			// If the alpha is zero, disable block raycasts
			// Enabling them back on is done in the transition method
			if (alpha == 0f)
			{
				this.m_CanvasGroup.blocksRaycasts = false;
				//this.m_CanvasGroup.interactable = false;
			}
		}
		
		/// <summary>
		/// Raises the list tween finished event.
		/// </summary>
		protected virtual void OnTweenFinished()
		{
			// Call the transition complete event
			if (this.onTransitionComplete != null)
				this.onTransitionComplete.Invoke(this, this.m_CurrentVisualState);
		}
		
		#region Static Methods
		/// <summary>
		/// Get all the windows in the scene (Including inactive).
		/// </summary>
		/// <returns>The windows.</returns>
		public static List<UIWindow> GetWindows()
		{
			List<UIWindow> windows = new List<UIWindow>();
			
			UIWindow[] ws = Resources.FindObjectsOfTypeAll<UIWindow>();
			
			foreach (UIWindow w in ws)
			{
				// Check if the window is active in the hierarchy
				if (w.gameObject.activeInHierarchy)
					windows.Add(w);
			}
			
			return windows;
		}
		
		public static int SortByCustomWindowID(UIWindow w1, UIWindow w2)
		{
			return w1.CustomID.CompareTo(w2.CustomID);
		}
		
		/// <summary>
		/// Gets the next unused custom ID for a window.
		/// </summary>
		/// <value>The next unused ID.</value>
		public static int NextUnusedCustomID
		{
			get
			{
				// Get the windows
				List<UIWindow> windows = UIWindow.GetWindows();
				
				if (GetWindows().Count > 0)
				{
					// Sort the windows by id
					windows.Sort(UIWindow.SortByCustomWindowID);
					
					// Return the last window id plus one
					return windows[windows.Count - 1].CustomID + 1;
				}
				
				// No windows, return 0
				return 0;
			}
		}
		
		/// <summary>
		/// Gets the window with the given ID.
		/// </summary>
		/// <returns>The window.</returns>
		/// <param name="id">Identifier.</param>
		public static UIWindow GetWindow(UIWindowID id)
		{
			// Get the windows and try finding the window with the given id
			foreach (UIWindow window in UIWindow.GetWindows())
				if (window.ID == id)
					return window;
			
			return null;
		}
		
		/// <summary>
		/// Gets the window with the given custom ID.
		/// </summary>
		/// <returns>The window.</returns>
		/// <param name="id">The custom identifier.</param>
		public static UIWindow GetWindowByCustomID(int customId)
		{
			// Get the windows and try finding the window with the given id
			foreach (UIWindow window in UIWindow.GetWindows())
				if (window.CustomID == customId)
					return window;
			
			return null;
		}
		
		/// <summary>
		/// Focuses the window with the given ID.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public static void FocusWindow(UIWindowID id)
		{
			// Focus the window
			if (UIWindow.GetWindow(id) != null)
				UIWindow.GetWindow(id).Focus();
		}
		
		/// <summary>
		/// Raises the before focus window event.
		/// </summary>
		/// <param name="window">The window.</param>
		protected static void OnBeforeFocusWindow(UIWindow window)
		{
			if (m_FucusedWindow != null)
				m_FucusedWindow.m_IsFocused = false;
			
			m_FucusedWindow = window;
		}
		#endregion
	}
}
