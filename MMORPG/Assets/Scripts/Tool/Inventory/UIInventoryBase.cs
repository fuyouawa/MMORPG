using QFramework;
using UnityEngine;

 namespace MMORPG.UI
{
    public abstract class UIInventoryBase : MonoBehaviour
    {
        /// <summary>
        /// 当前选中的槽
        /// </summary>
        public UISlotBase CurrentSelectionSlot { get; protected set; }
        /// <summary>
        /// 上一次选中的槽
        /// </summary>
        public UISlotBase PreviousSelectionSlot { get; protected set; }

        /// <summary>
        /// 管理槽的组
        /// </summary>
        public abstract RectTransform GroupSlots { get; }

        /// <summary>
        /// 槽资源的名称(使用QFramework标记后的)
        /// </summary>
        protected abstract string SlotAssetName { get; }
        protected ResLoader ResLoader { get; } = ResLoader.Allocate();

        /// <summary>
        /// 设置槽的数量
        /// </summary>
        /// <param name="slotCount"></param>
        public virtual void SetSlotCount(int slotCount)
        {
            var currentSlotCount = GroupSlots.childCount;

            if (currentSlotCount < slotCount)
            {
                var prefab = ResLoader.LoadSync<UISlotBase>(SlotAssetName);

                for (int i = currentSlotCount; i < slotCount; i++)
                {
                    var newSlot = Instantiate(prefab, GroupSlots);
                    newSlot.Setup(this, i + 1);
                    newSlot.Toggle.onValueChanged.AddListener(toggle => OnSlotToggle(newSlot, toggle));
                    OnInstantiatedSlot(newSlot);
                }
            }
            else if (currentSlotCount > slotCount)
            {
                for (int i = currentSlotCount - 1; i >= slotCount; i--)
                {
                    var slot = GroupSlots.GetChild(i);
                    OnDestroySlot(slot.GetComponent<UISlotBase>());
                    Destroy(slot.gameObject);
                }
            }
        }

        /// <summary>
        /// 当有新槽实例化时
        /// </summary>
        /// <param name="slot"></param>
        protected virtual void OnInstantiatedSlot(UISlotBase slot) {}
        /// <summary>
        /// 当有槽要被销毁前
        /// </summary>
        /// <param name="slot"></param>
        protected virtual void OnDestroySlot(UISlotBase slot) {}

        /// <summary>
        /// 用于计算当前选中的槽和上一次选中的槽
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="toggle"></param>
        protected virtual void OnSlotToggle(UISlotBase slot, bool toggle)
        {
            OnSlotClick(slot);

            if (CurrentSelectionSlot != null)
            {
                CurrentSelectionSlot.Toggle.SetIsOnWithoutNotify(CurrentSelectionSlot == slot);
            }

            if (toggle)
            {
                if (CurrentSelectionSlot != slot)
                {
                    PreviousSelectionSlot = CurrentSelectionSlot;
                    CurrentSelectionSlot = slot;

                    OnSlotSelectionChanged(PreviousSelectionSlot, CurrentSelectionSlot);
                }
            }
        }

        /// <summary>
        /// 当槽选中更改时
        /// </summary>
        /// <param name="prevSlot"></param>
        /// <param name="currentSlot"></param>
        protected virtual void OnSlotSelectionChanged(UISlotBase prevSlot, UISlotBase currentSlot) {}

        /// <summary>
        /// 当槽被点击时
        /// </summary>
        /// <param name="slot"></param>
        protected virtual void OnSlotClick(UISlotBase slot) { }

        protected virtual void OnDestroy()
        {
            ResLoader.Recycle2Cache();
        }
    }
}
