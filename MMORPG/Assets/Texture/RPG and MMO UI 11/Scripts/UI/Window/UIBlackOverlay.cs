using UnityEngine;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{
    /// <summary>
    /// The black overlay used behind windows such as the game menu.
    /// </summary>
	[ExecuteInEditMode, RequireComponent(typeof(CanvasGroup))]
    public class UIBlackOverlay : MonoBehaviour {
        
		private CanvasGroup m_CanvasGroup;
        private int m_WindowsCount = 0;

        private bool m_Transitioning = false;

        // Tween controls
        [System.NonSerialized]
        private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        // Called by Unity prior to deserialization, 
        // should not be called by users
        protected UIBlackOverlay()
        {
            if (this.m_FloatTweenRunner == null)
                this.m_FloatTweenRunner = new TweenRunner<FloatTween>();

            this.m_FloatTweenRunner.Init(this);
        }

        protected void Awake()
		{
			this.m_CanvasGroup = this.gameObject.GetComponent<CanvasGroup>();
            this.m_CanvasGroup.interactable = false;
            this.m_CanvasGroup.blocksRaycasts = false;
        }
		
		protected void Start()
		{
			// Non interactable
			this.m_CanvasGroup.interactable = false;

			// Hide the overlay
			this.Hide();
		}
		
		protected void OnEnable()
		{
			// Hide the overlay
			if (!Application.isPlaying)
				this.Hide();
        }
        
        /// <summary>
        /// Instantly show the overlay.
        /// </summary>
        public void Show()
		{
			// Show the overlay
			this.SetAlpha(1f);
			
			// Toggle block raycast on
			this.m_CanvasGroup.blocksRaycasts = true;
        }
		
        /// <summary>
        /// Instantly hide the overlay.
        /// </summary>
		public void Hide()
		{
			// Hide the overlay
			this.SetAlpha(0f);
			
			// Toggle block raycast off
			this.m_CanvasGroup.blocksRaycasts = false;
		}
		
        /// <summary>
        /// If the overlay is enabled and active in the hierarchy.
        /// </summary>
        /// <returns></returns>
		public bool IsActive()
		{
			return (this.enabled && this.gameObject.activeInHierarchy);
		}
		
        /// <summary>
        /// If the overlay is visible at all (alpha > 0f).
        /// </summary>
        /// <returns></returns>
		public bool IsVisible()
		{
			return (this.m_CanvasGroup.alpha > 0f);
		}
		
        /// <summary>
        /// This method is hooked to the window on transition begin event.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="state">The window visual state that we are transitioning to.</param>
        /// <param name="instant">If the transition is instant or not.</param>
		public void OnTransitionBegin(UIWindow window, UIWindow.VisualState state, bool instant)
		{
			if (!this.IsActive() || window == null)
				return;
			
			// Check if we are receiving hide event and we are not showing the overlay to begin with, return
			if (state == UIWindow.VisualState.Hidden && !this.IsVisible())
				return;
			
			// Prepare transition duration
			float duration = (instant) ? 0f : window.transitionDuration;
            TweenEasing easing = window.transitionEasing;

			// Showing a window
			if (state == UIWindow.VisualState.Shown)
			{
				// Increase the window count so we know when to hide the overlay
				this.m_WindowsCount += 1;
				
				// Check if the overlay is already visible
				if (this.IsVisible() && !this.m_Transitioning)
				{
					// Bring the window forward
					UIUtility.BringToFront(window.gameObject);
                    
                    // Break
                    return;
				}
				
				// Bring the overlay forward
				UIUtility.BringToFront(this.gameObject);
				
				// Bring the window forward
				UIUtility.BringToFront(window.gameObject);
                
                // Transition
                this.StartAlphaTween(1f, duration, easing);

                // Toggle block raycast on
                this.m_CanvasGroup.blocksRaycasts = true;
			}
			// Hiding a window
			else
			{
				// Decrease the window count
				this.m_WindowsCount -= 1;
                
                // Never go below 0
                if (this.m_WindowsCount < 0)
					this.m_WindowsCount = 0;
				
				// Check if we still have windows using the overlay
				if (this.m_WindowsCount > 0)
					return;
                
                // Transition
                this.StartAlphaTween(0f, duration, easing);

                // Toggle block raycast on
                this.m_CanvasGroup.blocksRaycasts = false;
			}
		}

        private void StartAlphaTween(float targetAlpha, float duration, TweenEasing easing)
        {
            if (this.m_CanvasGroup == null)
                return;
            
            // Check if currently transitioning
            if (this.m_Transitioning)
            {
                this.m_FloatTweenRunner.StopTween();
            }

            if (duration == 0f || !Application.isPlaying)
            {
                this.SetAlpha(targetAlpha);
            }
            else
            {
                this.m_Transitioning = true;

                var floatTween = new FloatTween { duration = duration, startFloat = this.m_CanvasGroup.alpha, targetFloat = targetAlpha };
                floatTween.AddOnChangedCallback(SetAlpha);
                floatTween.ignoreTimeScale = true;
                floatTween.easing = easing;
                floatTween.AddOnFinishCallback(OnTweenFinished);

                this.m_FloatTweenRunner.StartTween(floatTween);
            }
        }

        public void SetAlpha(float alpha)
		{
            if (this.m_CanvasGroup != null)
			    this.m_CanvasGroup.alpha = alpha;
		}

        protected void OnTweenFinished()
        {
            this.m_Transitioning = false;
        }

        /// <summary>
        /// Gets the black overlay based on relative game object. (In case we have multiple canvases.)
        /// </summary>
        /// <param name="relativeGameObject">The relative game object.</param>
        /// <returns>The black overlay component.</returns>
        public static UIBlackOverlay GetOverlay(GameObject relativeGameObject)
        {
            // Find the black overlay in the current canvas
            Canvas canvas = UIUtility.FindInParents<Canvas>(relativeGameObject);

            if (canvas != null)
            {
                // Try finding an overlay in the canvas
                UIBlackOverlay overlay = canvas.gameObject.GetComponentInChildren<UIBlackOverlay>();

                if (overlay != null)
                    return overlay;

                // In case no overlay is found instantiate one
                if (UIBlackOverlayManager.Instance != null && UIBlackOverlayManager.Instance.prefab != null)
                    return UIBlackOverlayManager.Instance.Create(canvas.transform);
            }

            return null;
        }
	}
}
