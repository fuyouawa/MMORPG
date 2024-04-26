using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DuloGames.UI
{
    public class UISelectField_OptionOverlay : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum VisualState
        {
            Normal,
            Highlighted,
            Selected,
            Pressed
        }

        public enum Transition
        {
            None,
            ColorTint
        }
        
        [SerializeField] private Transition m_Transition = Transition.None;
        [SerializeField] private ColorBlock m_ColorBlock = ColorBlock.defaultColorBlock;
        
        [SerializeField, Tooltip("Graphic that will have the selected transtion applied.")]
        private Graphic m_TargetGraphic;
        
        private bool m_Highlighted = false;
        private bool m_Selected = false;
        private bool m_Pressed = false;

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
        /// Gets or set the color block.
        /// </summary>
        public ColorBlock colorBlock
        {
            get
            {
                return this.m_ColorBlock;
            }
            set
            {
                this.m_ColorBlock = value;
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
        
        protected void OnEnable()
        {
            this.InternalEvaluateAndTransitionToNormalState(true);
        }

        protected void OnDisable()
        {
            this.InstantClearState();
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
            }
        }

        /// <summary>
        /// Internally evaluates and transitions to normal state.
        /// </summary>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        public void InternalEvaluateAndTransitionToNormalState(bool instant)
        {
            this.DoStateTransition(VisualState.Normal, instant);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            this.m_Highlighted = true;

            if (!this.m_Selected && !this.m_Pressed)
                this.DoStateTransition(VisualState.Highlighted, false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.m_Highlighted = false;

            if (!this.m_Selected && !this.m_Pressed)
                this.DoStateTransition(VisualState.Normal, false);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!this.m_Highlighted)
                return;

            this.m_Pressed = true;
            this.DoStateTransition(VisualState.Pressed, false);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            this.m_Pressed = false;

            VisualState newState = VisualState.Normal;

            if (this.m_Selected)
            {
                newState = VisualState.Selected;
            }
            else if (this.m_Highlighted)
            {
                newState = VisualState.Highlighted;
            }

            this.DoStateTransition(newState, false);
        }

        /// <summary>
        /// Does the state transition.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        protected virtual void DoStateTransition(VisualState state, bool instant)
        {
            // Check if the script is enabled
            if (!this.enabled || !this.gameObject.activeInHierarchy)
                return;

            Color color = this.m_ColorBlock.normalColor;

            // Prepare the transition values
            switch (state)
            {
                case VisualState.Normal:
                    color = this.m_ColorBlock.normalColor;
                    break;
                case VisualState.Highlighted:
                    color = this.m_ColorBlock.highlightedColor;
                    break;
                case VisualState.Pressed:
                    color = this.m_ColorBlock.pressedColor;
                    break;
            }

            // Do the transition
            switch (this.m_Transition)
            {
                case Transition.ColorTint:
                    this.StartColorTween(color * this.m_ColorBlock.colorMultiplier, instant);
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

            if (instant || this.m_ColorBlock.fadeDuration == 0f || !Application.isPlaying)
            {
                this.m_TargetGraphic.canvasRenderer.SetColor(targetColor);
            }
            else
            {
                this.m_TargetGraphic.CrossFadeColor(targetColor, this.m_ColorBlock.fadeDuration, true, true);
            }
        }
    }
}
