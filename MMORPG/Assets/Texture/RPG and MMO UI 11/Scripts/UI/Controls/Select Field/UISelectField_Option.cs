using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace DuloGames.UI
{
	public class UISelectField_Option : Toggle, IPointerClickHandler {
		
		[Serializable] public class SelectOptionEvent : UnityEvent<string> { }
		[Serializable] public class PointerUpEvent : UnityEvent<BaseEventData> { }
		
		/// <summary>
		/// The select field referrence.
		/// </summary>
		public UISelectField selectField;
		
		/// <summary>
		/// The text component referrence.
		/// </summary>
		public Text textComponent;
		
		/// <summary>
		/// The On Select Option event.
		/// </summary>
		public SelectOptionEvent onSelectOption = new SelectOptionEvent();
		
		/// <summary>
		/// The On Pointer Up event.
		/// </summary>
		public PointerUpEvent onPointerUp = new PointerUpEvent();
		
		protected override void Start()
		{
			base.Start();
			
			// Disable navigation
			Navigation nav = new Navigation();
			nav.mode = Navigation.Mode.None;
			this.navigation = nav;
			
			// Set initial transition to none
			this.transition = Transition.None;
			
			// Disable toggle transition
			this.toggleTransition = ToggleTransition.None;
		}
		
		/// <summary>
		/// Initialize the option.
		/// </summary>
		/// <param name="select">Select.</param>
		/// <param name="text">Text.</param>
		public void Initialize(UISelectField select, Text text)
		{
			this.selectField = select;
			this.textComponent = text;
			this.OnEnable();
		}
		
#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			
			if (this.selectField != null)
				this.selectField.optionBackgroundTransColors.fadeDuration = Mathf.Max(this.selectField.optionBackgroundTransColors.fadeDuration, 0f);
		}
#endif
		
		/// <summary>
		/// Determines whether this option is pressed.
		/// </summary>
		/// <returns><c>true</c> if this instance is pressed the specified eventData; otherwise, <c>false</c>.</returns>
		new public bool IsPressed()
		{
			return base.IsPressed();
		}
		
		/// <summary>
		/// Determines whether this option is highlighted.
		/// </summary>
		/// <returns><c>true</c> if this instance is highlighted the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		public bool IsHighlighted(BaseEventData eventData)
		{
			return base.IsHighlighted();
		}
		
		/// <summary>
		/// Raises the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			
			if (this.onPointerUp != null)
				this.onPointerUp.Invoke(eventData);
		}

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            // Transition to the correct state
			this.DoStateTransition(SelectionState.Normal, false);
            
			// Invoke the select event
			if (this.onSelectOption != null && this.textComponent != null)
			{
				this.onSelectOption.Invoke(this.textComponent.text);
			}
        }
        
		/// <summary>
		/// Does the state transition.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			if (!this.enabled || !this.enabled || !this.gameObject.activeInHierarchy || this.selectField == null)
				return;
            
			Color color = this.selectField.optionBackgroundTransColors.normalColor;
			Sprite newSprite = null;
			string triggername = this.selectField.optionBackgroundAnimationTriggers.normalTrigger;
			
			// Check if this is the disabled state before any others
			if (state == Selectable.SelectionState.Disabled)
			{
				color = this.selectField.optionBackgroundTransColors.disabledColor;
				newSprite = this.selectField.optionBackgroundSpriteStates.disabledSprite;
				triggername = this.selectField.optionBackgroundAnimationTriggers.disabledTrigger;
			}
			else
			{
				// Prepare the state values
				switch (state)
				{
					case Selectable.SelectionState.Normal:
						color = 					(this.isOn) ? this.selectField.optionBackgroundTransColors.activeColor : this.selectField.optionBackgroundTransColors.normalColor;
						newSprite = 				(this.isOn) ? this.selectField.optionBackgroundSpriteStates.activeSprite : null;
						triggername = 				(this.isOn) ? this.selectField.optionBackgroundAnimationTriggers.activeTrigger : this.selectField.optionBackgroundAnimationTriggers.normalTrigger;
						break;
					case Selectable.SelectionState.Highlighted:
						color = 					(this.isOn) ? this.selectField.optionBackgroundTransColors.activeHighlightedColor : this.selectField.optionBackgroundTransColors.highlightedColor;
						newSprite = 				(this.isOn) ? this.selectField.optionBackgroundSpriteStates.activeHighlightedSprite : this.selectField.optionBackgroundSpriteStates.highlightedSprite;
						triggername = 				(this.isOn) ? this.selectField.optionBackgroundAnimationTriggers.activeHighlightedTrigger : this.selectField.optionBackgroundAnimationTriggers.highlightedTrigger;
						break;
					case Selectable.SelectionState.Pressed:
						color = 					(this.isOn) ? this.selectField.optionBackgroundTransColors.activePressedColor : this.selectField.optionBackgroundTransColors.pressedColor;
						newSprite = 				(this.isOn) ? this.selectField.optionBackgroundSpriteStates.activePressedSprite : this.selectField.optionBackgroundSpriteStates.pressedSprite;
						triggername = 				(this.isOn) ? this.selectField.optionBackgroundAnimationTriggers.activePressedTrigger : this.selectField.optionBackgroundAnimationTriggers.pressedTrigger;
						break;
				}
			}
			
			// Do the transition
			switch (this.selectField.optionBackgroundTransitionType)
			{
				case Transition.ColorTint:
				this.StartColorTween(color * this.selectField.optionBackgroundTransColors.colorMultiplier, (instant ? 0f : this.selectField.optionBackgroundTransColors.fadeDuration));
					break;
				case Transition.SpriteSwap:
					this.DoSpriteSwap(newSprite);
					break;
				case Transition.Animation:
					this.TriggerAnimation(triggername);
					break;
			}
			
			// Do the transition of the text component
			this.DoTextStateTransition(state, instant);
		}
		
		/// <summary>
		/// Does the text state transition.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void DoTextStateTransition(SelectionState state, bool instant)
		{
			// Make sure we have the select field and text component
			if (this.selectField != null && this.textComponent != null)
			{
				// Cross Fade transition
				// Currently the only supported
				if (this.selectField.optionTextTransitionType == UISelectField.OptionTextTransitionType.CrossFade)
				{
					Color color = this.selectField.optionTextTransitionColors.normalColor;
					
					if (state == SelectionState.Disabled)
					{
						color = this.selectField.optionTextTransitionColors.disabledColor;
					}
					else
					{
						switch (state)
						{
						case SelectionState.Normal:
							color = (this.isOn) ? this.selectField.optionTextTransitionColors.activeColor : this.selectField.optionTextTransitionColors.normalColor;
							break;
						case SelectionState.Highlighted:
							color = (this.isOn) ? this.selectField.optionTextTransitionColors.activeHighlightedColor : this.selectField.optionTextTransitionColors.highlightedColor;
							break;
						case SelectionState.Pressed:
							color = (this.isOn) ? this.selectField.optionTextTransitionColors.activePressedColor : this.selectField.optionTextTransitionColors.pressedColor;
							break;
						}
					}
					
					// Start the tween
					this.textComponent.CrossFadeColor(color * this.selectField.optionTextTransitionColors.colorMultiplier, (instant ? 0f : this.selectField.optionTextTransitionColors.fadeDuration), true, true);
				}
			}
		}
		
		/// <summary>
		/// Starts the color tween.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="duration">Duration.</param>
		private void StartColorTween(Color color, float duration)
		{
			if (this.targetGraphic == null)
				return;
	
			this.targetGraphic.CrossFadeColor(color, duration, true, true);
		}
		
		/// <summary>
		/// Does the sprite swap.
		/// </summary>
		/// <param name="newSprite">New sprite.</param>
		private void DoSpriteSwap(Sprite newSprite)
		{
			Image image = this.targetGraphic as Image;
			
			if (image == null)
				return;
			
			image.overrideSprite = newSprite;
		}
		
		/// <summary>
		/// Triggers the animation.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		private void TriggerAnimation(string trigger)
		{
			if (this.selectField == null || this.animator == null || !this.animator.enabled || !this.animator.isActiveAndEnabled || this.animator.runtimeAnimatorController == null || !this.animator.hasBoundPlayables || string.IsNullOrEmpty(trigger))
				return;
			
			this.animator.ResetTrigger(this.selectField.optionBackgroundAnimationTriggers.normalTrigger);
			this.animator.ResetTrigger(this.selectField.optionBackgroundAnimationTriggers.pressedTrigger);
			this.animator.ResetTrigger(this.selectField.optionBackgroundAnimationTriggers.highlightedTrigger);
			this.animator.ResetTrigger(this.selectField.optionBackgroundAnimationTriggers.activeTrigger);
			this.animator.ResetTrigger(this.selectField.optionBackgroundAnimationTriggers.activeHighlightedTrigger);
			this.animator.ResetTrigger(this.selectField.optionBackgroundAnimationTriggers.activePressedTrigger);
			this.animator.ResetTrigger(this.selectField.optionBackgroundAnimationTriggers.disabledTrigger);
			this.animator.SetTrigger(trigger);
		}
	}
}
