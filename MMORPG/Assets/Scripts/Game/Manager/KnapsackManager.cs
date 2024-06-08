using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public class KnapsackManager : MonoBehaviour, IController, ICanSendEvent
    {
        public void SetSlotCount(int slotCount)
        {
            Transform grid = transform.Find("SlotGrid");

            var currentSlotCount = grid.childCount;

            if (currentSlotCount < slotCount)
            {
                var prefab = Resources.Load<UIItemSlot>("UIPrefab/UIPlayerInventoryPanel/UIItemSlot");
                var slotsToAdd = slotCount - currentSlotCount;
                for (int i = 0; i < slotsToAdd; i++)
                {
                    var newSlot = Instantiate(prefab);
                }
            }
            else if (currentSlotCount > slotCount)
            {
                var slotsToRemove = slotCount - currentSlotCount;
                for (int i = currentSlotCount - 1; i >= slotCount; i--)
                {
                    var slotToRemove = grid.GetChild(i);
                    GameObject.Destroy(slotToRemove);
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}
