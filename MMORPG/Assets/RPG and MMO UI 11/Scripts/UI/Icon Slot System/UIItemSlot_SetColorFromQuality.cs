using System;
using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    [RequireComponent(typeof(UIItemSlot))]
    public class UIItemSlot_SetColorFromQuality : MonoBehaviour
    {
        [Serializable]
        public struct QualityToColor
        {
            public UIItemQuality quality;
            public Color color;
        }

        #pragma warning disable 0649
        [SerializeField] private Graphic m_Target;
        #pragma warning restore 0649

        private UIItemSlot m_Slot;

        protected void Awake()
        {
            this.m_Slot = this.gameObject.GetComponent<UIItemSlot>();
        }

        protected void OnEnable()
        {
            this.m_Slot.onAssign.AddListener(OnSlotAssign);
            this.m_Slot.onUnassign.AddListener(OnSlotUnassign);
        }

        protected void OnDisable()
        {
            this.m_Slot.onAssign.RemoveListener(OnSlotAssign);
            this.m_Slot.onUnassign.RemoveListener(OnSlotUnassign);
        }

        public void OnSlotAssign(UIItemSlot slot)
        {
            if (this.m_Target == null && slot.GetItemInfo() != null)
                return;
            
            this.m_Target.color = UIItemQualityColor.GetColor(slot.GetItemInfo().Quality);
        }

        public void OnSlotUnassign(UIItemSlot slot)
        {
            if (this.m_Target == null)
                return;

            this.m_Target.color = Color.clear;
        }
    }
}
