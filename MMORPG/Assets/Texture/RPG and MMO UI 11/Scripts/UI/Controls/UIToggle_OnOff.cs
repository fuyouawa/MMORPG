using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DuloGames.UI
{
	[ExecuteInEditMode, RequireComponent(typeof(UnityEngine.UI.Toggle)), AddComponentMenu("UI/Toggle OnOff", 58)]
	public class UIToggle_OnOff : MonoBehaviour, IEventSystemHandler {
		
        public enum Transition
        {
            SpriteSwap,
            Reposition
        }

        #pragma warning disable 0649
		[SerializeField] private Image m_Target;
        [SerializeField] private Transition m_Transition = Transition.SpriteSwap;
        [SerializeField] private Sprite m_ActiveSprite;
        [SerializeField] private Vector2 m_InactivePosition = Vector2.zero;
        [SerializeField] private Vector2 m_ActivePosition = Vector2.zero;
		#pragma warning restore 0649

		public Toggle toggle {
			get { return this.gameObject.GetComponent<Toggle>(); }
		}
		
		protected void OnEnable()
		{
			this.toggle.onValueChanged.AddListener(OnValueChanged);
			this.OnValueChanged(this.toggle.isOn);
		}
		
		protected void OnDisable()
		{
			this.toggle.onValueChanged.RemoveListener(OnValueChanged);
		}

		public void OnValueChanged(bool state)
		{
			if (this.m_Target == null || !this.isActiveAndEnabled)
				return;

            // Do the transition
            if (this.m_Transition == Transition.SpriteSwap)
            {
                this.m_Target.overrideSprite = (state) ? this.m_ActiveSprite : null;
            }
            else if (this.m_Transition == Transition.Reposition)
            {
                this.m_Target.rectTransform.anchoredPosition = (state) ? this.m_ActivePosition : this.m_InactivePosition;
            }
		}
	}
}
