using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;
using System;

namespace DuloGames.UI
{
	[DisallowMultipleComponent, AddComponentMenu("UI/Tab", 58)]
	public class UITab : Toggle {
		
		public enum TextTransition
		{
			None,
			ColorTint
		}
		
		#pragma warning disable 0649
		[SerializeField] private GameObject m_TargetContent;
		
		[SerializeField] private Image m_ImageTarget;
		[SerializeField] private Selectable.Transition m_ImageTransition = Selectable.Transition.None;
		[SerializeField] private ColorBlockExtended m_ImageColors = ColorBlockExtended.defaultColorBlock;
		[SerializeField] private SpriteStateExtended m_ImageSpriteState;
		[SerializeField] private AnimationTriggersExtended m_ImageAnimationTriggers = new AnimationTriggersExtended();
		
		[SerializeField] private Text m_TextTarget;
		[SerializeField] private TextTransition m_TextTransition = TextTransition.None;
		[SerializeField] private ColorBlockExtended m_TextColors = ColorBlockExtended.defaultColorBlock;
		#pragma warning restore 0649

        /// <summary>
        /// Gets or sets the target content game object.
        /// </summary>
        public GameObject targetContent
        {
            get { return this.m_TargetContent; }
            set { this.m_TargetContent = value; }
        }

        /// <summary>
        /// Gets or sets the image target.
        /// </summary>
        public Image imageTarget
        {
            get { return this.m_ImageTarget; }
            set { this.m_ImageTarget = value; }
        }

        /// <summary>
        /// Gets or sets the image transition.
        /// </summary>
        public Transition imageTransition
        {
            get { return this.m_ImageTransition; }
            set { this.m_ImageTransition = value; }
        }

        /// <summary>
        /// Gets or sets the image color block.
        /// </summary>
        public ColorBlockExtended imageColors
        {
            get { return this.m_ImageColors; }
            set { this.m_ImageColors = value; }
        }

        /// <summary>
        /// Gets or sets the image sprite state.
        /// </summary>
        public SpriteStateExtended imageSpriteState
        {
            get { return this.m_ImageSpriteState; }
            set { this.m_ImageSpriteState = value; }
        }

        /// <summary>
        /// Gets or sets the image animation triggers.
        /// </summary>
        public AnimationTriggersExtended imageAnimationTriggers
        {
            get { return this.m_ImageAnimationTriggers; }
            set { this.m_ImageAnimationTriggers = value; }
        }

        /// <summary>
        /// Gets or sets the text target.
        /// </summary>
        public Text textTarget
        {
            get { return this.m_TextTarget; }
            set { this.m_TextTarget = value; }
        }

        /// <summary>
        /// Gets or sets the text transition.
        /// </summary>
        public TextTransition textTransition
        {
            get { return this.m_TextTransition; }
            set { this.m_TextTransition = value; }
        }

        /// <summary>
        /// Gets or sets the text colors block.
        /// </summary>
        public ColorBlockExtended textColors
        {
            get { return this.m_TextColors; }
            set { this.m_TextColors = value; }
        }


        private Selectable.SelectionState m_CurrentState = Selectable.SelectionState.Normal;
		
		// Tween controls
		[NonSerialized] private readonly TweenRunner<ColorTween> m_ColorTweenRunner;
		
		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UITab()
		{
			if (this.m_ColorTweenRunner == null)
				this.m_ColorTweenRunner = new TweenRunner<ColorTween>();
			
			this.m_ColorTweenRunner.Init(this);
		}

        protected override void Awake()
        {
            base.Awake();

            // Make sure we have toggle group
            if (this.group == null)
            {
                // Try to find the group in the parents
                ToggleGroup grp = UIUtility.FindInParents<ToggleGroup>(this.gameObject);

                if (grp != null)
                {
                    this.group = grp;
                }
                else
                {
                    // Add new group on the parent
                    this.group = this.transform.parent.gameObject.AddComponent<ToggleGroup>();
                }
            }
        }

        protected override void OnEnable()
		{
			base.OnEnable();

			// Hook an event listener
			this.onValueChanged.AddListener(OnToggleStateChanged);
			
			// Apply initial state
			this.InternalEvaluateAndTransitionState(true);
		}

