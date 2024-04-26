using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DuloGames.UI.Tweens;
using System;

namespace DuloGames.UI
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(CanvasGroup)), AddComponentMenu("UI/UI Scene/Scene")]
    public class UIScene : MonoBehaviour
    {
        public enum Type
        {
            Preloaded,
            Prefab,
            Resource
        }

        public enum Transition
        {
            None,
            Animation,
            CrossFade,
            SlideFromRight,
            SlideFromLeft,
            SlideFromTop,
            SlideFromBottom
        }
        
        [Serializable] public class OnActivateEvent : UnityEvent<UIScene> { }
        [Serializable] public class OnDeactivateEvent : UnityEvent<UIScene> { }

        private UISceneRegistry m_SceneManager;
        private bool m_AnimationState = false;

        #pragma warning disable 0649
        [SerializeField] private int m_Id = 0;
        [SerializeField] private bool m_IsActivated = true;
        [SerializeField] private Type m_Type = Type.Preloaded;
        [SerializeField] private Transform m_Content;
        [SerializeField] private GameObject m_Prefab;
        [SerializeField][ResourcePath] private string m_Resource;
        [SerializeField] private Transition m_Transition = Transition.None;
        [SerializeField] private float m_TransitionDuration = 0.2f;
        [SerializeField] private TweenEasing m_TransitionEasing = TweenEasing.InOutQuint;
        [SerializeField] private string m_AnimateInTrigger = "AnimateIn";
        [SerializeField] private string m_AnimateOutTrigger = "AnimateOut";
        [SerializeField] private GameObject m_FirstSelected;
        #pragma warning restore 0649

        /// <summary>
        /// Gets the scene id.
        /// </summary>
        public int id
        {
            get { return this.m_Id; }
        }

        /// <summary>
        /// Gets or sets value indicating whether the scene is activated.
        /// </summary>
        public bool isActivated
        {
            get { return this.m_IsActivated; }
            set { if (value) { this.Activate(); } else { this.Deactivate(); } }
        }

        /// <summary>
        /// Gets the scene type.
        /// </summary>
        public Type type
        {
            get { return this.m_Type; }
        }

        /// <summary>
        /// Gets or sets the scene content holder.
        /// </summary>
        public Transform content
        {
            get { return this.m_Content; }
            set { this.m_Content = value; }
        }

        /// <summary>
        /// Gets or sets the scene transition.
        /// </summary>
        public Transition transition
        {
            get { return this.m_Transition; }
            set { this.m_Transition = value; }
        }

        /// <summary>
        /// Gets or sets the transition duration.
        /// </summary>
        public float transitionDuration
        {
            get { return this.m_TransitionDuration; }
            set { this.m_TransitionDuration = value; }
        }

        /// <summary>
        /// Gets or set the transition easing.
        /// </summary>
        public TweenEasing transitionEasing
        {
            get { return this.m_TransitionEasing; }
            set { this.m_TransitionEasing = value; }
        }

        /// <summary>
        /// Gets or sets the animate in trigger.
        /// </summary>
        public string animateInTrigger
        {
            get { return this.m_AnimateInTrigger; }
            set { this.m_AnimateInTrigger = value; }
        }

        /// <summary>
        /// Gets or sets the animate out trigger.
        /// </summary>
        public string animateOutTrigger
        {
            get { return this.m_AnimateOutTrigger; }
            set { this.m_AnimateOutTrigger = value; }
        }

        /// <summary>
		/// Invoked when the scene is activated.
		/// </summary>
		public OnActivateEvent onActivate = new OnActivateEvent();

        /// <summary>
        /// Invoked when the scene is deactivated.
        /// </summary>
        public OnDeactivateEvent onDeactivate = new OnDeactivateEvent();

        /// <summary>
        /// Gets the rect transform.
        /// </summary>
        public RectTransform rectTransform
        {
            get { return (this.transform as RectTransform); }
        }

        /// <summary>
		/// Gets the animator.
		/// </summary>
		/// <value>The animator.</value>
		public Animator animator
        {
            get { return this.gameObject.GetComponent<Animator>(); }
        }

        // The canvas group
        private CanvasGroup m_CanvasGroup;

        // Tween controls
        [NonSerialized]
        private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        // Called by Unity prior to deserialization, 
        // should not be called by users
        protected UIScene()
        {
            if (this.m_FloatTweenRunner == null)
                this.m_FloatTweenRunner = new TweenRunner<FloatTween>();

            this.m_FloatTweenRunner.Init(this);
        }

        protected virtual void Awake()
        {
            // Get the scene mangaer
            this.m_SceneManager = UISceneRegistry.instance;

            if (this.m_SceneManager == null)
            {
                Debug.LogWarning("Scene registry does not exist.");
                this.enabled = false;
                return;
            }

            // Set the initial animation state
            this.m_AnimationState = this.m_IsActivated;

            // Get the canvas group
            this.m_CanvasGroup = this.gameObject.GetComponent<CanvasGroup>();

            // Set the first selected game object for the navigation
            if (Application.isPlaying && this.isActivated && this.isActiveAndEnabled && this.m_FirstSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(this.m_FirstSelected);
            }
        }

        protected virtual void Start() { }

        protected virtual void OnEnable()
        {
            // Register the scene
            if (this.m_SceneManager != null)
            {
                // Register only if in the scene
                if (this.gameObject.activeInHierarchy)
                {
                    this.m_SceneManager.RegisterScene(this);
                }
            }

            // Trigger the on activate event
            if (this.isActivated && this.onActivate != null)
                this.onActivate.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            // Unregister the scene
            if (this.m_SceneManager != null)
            {
                this.m_SceneManager.UnregisterScene(this);
            }
        }
        
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // Check for duplicate id
            if (this.m_SceneManager != null)
            {
                UIScene[] scenes = this.m_SceneManager.scenes;

                foreach (UIScene scene in scenes)
                {
                    if (!scene.Equals(this) && scene.id == this.m_Id)
                    {
                        Debug.LogWarning("Duplicate scene ids, change the id of scene: " + this.gameObject.name);
                        this.m_Id = this.m_SceneManager.GetAvailableSceneId();
                        break;
                    }
                }
            }

            // Activate or deactivate the scene
            if (this.m_Type == Type.Preloaded)
            {
                if (this.m_IsActivated)
                {
                    // Enable the game object
                    if (this.m_Content != null)
                        this.m_Content.gameObject.SetActive(true);
                }
                else
                {
                    // Disable the game object
                    if (this.m_Content != null)
                        this.m_Content.gameObject.SetActive(false);
                }
            }

            this.m_TransitionDuration = Mathf.Max(this.m_TransitionDuration, 0f);
        }
