using System.Collections;
using UnityEngine;
using QFramework;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NotImplementedException = System.NotImplementedException;
using DuloGames.UI;
using Common.Proto.Inventory;

namespace MMORPG.UI
{
    [RequireComponent(typeof(Toggle))]
	public class UISlotBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public bool TooltipEnabled = true;
        public float TooltipDelay = 0.5f;

        private bool _isPointerInside;
        private bool _isPointerDown;
        private bool _isTooltipShown;
        public Toggle Toggle { get; protected set; }
        public UIInventoryBase Inventory { get; protected set; }

        /// <summary>
        /// 槽在容器中的Id(也就是Index)
        /// </summary>
        public int SlotId { get; protected set; }

        public void Setup(UIInventoryBase inventory, int slotId)
        {
            Inventory = inventory;
            SlotId = slotId;
        }

        protected virtual void Awake()
        {
            Toggle = GetComponent<Toggle>();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerInside = true;

            if (enabled && TooltipEnabled)
            {
                if (TooltipDelay > 0f)
                {
                    StartCoroutine(TooltipDelayedShow());
                }
                else
                {
                    InternalShowTooltip();
                }
            }
        }

        protected IEnumerator TooltipDelayedShow()
        {
            yield return new WaitForSeconds(TooltipDelay);
            InternalShowTooltip();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _isPointerInside = false;
            InternalHideTooltip();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;

            InternalHideTooltip();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
        }

        private void InternalShowTooltip()
        {
            if (!_isTooltipShown)
            {
                _isTooltipShown = true;
                OnShowTooltip();
            }
        }

        private void InternalHideTooltip()
        {
            StopCoroutine(TooltipDelayedShow());

            if (_isTooltipShown)
            {
                _isTooltipShown = false;
                OnHideTooltip();
            }
        }

        protected virtual void OnShowTooltip()
        {
        }

        protected virtual void OnHideTooltip()
        {
        }
    }
}
