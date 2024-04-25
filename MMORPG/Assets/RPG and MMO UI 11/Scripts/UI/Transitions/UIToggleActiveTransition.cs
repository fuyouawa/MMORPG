using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{
    [ExecuteInEditMode, AddComponentMenu("UI/Toggle Active Transition")]
    public class UIToggleActiveTransition : MonoBehaviour, IEventSystemHandler
    {
        public enum VisualState
        {
            Normal,
            Active
        }

        public enum Transition
        {
            None,
            ColorTint,
            SpriteSwap,
            Animation,
            TextColor,
            CanvasGroup
        }

        #pragma warning disable 0649
        [SerializeField] private Transition m_Transition = Transition.None;

        [SerializeField] private Color m_NormalColor = new Color(1f, 1f, 1f, 0f);
        [SerializeField] private Color m_ActiveColor = Color.white;
        [SerializeField] private float m_Duration = 0.1f;

        [SerializeField, Range(1f, 6f)] private float m_ColorMultiplier = 1f;
        
        [SerializeField] private Sprite m_ActiveSprite;

        [SerializeField] private string m_NormalTrigger = "Normal";
        [SerializeField] private string m_ActiveBool = "Active";

        [SerializeField][Range(0f, 1f)] private float m_NormalAlpha = 0f;
        [SerializeField][Range(0f, 1f)] private float m_ActiveAlpha = 1f;

        [SerializeField, Tooltip("Graphic that will have the selected transtion applied.")]
        private Graphic m_TargetGraphic;

        [SerializeField, Tooltip("GameObject that will have the selected transtion applied.")]
        private GameObject m_TargetGameObject;

        [SerializeField, Tooltip("CanvasGroup that will have the selected transtion applied.")]
        private CanvasGroup m_TargetCanvasGroup;
        
        [SerializeField] private Toggle m_TargetToggle;
        #pragma warning restore 0649

        private bool m_Active = false;
        
        /// <summary>
        /// Gets or sets the transition type.
        /// </summary>
        /// <value>The transition.</value>
        public Transition transition
        {
            get
            {
                return this.m_Transition;
            }
            set
            {
                this.m_Transition = value;
            }
        }

        /// <summary>
        /// Gets or sets the target graphic.
        /// </summary>
        /// <value>The target graphic.</value>
        public Graphic targetGraphic
        {
            get
            {
                return this.m_TargetGraphic;
            }
            set
            {
                this.m_TargetGraphic = value;
            }
        }

        /// <summary>
        /// Gets or sets the target game object.
        /// </summary>
        /// <value>The target game object.</value>
        public GameObject targetGameObject
        {
            get
            {
                return this.m_TargetGameObject;
            }
            set
            {
                this.m_TargetGameObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the target canvas group.
        /// </summary>
        /// <value>The target canvas group.</value>
        public CanvasGroup targetCanvasGroup
        {
            get
            {
                return this.m_TargetCanvasGroup;
            }
            set
            {
                this.m_TargetCanvasGroup = value;
            }
        }

        /// <summary>
        /// Gets the animator.
        /// </summary>
        /// <value>The animator.</value>
        public Animator animator
        {
            get
            {
                if (this.m_TargetGameObject != null)
                    return this.m_TargetGameObject.GetComponent<Animator>();

                // Default
                return null;
            }
        }

        // Tween controls
        [System.NonSerialized]
        private readonly TweenRunner<ColorTween> m_ColorTweenRunner;
        [System.NonSerialized]
        private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        // Called by Unity prior to deserialization, 
        // should not be called by users
        protected UIToggleActiveTransition()
        {
            if (this.m_ColorTweenRunner == null)
                this.m_ColorTweenRunner = new TweenRunner<ColorTween>();

            if (this.m_FloatTweenRunner == null)
                this.m_FloatTweenRunner = new TweenRunner<FloatTween>();

            this.m_ColorTweenRunner.Init(this);
            this.m_FloatTweenRunner.Init(this);
        }

        protected void Awake()
        {
            if (this.m_TargetToggle == null)
                this.m_TargetToggle = this.gameObject.GetComponent<Toggle>();

            if (this.m_TargetToggle != null)
                this.m_Active = this.m_TargetToggle.isOn;
        }

        protected void OnEnable()
        {
            if (this.m_TargetToggle != null)
                this.m_TargetToggle.onValueChanged.AddListener(OnToggleValueChange);

            this.InternalEvaluateAndTransitionToNormalState(true);
        }

        protected void OnDisable()
        {
            if (this.m_TargetToggle != null)
                this.m_TargetToggle.onValueChanged.RemoveListener(OnToggleValueChange);

            this.InstantClearState();
        }

        /// <summary>
		/// Internally evaluates and transitions to normal state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionToNormalState(bool instant)
        {
            this.DoStateTransition(this.m_Active ? VisualState.Active : VisualState.Normal, instant);
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            this.m_Duration = Mathf.Max(this.m_Duration, 0f);

            if (this.isActiveAndEnabled)
            {
                this.DoSpriteSwap(null);

                if (this.m_Transition != Transition.CanvasGroup)
                    this.InternalEvaluateAndTransitionToNormalState(true);
            }
        }
#endif
        
        protected void OnToggleValueChange(bool value)
        {
            if (this.m_TargetToggle == null)
                return;

            this.m_Active = this.m_TargetToggle.isOn;

            if (this.m_Transition == Transition.Animation)
            {
                if (this.targetGameObject == null || this.animator == null || !this.animator.isActiveAndEnabled || this.animator.runtimeAnimatorController == null || string.IsNullOrEmpty(this.m_ActiveBool))
                    return;

                this.animator.SetBool(this.m_ActiveBool, this.m_Active);
            }

            this.DoStateTransition(this.m_Active ? VisualState.Active : VisualState.Normal, false);
        }

        /// <summary>
        /// Instantly clears the visual state.
        /// </summary>
        protected void InstantClearState()
        {
            switch (this.m_Transition)
            {
                case Transition.ColorTint:
                    this.StartColorTween(Color.white, true);
                    break;
                case Transition.SpriteSwap:
                    this.DoSpriteSwap(null);
                    break;
                case Transition.TextColor:
                    this.SetTextColor(this.m_NormalColor);
                    break;
                case Transition.CanvasGroup:
                    this.SetCanvasGroupAlpha(1f);
                    break;
            }
        }
        
        /// <summary>
        /// Does the state transition.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        protected virtual void DoStateTransition(VisualState state, bool instant)
        {
            // Check if active in the scene
            if (!this.gameObject.activeInHierarchy)
                return;
            
            Color color = this.m_NormalColor;
            Sprite newSprite = null;
            string triggername = this.m_NormalTrigger;
            float alpha = this.m_NormalAlpha;

            // Prepare the transition values
            switch (state)
            {
                case VisualState.Normal:
                    color = this.m_NormalColor;
                    newSprite = null;
                    triggername = this.m_NormalTrigger;
                    alpha = this.m_NormalAlpha;
                    break;
                case VisualState.Active:
                    color = this.m_ActiveColor;
                    newSprite = this.m_ActiveSprite;
                    triggername = this.m_NormalTrigger;
                    alpha = this.m_ActiveAlpha;
                    break;
            }

            // Do the transition
            switch (this.m_Transition)
            {
                case Transition.ColorTint:
                    this.StartColorTween(color * this.m_ColorMultiplier, instant);
                    break;
                case Transition.SpriteSwap:
                    this.DoSpriteSwap(newSprite);
                    break;
                case Transition.Animation:
                    this.TriggerAnimation(triggername);
                    break;
                case Transition.TextColor:
                    this.StartTextColorTween(color, false);
                    break;
                case Transition.CanvasGroup:
                    this.StartCanvasGroupTween(alpha, instant);
                    break;
            }
        }

        /// <summary>
        /// Starts the color tween.
        /// </summary>
        /// <param name="targetColor">Target color.</param>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        private void StartColorTween(Color targetColor, bool instant)
        {
            if (this.m_TargetGraphic == null)
                return;

            if (instant || this.m_Duration == 0f || !Application.isPlaying)
            {
                this.m_TargetGraphic.canvasRenderer.SetColor(targetColor);
            }
            else
            {
                this.m_TargetGraphic.CrossFadeColor(targetColor, this.m_Duration, true, true);
            }
        }

        private void DoSpriteSwap(Sprite newSprite)
        {
            Image image = this.m_TargetGraphic as Image;

            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }

        private void TriggerAnimation(string triggername)
        {
            if (this.targetGameObject == null || this.animator == null || !this.animator.isActiveAndEnabled || this.animator.runtimeAnimatorController == null || !this.animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            this.animator.ResetTrigger(this.m_NormalTrigger);
            this.animator.SetTrigger(triggername);
        }

        private void StartTextColorTween(Color targetColor, bool instant)
        {
            if (this.m_TargetGraphic == null)
                return;

            if ((this.m_TargetGraphic is Text) == false)
                return;

            if (instant || this.m_Duration == 0f || !Application.isPlaying)
            {
                (this.m_TargetGraphic as Text).color = targetColor;
            }
            else
            {
                var colorTween = new ColorTween { duration = this.m_Duration, startColor = (this.m_TargetGraphic as Text).color, targetColor = targetColor };
                colorTween.AddOnChangedCallback(SetTextColor);
                colorTween.ignoreTimeScale = true;

                this.m_ColorTweenRunner.StartTween(colorTween);
            }
        }

        /// <summary>
		/// Sets the text color.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		private void SetTextColor(Color targetColor)
        {
            if (this.m_TargetGraphic == null)
                return;

            if (this.m_TargetGraphic is Text)
            {
                (this.m_TargetGraphic as Text).color = targetColor;
            }
        }

        /// <summary>
		/// Starts the color tween.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void StartCanvasGroupTween(float targetAlpha, bool instant)
        {
            if (this.m_TargetCanvasGroup == null)
                return;

            if (instant || this.m_Duration == 0f || !Application.isPlaying)
            {
                this.SetCanvasGroupAlpha(targetAlpha);
            }
            else
            {
                var floatTween = new FloatTween { duration = this.m_Duration, startFloat = this.m_TargetCanvasGroup.alpha, targetFloat = targetAlpha };
                floatTween.AddOnChangedCallback(SetCanvasGroupAlpha);
                floatTween.ignoreTimeScale = true;

                this.m_FloatTweenRunner.StartTween(floatTween);
            }
        }

        /// <summary>
        /// Sets the canvas group alpha value.
        /// </summary>
        /// <param name="alpha">The alpha value.</param>
        private void SetCanvasGroupAlpha(float alpha)
        {
            if (this.m_TargetCanvasGroup == null)
                return;

            this.m_TargetCanvasGroup.alpha = alpha;
        }
    }
}