#endif

        protected void Update()
        {
            if (this.animator != null && !string.IsNullOrEmpty(this.m_AnimateInTrigger) && !string.IsNullOrEmpty(this.m_AnimateOutTrigger))
            {
                AnimatorStateInfo state = this.animator.GetCurrentAnimatorStateInfo(0);

                // Check which is the current state
                if (state.IsName(this.m_AnimateInTrigger) && !this.m_AnimationState)
                {
                    if (state.normalizedTime >= state.length)
                    {
                        // Flag as opened
                        this.m_AnimationState = true;

                        // On animation finished
                        this.OnTransitionIn();
                    }
                }
                else if (state.IsName(this.m_AnimateOutTrigger) && this.m_AnimationState)
                {
                    if (state.normalizedTime >= state.length)
                    {
                        // Flag as closed
                        this.m_AnimationState = false;

                        // On animation finished
                        this.OnTransitionOut();
                    }
                }
            }
        }

        /// <summary>
        /// Activate the scene, no transition is used.
        /// </summary>
        public void Activate()
        {
            // Make sure the scene is active and enabled
            if (!this.isActiveAndEnabled || !this.gameObject.activeInHierarchy)
                return;

            // If it's prefab
            if (this.m_Type == Type.Prefab || this.m_Type == Type.Resource)
            {
                GameObject prefab = null;

                if (this.m_Type == Type.Prefab)
                {
                    // Check the prefab
                    if (this.m_Prefab == null)
                    {
                        Debug.LogWarning("You are activating a prefab scene and no prefab is specified.");
                        return;
                    }

                    prefab = this.m_Prefab;
                }

                if (this.m_Type == Type.Resource)
                {
                    // Try loading the resource
                    if (string.IsNullOrEmpty(this.m_Resource))
                    {
                        Debug.LogWarning("You are activating a resource scene and no resource path is specified.");
                        return;
                    }

                    prefab = Resources.Load<GameObject>(this.m_Resource);
                }

                // Instantiate the prefab
                if (prefab != null)
                {
                    // Instantiate the prefab
                    GameObject obj = Instantiate<GameObject>(prefab);
                    
                    // Set the content variable
                    this.m_Content = obj.transform;

                    // Set parent
                    this.m_Content.SetParent(this.transform);

                    // Check if it's a rect transform
                    if (this.m_Content is RectTransform)
                    {
                        // Get the rect transform
                        RectTransform rectTransform = this.m_Content as RectTransform;

                        // Prepare the rect
                        rectTransform.localScale = Vector3.one;
                        rectTransform.localPosition = Vector3.zero;

                        // Set anchor and pivot
                        rectTransform.anchorMin = new Vector2(0f, 0f);
                        rectTransform.anchorMax = new Vector2(1f, 1f);
                        rectTransform.pivot = new Vector2(0.5f, 0.5f);

                        // Get the canvas size
                        Canvas canvas = UIUtility.FindInParents<Canvas>(this.gameObject);

                        if (canvas == null)
                        {
                            canvas = this.gameObject.GetComponentInChildren<Canvas>();
                        }

                        if (canvas != null)
                        {
                            RectTransform crt = canvas.transform as RectTransform;

                            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, crt.sizeDelta.x);
                            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, crt.sizeDelta.y);
                        }

                        // Set position
                        rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
                    }
                }
            }

            // Enable the game object
            if (this.m_Content != null)
            {
                this.m_Content.gameObject.SetActive(true);
            }

            // Set the first selected for the navigation
            if (this.isActiveAndEnabled && this.m_FirstSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(this.m_FirstSelected);
            }

            // Set the active variable
            this.m_IsActivated = true;

            // Invoke the event
            if (this.onActivate != null)
            {
                this.onActivate.Invoke(this);
            }
        }

        /// <summary>
        /// Deactivate the scene, no transition is used.
        /// </summary>
        public void Deactivate()
        {
            // Disable the game object
            if (this.m_Content != null)
            {
                this.m_Content.gameObject.SetActive(false);
            }

            // If prefab destroy the object
            if (this.m_Type == Type.Prefab || this.m_Type == Type.Resource)
            {
                Destroy(this.m_Content.gameObject);
            }

            // Unload unused assets
            Resources.UnloadUnusedAssets();

            // Set the active variable
            this.m_IsActivated = false;

            // Invoke the event
            if (this.onDeactivate != null)
            {
                this.onDeactivate.Invoke(this);
            }
        }

        /// <summary>
        /// Transition to the scene.
        /// </summary>
        public void TransitionTo()
        {
            // Make sure the scene is active and enabled
            if (!this.isActiveAndEnabled || !this.gameObject.activeInHierarchy)
                return;

            if (this.m_SceneManager != null)
            {
                this.m_SceneManager.TransitionToScene(this);
            }
        }

        /// <summary>
        /// Transition the scene in.
        /// </summary>
        public void TransitionIn()
        {
            this.TransitionIn(this.m_Transition, this.m_TransitionDuration, this.m_TransitionEasing);
        }

        /// <summary>
        /// Transition the scene in.
        /// </summary>
        /// <param name="transition">The transition.</param>
        /// <param name="duration">The transition duration.</param>
        /// <param name="easing">The transition easing.</param>
        public void TransitionIn(Transition transition, float duration, TweenEasing easing)
        {
            // Make sure the scene is active and enabled
            if (!this.isActiveAndEnabled || !this.gameObject.activeInHierarchy)
                return;

            if (this.m_CanvasGroup == null)
                return;

            // If no transition is used
            if (transition == Transition.None)
            {
                this.Activate();
                return;
            }

            // If the transition is animation
            if (transition == Transition.Animation)
            {
                this.Activate();
                this.TriggerAnimation(this.m_AnimateInTrigger);
                return;
            }

            // Make the scene non interactable
            //this.m_CanvasGroup.interactable = false;
            //this.m_CanvasGroup.blocksRaycasts = false;

            // Prepare some variable
            Vector2 rectSize = this.rectTransform.rect.size;

            // Prepare the rect transform
            if (transition == Transition.SlideFromLeft || transition == Transition.SlideFromRight || transition == Transition.SlideFromTop || transition == Transition.SlideFromBottom)
            {
                // Anchor and pivot top left
                this.rectTransform.pivot = new Vector2(0f, 1f);
                this.rectTransform.anchorMin = new Vector2(0f, 1f);
                this.rectTransform.anchorMax = new Vector2(0f, 1f);
                this.rectTransform.sizeDelta = rectSize;
            }

            // Prepare the tween
            FloatTween floatTween = new FloatTween();
            floatTween.duration = duration;

            switch (transition)
            {
                case Transition.CrossFade:
                    this.m_CanvasGroup.alpha = 0f;
                    floatTween.startFloat = 0f;
                    floatTween.targetFloat = 1f;
                    floatTween.AddOnChangedCallback(SetCanvasAlpha);
                    break;
                case Transition.SlideFromRight:
                    this.rectTransform.anchoredPosition = new Vector2(rectSize.x, 0f);
                    floatTween.startFloat = rectSize.x;
                    floatTween.targetFloat = 0f;
                    floatTween.AddOnChangedCallback(SetPositionX);
                    break;
                case Transition.SlideFromLeft:
                    this.rectTransform.anchoredPosition = new Vector2((rectSize.x * -1f), 0f);
                    floatTween.startFloat = (rectSize.x * -1f);
                    floatTween.targetFloat = 0f;
                    floatTween.AddOnChangedCallback(SetPositionX);
                    break;
                case Transition.SlideFromBottom:
                    this.rectTransform.anchoredPosition = new Vector2(0f, (rectSize.y * -1f));
                    floatTween.startFloat = (rectSize.y * -1f);
                    floatTween.targetFloat = 0f;
                    floatTween.AddOnChangedCallback(SetPositionY);
                    break;
                case Transition.SlideFromTop:
                    this.rectTransform.anchoredPosition = new Vector2(0f, rectSize.y);
                    floatTween.startFloat = rectSize.y;
                    floatTween.targetFloat = 0f;
                    floatTween.AddOnChangedCallback(SetPositionY);
                    break;
            }

            // Activate the scene
            this.Activate();

            // Start the transition
            floatTween.AddOnFinishCallback(OnTransitionIn);
            floatTween.ignoreTimeScale = true;
            floatTween.easing = easing;
            this.m_FloatTweenRunner.StartTween(floatTween);
        }

        /// <summary>
        /// Transition the scene out.
        /// </summary>
        public void TransitionOut()
        {
            this.TransitionOut(this.m_Transition, this.m_TransitionDuration, this.m_TransitionEasing);
        }

        /// <summary>
        /// Transition the scene out.
        /// </summary>
        /// <param name="transition">The transition.</param>
        /// <param name="duration">The transition duration.</param>
        /// <param name="easing">The transition easing.</param>
        public void TransitionOut(Transition transition, float duration, TweenEasing easing)
        {
            // Make sure the scene is active and enabled
            if (!this.isActiveAndEnabled || !this.gameObject.activeInHierarchy)
                return;

            if (this.m_CanvasGroup == null)
                return;

            // If no transition is used
            if (transition == Transition.None)
            {
                this.Deactivate();
                return;
            }
            
            // If the transition is animation
            if (transition == Transition.Animation)
            {
                this.TriggerAnimation(this.m_AnimateOutTrigger);
                return;
            }

            // Make the scene non interactable
            //this.m_CanvasGroup.interactable = false;
            //this.m_CanvasGroup.blocksRaycasts = false;

            // Prepare some variable
            Vector2 rectSize = this.rectTransform.rect.size;

            // Prepare the rect transform
            if (transition == Transition.SlideFromLeft || transition == Transition.SlideFromRight || transition == Transition.SlideFromTop || transition == Transition.SlideFromBottom)
            {
                // Anchor and pivot top left
                this.rectTransform.pivot = new Vector2(0f, 1f);
                this.rectTransform.anchorMin = new Vector2(0f, 1f);
                this.rectTransform.anchorMax = new Vector2(0f, 1f);
                this.rectTransform.sizeDelta = rectSize;
                this.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            }

            // Prepare the tween
            FloatTween floatTween = new FloatTween();
            floatTween.duration = duration;

            switch (transition)
            {
                case Transition.CrossFade:
                    this.m_CanvasGroup.alpha = 1f;
                    // Start the tween
                    floatTween.startFloat = this.m_CanvasGroup.alpha;
                    floatTween.targetFloat = 0f;
                    floatTween.AddOnChangedCallback(SetCanvasAlpha);
                    break;
                case Transition.SlideFromRight:
                    // Start the tween
                    floatTween.startFloat = 0f;
                    floatTween.targetFloat = (rectSize.x * -1f);
                    floatTween.AddOnChangedCallback(SetPositionX);
                    break;
                case Transition.SlideFromLeft:
                    // Start the tween
                    floatTween.startFloat = 0f;
                    floatTween.targetFloat = rectSize.x;
                    floatTween.AddOnChangedCallback(SetPositionX);
                    break;
                case Transition.SlideFromBottom:
                    // Start the tween
                    floatTween.startFloat = 0f;
                    floatTween.targetFloat = rectSize.y;
                    floatTween.AddOnChangedCallback(SetPositionY);
                    break;
                case Transition.SlideFromTop:
                    // Start the tween
                    floatTween.startFloat = 0f;
                    floatTween.targetFloat = (rectSize.y * -1f);
                    floatTween.AddOnChangedCallback(SetPositionY);
                    break;
            }

            // Start the transition
            floatTween.AddOnFinishCallback(OnTransitionOut);
            floatTween.ignoreTimeScale = true;
            floatTween.easing = easing;
            this.m_FloatTweenRunner.StartTween(floatTween);
        }

        /// <summary>
		/// Starts alpha tween.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="duration">Duration.</param>
        /// <param name="easing">Easing.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
        /// <param name="callback">Event to be called on transition finish.</param>
		public void StartAlphaTween(float targetAlpha, float duration, TweenEasing easing, bool ignoreTimeScale, UnityAction callback)
        {
            if (this.m_CanvasGroup == null)
                return;

            // Start the tween
            var floatTween = new FloatTween { duration = duration, startFloat = this.m_CanvasGroup.alpha, targetFloat = targetAlpha };
            floatTween.AddOnChangedCallback(SetCanvasAlpha);
            floatTween.AddOnFinishCallback(callback);
            floatTween.ignoreTimeScale = ignoreTimeScale;
            floatTween.easing = easing;
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
        }

        /// <summary>
        /// Sets the rect transform anchored position X.
        /// </summary>
        /// <param name="x">The X position.</param>
        public void SetPositionX(float x)
        {
            this.rectTransform.anchoredPosition = new Vector2(x, this.rectTransform.anchoredPosition.y);
        }

        /// <summary>
        /// Sets the rect transform anchored position Y.
        /// </summary>
        /// <param name="y">The Y position.</param>
        public void SetPositionY(float y)
        {
            this.rectTransform.anchoredPosition = new Vector2(this.rectTransform.anchoredPosition.x, y);
        }

        /// <summary>
        /// Triggers animation.
        /// </summary>
        /// <param name="triggername"></param>
        private void TriggerAnimation(string triggername)
        {
            // Get the animator on the target game object
            Animator animator = this.gameObject.GetComponent<Animator>();

            if (animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(this.m_AnimateInTrigger);
            animator.ResetTrigger(this.m_AnimateOutTrigger);
            animator.SetTrigger(triggername);
        }

        /// <summary>
		/// Raises the transition in finished event.
		/// </summary>
		protected virtual void OnTransitionIn()
        {
            // Re-enable the canvas group interaction
            if (this.m_CanvasGroup != null)
            {
                //this.m_CanvasGroup.interactable = true;
                //this.m_CanvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
		/// Raises the transition out finished event.
		/// </summary>
		protected virtual void OnTransitionOut()
        {
            // Deactivate the scene
            this.Deactivate();

            // Re-enable the canvas group interaction
            if (this.m_CanvasGroup != null)
            {
                //this.m_CanvasGroup.interactable = true;
                //this.m_CanvasGroup.blocksRaycasts = true;
            }

            // Reset the alpha
            this.SetCanvasAlpha(1f);

            // Reset the position of the transform
            this.SetPositionX(0f);
        }
    }
}