        protected override void OnDisable()
        {
            base.OnDisable();

            // Unhook the event listener
            this.onValueChanged.RemoveListener(OnToggleStateChanged);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
		{
			base.OnValidate();
			
			this.m_ImageColors.fadeDuration = Mathf.Max(this.m_ImageColors.fadeDuration, 0f);
			this.m_TextColors.fadeDuration = Mathf.Max(this.m_TextColors.fadeDuration, 0f);
		}

        /// <summary>
        /// Raises the property change event from the editor.
        /// </summary>
        public void OnProperyChange_Editor()
        {
            if (!this.isActiveAndEnabled)
                return;
            
            this.DoSpriteSwap(this.m_ImageTarget, null);
            this.InternalEvaluateAndTransitionState(true);
        }
#endif
		
		/// <summary>
		/// Raises the toggle state changed event.
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		protected void OnToggleStateChanged(bool state)
		{
			if (!this.IsActive() || !Application.isPlaying)
				return;
			
			this.InternalEvaluateAndTransitionState(false);
		}
		
		/// <summary>
		/// Evaluates and toggles the content visibility.
		/// </summary>
		public void EvaluateAndToggleContent()
		{
			if (this.m_TargetContent != null)
				m_TargetContent.SetActive(this.isOn);
		}
		
		/// <summary>
		/// Internaly evaluates and transitions to the current state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionState(bool instant)
		{
            if (!this.isActiveAndEnabled)
                return;

            // Toggle the content
            this.EvaluateAndToggleContent();

#if UNITY_EDITOR
            // Transition the active graphic
            // Hackfix because unity is not toggling it in edit mode
            if (instant && !Application.isPlaying && this.graphic != null)
            {
                this.graphic.canvasRenderer.SetAlpha((!this.isOn) ? 0f : 1f);
            }
#endif

            // Transition the active graphic children
            if (this.graphic != null && this.graphic.transform.childCount > 0)
			{
                float targetAlpha = (!this.isOn) ? 0f : 1f;

                // Loop through the children
                foreach (Transform child in this.graphic.transform)
				{
					// Try getting a graphic component
					Graphic g = child.GetComponent<Graphic>();
					
					if (g != null)
					{
                        if (!g.canvasRenderer.GetAlpha().Equals(targetAlpha))
                        {
                            if (instant) g.canvasRenderer.SetAlpha(targetAlpha);
                            else g.CrossFadeAlpha(targetAlpha, 0.1f, true);
                        }
					}
				}
			}
			
			// Do a state transition
			this.DoStateTransition(this.m_CurrentState, instant);
		}
		
		/// <summary>
		/// Does the state transitioning.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (!this.isActiveAndEnabled)
				return;

			// Save the state as current state
			this.m_CurrentState = state;
			
			Color newImageColor = this.m_ImageColors.normalColor;
			Color newTextColor = this.m_TextColors.normalColor;
			Sprite newSprite = null;
			string imageTrigger = this.m_ImageAnimationTriggers.normalTrigger;
			
			// Prepare state values
			switch (state)
			{
				case Selectable.SelectionState.Normal:
					newImageColor = (!this.isOn) ? this.m_ImageColors.normalColor : this.m_ImageColors.activeColor;
					newTextColor = (!this.isOn) ? this.m_TextColors.normalColor : this.m_TextColors.activeColor;
					newSprite = (!this.isOn) ? null : this.m_ImageSpriteState.activeSprite;
					imageTrigger = (!this.isOn) ? this.m_ImageAnimationTriggers.normalTrigger : this.m_ImageAnimationTriggers.activeTrigger;
					break;
				case Selectable.SelectionState.Highlighted:
					newImageColor = (!this.isOn) ? this.m_ImageColors.highlightedColor : this.m_ImageColors.activeHighlightedColor;
					newTextColor = (!this.isOn) ? this.m_TextColors.highlightedColor : this.m_TextColors.activeHighlightedColor;
					newSprite = (!this.isOn) ? this.m_ImageSpriteState.highlightedSprite : this.m_ImageSpriteState.activeHighlightedSprite;
					imageTrigger = (!this.isOn) ? this.m_ImageAnimationTriggers.highlightedTrigger : this.m_ImageAnimationTriggers.activeHighlightedTrigger;
					break;
				case Selectable.SelectionState.Pressed:
					newImageColor = (!this.isOn) ? this.m_ImageColors.pressedColor : this.m_ImageColors.activePressedColor;
					newTextColor = (!this.isOn) ? this.m_TextColors.pressedColor : this.m_TextColors.activePressedColor;
					newSprite = (!this.isOn) ? this.m_ImageSpriteState.pressedSprite : this.m_ImageSpriteState.activePressedSprite;
					imageTrigger = (!this.isOn) ? this.m_ImageAnimationTriggers.pressedTrigger : this.m_ImageAnimationTriggers.activePressedTrigger;
					break;
				case Selectable.SelectionState.Disabled:
					newImageColor = this.m_ImageColors.disabledColor;
					newTextColor = this.m_TextColors.disabledColor;
					newSprite = this.m_ImageSpriteState.disabledSprite;
					imageTrigger = this.m_ImageAnimationTriggers.disabledTrigger;
					break;
			}
			
			// Check if the tab is active in the scene
			if (this.gameObject.activeInHierarchy)
			{
				// Do the image transition
				switch (this.m_ImageTransition)
				{
					case Selectable.Transition.ColorTint:
						this.StartColorTween((this.m_ImageTarget as Graphic), newImageColor * this.m_ImageColors.colorMultiplier, (instant ? 0f : this.m_ImageColors.fadeDuration));
						break;
					case Selectable.Transition.SpriteSwap:
						this.DoSpriteSwap(this.m_ImageTarget, newSprite);
						break;
					case Selectable.Transition.Animation:
						this.TriggerAnimation(this.m_ImageTarget.gameObject, imageTrigger);
						break;
				}
				
				// Do the text transition
				switch (this.m_TextTransition)
				{
					case TextTransition.ColorTint:
						this.StartColorTweenText(newTextColor * this.m_TextColors.colorMultiplier, (instant ? 0f : this.m_TextColors.fadeDuration));
						break;
				}
			}
		}
		
