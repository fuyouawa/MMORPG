using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace DuloGames.UI
{
	public class UISelectField_List : Selectable {
		
		public enum State
		{
			Opened,
			Closed
		}
		
		[Serializable] public class AnimationFinishEvent : UnityEvent<UISelectField_List.State> { }
		public AnimationFinishEvent onAnimationFinish = new AnimationFinishEvent();
		public UnityEvent onDimensionsChange = new UnityEvent();
		
		private string m_AnimationOpenTrigger = string.Empty;
		private string m_AnimationCloseTrigger = string.Empty;
		private State m_State = State.Closed;
		
		protected override void Start()
		{
			base.Start();
			this.transition = Transition.None;
			
			Navigation nav = new Navigation();
			nav.mode = Navigation.Mode.None;
			this.navigation = nav;
		}
		
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			
			if (this.onDimensionsChange != null)
				this.onDimensionsChange.Invoke();
		}
		
		/// <summary>
		/// Sets the animation triggers (Used to detect animation finish).
		/// </summary>
		/// <param name="openTrigger">Open trigger.</param>
		/// <param name="closeTrigger">Close trigger.</param>
		public void SetTriggers(string openTrigger, string closeTrigger)
		{
			this.m_AnimationOpenTrigger = openTrigger;
			this.m_AnimationCloseTrigger = closeTrigger;
		}
		
		protected void Update()
		{
			if (this.animator != null && !string.IsNullOrEmpty(this.m_AnimationOpenTrigger) && !string.IsNullOrEmpty(this.m_AnimationCloseTrigger))
			{
				AnimatorStateInfo state = this.animator.GetCurrentAnimatorStateInfo(0);
				
				// Check which is the current state
				if (state.IsName(this.m_AnimationOpenTrigger) && this.m_State == State.Closed)
				{
					if (state.normalizedTime >= state.length)
					{
						// Flag as opened
						this.m_State = State.Opened;
						
						// Invoke the animation finish event
						if (this.onAnimationFinish != null)
							onAnimationFinish.Invoke(this.m_State);
					}
				}
				else if (state.IsName(this.m_AnimationCloseTrigger) && this.m_State == State.Opened)
				{
					if (state.normalizedTime >= state.length)
					{
						// Flag as closed
						this.m_State = State.Closed;
						
						// Invoke the animation finish event
						if (this.onAnimationFinish != null)
							onAnimationFinish.Invoke(this.m_State);
					}
				}
			}
		}
		
		/// <summary>
		/// Determines whether list is pressed.
		/// </summary>
		/// <returns><c>true</c> if the list is pressed by the specified eventData; otherwise, <c>false</c>.</returns>
		new public bool IsPressed()
		{
			return base.IsPressed();
		}
		
		/// <summary>
		/// Determines whether list is highlighted.
		/// </summary>
		/// <returns><c>true</c> if this instance is highlighted the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		public bool IsHighlighted(BaseEventData eventData)
		{
			return base.IsHighlighted();
		}
	}
}
