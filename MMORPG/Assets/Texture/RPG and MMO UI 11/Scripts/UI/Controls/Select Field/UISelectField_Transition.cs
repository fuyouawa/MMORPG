using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    [ExecuteInEditMode, AddComponentMenu("UI/Select Field - Transition", 58), RequireComponent(typeof(UISelectField))]
    public class UISelectField_Transition : MonoBehaviour
    {
        [SerializeField, Tooltip("Graphic that will have the selected transtion applied.")]
        private Graphic m_TargetGraphic;

        [SerializeField, Tooltip("GameObject that will have the selected transtion applied.")]
        private GameObject m_TargetGameObject;

        [SerializeField] private Selectable.Transition m_Transition = Selectable.Transition.None;
        [SerializeField] private ColorBlockExtended m_Colors = ColorBlockExtended.defaultColorBlock;
        [SerializeField] private SpriteStateExtended m_SpriteState;
        [SerializeField] private AnimationTriggersExtended m_AnimationTriggers = new AnimationTriggersExtended();

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
        
        /// <summary>
        /// Gets or sets the transition type.
        /// </summary>
        public Selectable.Transition transition
        {
            get { return this.m_Transition; }
            set { this.m_Transition = value; }
        }

        /// <summary>
        /// Gets or sets the color block.
        /// </summary>
        public ColorBlockExtended colors
        {
            get { return this.m_Colors; }
            set { this.m_Colors = value; }
        }

        /// <summary>
        /// Gets or sets the sprite state.
        /// </summary>
        public SpriteStateExtended spriteState
        {
            get { return this.m_SpriteState; }
            set { this.m_SpriteState = value; }
        }

        /// <summary>
        /// Gets or sets the animation triggers.
        /// </summary>
        public AnimationTriggersExtended animationTriggers
        {
            get { return this.m_AnimationTriggers; }
            set { this.m_AnimationTriggers = value; }
        }

        /// <summary>
        /// The select field.
        /// </summary>
        private UISelectField m_Select;

        protected void Awake()
        {
            this.m_Select = this.gameObject.GetComponent<UISelectField>();
        }

        protected void OnEnable()
        {
            if (this.m_Select != null)
            {
                this.m_Select.onTransition.AddListener(OnTransition);
            }

            this.OnTransition(UISelectField.VisualState.Normal, true);
        }

        protected void OnDisable()
        {
            if (this.m_Select != null)
            {
                this.m_Select.onTransition.RemoveListener(OnTransition);
            }

            this.InstantClearState();
        }

        /// <summary>
		/// Instantly clears the visual state.
		/// </summary>
		protected void InstantClearState()
        {
            switch (this.m_Transition)
            {
                case Selectable.Transition.ColorTint:
                    this.StartColorTween(Color.white, true);
                    break;
                case Selectable.Transition.SpriteSwap:
                    this.DoSpriteSwap(null);
                    break;
            }
        }

        public void OnTransition(UISelectField.VisualState state, bool instant)
        {
            if ((this.targetGraphic == null && this.targetGameObject == null) || !this.gameObject.activeInHierarchy || this.m_Transition == Selectable.Transition.None)
                return;

            Color color = this.colors.normalColor;
            Sprite newSprite = null;
            string triggername = this.animationTriggers.normalTrigger;
            
            // Prepare the state values
            switch (state)
            {
                case UISelectField.VisualState.Normal:
                    color = this.colors.normalColor;
                    newSprite = null;
                    triggername = this.animationTriggers.normalTrigger;
                    break;
                case UISelectField.VisualState.Highlighted:
                    color = this.colors.highlightedColor;
                    newSprite = this.spriteState.highlightedSprite;
                    triggername = this.animationTriggers.highlightedTrigger;
                    break;
                case UISelectField.VisualState.Pressed:
                    color = this.colors.pressedColor;
                    newSprite = this.spriteState.pressedSprite;
                    triggername = this.animationTriggers.pressedTrigger;
                    break;
                case UISelectField.VisualState.Active:
                    color = this.colors.activeColor;
                    newSprite = this.spriteState.activeSprite;
                    triggername = this.animationTriggers.activeTrigger;
                    break;
                case UISelectField.VisualState.ActiveHighlighted:
                    color = this.colors.activeHighlightedColor;
                    newSprite = this.spriteState.activeHighlightedSprite;
                    triggername = this.animationTriggers.activeHighlightedTrigger;
                    break;
                case UISelectField.VisualState.ActivePressed:
                    color = this.colors.activePressedColor;
                    newSprite = this.spriteState.activePressedSprite;
                    triggername = this.animationTriggers.activePressedTrigger;
                    break;
                case UISelectField.VisualState.Disabled:
                    color = this.colors.disabledColor;
                    newSprite = this.spriteState.disabledSprite;
                    triggername = this.animationTriggers.disabledTrigger;
                    break;
            }

            // Do the transition
            switch (this.m_Transition)
            {
                case Selectable.Transition.ColorTint:
                    this.StartColorTween(color * this.colors.colorMultiplier, (instant ? true : (this.colors.fadeDuration == 0f)));
                    break;
                case Selectable.Transition.SpriteSwap:
                    this.DoSpriteSwap(newSprite);
                    break;
                case Selectable.Transition.Animation:
                    this.TriggerAnimation(triggername);
                    break;
            }
        }

        private void StartColorTween(Color color, bool instant)
        {
            if (this.targetGraphic == null)
                return;

            if (instant)
            {
                this.targetGraphic.canvasRenderer.SetColor(color);
            }
            else
            {
                this.targetGraphic.CrossFadeColor(color, this.colors.fadeDuration, true, true);
            }
        }

        private void DoSpriteSwap(Sprite newSprite)
        {
            if (this.targetGraphic == null)
                return;

            Image image = this.targetGraphic as Image;

            if (image != null)
                image.overrideSprite = newSprite;
        }

        private void TriggerAnimation(string trigger)
        {
            Animator animator = this.GetComponent<Animator>();

            if (animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || !animator.hasBoundPlayables || string.IsNullOrEmpty(trigger))
                return;

            animator.ResetTrigger(this.animationTriggers.normalTrigger);
            animator.ResetTrigger(this.animationTriggers.pressedTrigger);
            animator.ResetTrigger(this.animationTriggers.highlightedTrigger);
            animator.ResetTrigger(this.animationTriggers.activeTrigger);
            animator.ResetTrigger(this.animationTriggers.activeHighlightedTrigger);
            animator.ResetTrigger(this.animationTriggers.activePressedTrigger);
            animator.ResetTrigger(this.animationTriggers.disabledTrigger);
            animator.SetTrigger(trigger);
        }
    }
}