		/// <summary>
		/// Starts a color tween.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="targetColor">Target color.</param>
		/// <param name="duration">Duration.</param>
		private void StartColorTween(Graphic target, Color targetColor, float duration)
		{
			if (target == null)
				return;
			
			if (!Application.isPlaying || duration == 0f)
			{
				target.canvasRenderer.SetColor(targetColor);
			}
			else
			{
				target.CrossFadeColor(targetColor, duration, true, true);
			}
		}
		
		/// <summary>
		/// Starts a color tween.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		/// <param name="duration">Duration.</param>
		private void StartColorTweenText(Color targetColor, float duration)
		{
			if (this.m_TextTarget == null)
				return;
			
			if (!Application.isPlaying || duration == 0f)
			{
				this.m_TextTarget.color = targetColor;
			}
			else
			{
				var colorTween = new ColorTween { duration = duration, startColor = this.m_TextTarget.color, targetColor = targetColor };
				colorTween.AddOnChangedCallback(SetTextColor);
				colorTween.ignoreTimeScale = true;
				
				this.m_ColorTweenRunner.StartTween(colorTween);
			}
		}
		
		/// <summary>
		/// Sets the color of the text.
		/// </summary>
		/// <param name="color">Color.</param>
		private void SetTextColor(Color color)
		{
			if (this.m_TextTarget == null)
				return;
			
			this.m_TextTarget.color = color;
		}
		
		/// <summary>
		/// Does a sprite swap.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="newSprite">New sprite.</param>
		private void DoSpriteSwap(Image target, Sprite newSprite)
		{
			if (target == null)
				return;

            if (!target.overrideSprite.Equals(newSprite))
			    target.overrideSprite = newSprite;
		}
		
		/// <summary>
		/// Triggers the animation.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="triggername">Triggername.</param>
		private void TriggerAnimation(GameObject target, string triggername)
		{
			if (target == null)
				return;
				
			Animator animator = target.GetComponent<Animator>();
			
			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(this.m_ImageAnimationTriggers.normalTrigger);
			animator.ResetTrigger(this.m_ImageAnimationTriggers.pressedTrigger);
			animator.ResetTrigger(this.m_ImageAnimationTriggers.highlightedTrigger);
			animator.ResetTrigger(this.m_ImageAnimationTriggers.activeTrigger);
			animator.ResetTrigger(this.m_ImageAnimationTriggers.activeHighlightedTrigger);
			animator.ResetTrigger(this.m_ImageAnimationTriggers.activePressedTrigger);
			animator.ResetTrigger(this.m_ImageAnimationTriggers.disabledTrigger);
			animator.SetTrigger(triggername);
		}
		
		/// <summary>
		/// Activate the tab.
		/// </summary>
		public void Activate()
		{
			if (!this.isOn)
				this.isOn = true;
		}
	}
}
