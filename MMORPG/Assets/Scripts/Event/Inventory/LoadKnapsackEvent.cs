using MMORPG.Common.Proto.Inventory;
using MMORPG.Game;

namespace MMORPG.Event
{
    public class LoadKnapsackEvent
    {
        public InventoryInfo KnapsackInfo { get; }
        public LoadKnapsackEvent(InventoryInfo knapsackInfo)
        {
            KnapsackInfo = knapsackInfo;
        }
    }
}
