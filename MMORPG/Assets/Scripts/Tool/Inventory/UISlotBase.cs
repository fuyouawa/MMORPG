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
	public class UISlotBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 槽在容器中的Id(也就是Index)
        /// </summary>
        public int SlotId;

        public bool TooltipEnabled = true;
        public float TooltipDelay = 0.5f;
        private bool _isPointerInside;
        private bool _isPointerDown;
        private bool _isTooltipShown;
        private Coroutine _tooltipDelayedShowCoroutine;
        public UIInventoryBase Inventory { get; protected set; }


        public virtual void Setup(UIInventoryBase inventory, int slotId)
        {
            Inventory = inventory;
            SlotId = slotId;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerInside = true;

            if (enabled && TooltipEnabled)
            {
                if (TooltipDelay > 0f)
                {
                    _tooltipDelayedShowCoroutine = StartCoroutine(TooltipDelayedShow());
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
            StopCoroutine(_tooltipDelayedShowCoroutine);

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
