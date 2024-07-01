using QFramework;
using UnityEngine;

 namespace MMORPG.UI
{
    public abstract class UIInventoryBase : MonoBehaviour
    {
        /// <summary>
        /// 管理槽的组
        /// </summary>
        public abstract RectTransform GroupSlots { get; }

        /// <summary>
        /// 槽资源的名称(使用QFramework标记后的)
        /// </summary>
        protected abstract string SlotAssetName { get; }

        public virtual UISlotBase[] GetSlots()
        {
            return GroupSlots.GetComponentsInChildren<UISlotBase>();
        }

        /// <summary>
        /// 设置槽的数量
        /// </summary>
        /// <param name="slotCount"></param>
        public virtual void SetSlotCount(int slotCount)
        {
            var currentSlotCount = GroupSlots.childCount;

            if (currentSlotCount < slotCount)
            {
                var prefab = Resources.Load<UISlotBase>(SlotAssetName);

                for (int i = currentSlotCount; i < slotCount; i++)
                {
                    var newSlot = Instantiate(prefab, GroupSlots);
                    newSlot.Setup(this, i);
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
    }
}
