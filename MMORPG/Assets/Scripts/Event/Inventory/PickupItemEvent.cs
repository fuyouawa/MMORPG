using MMORPG.Common.Proto.Inventory;
using MMORPG.Game;

namespace MMORPG.Event
{
    public class PickupItemEvent
    {
        public PickupItemResponse Resp { get; }
        public PickupItemEvent(PickupItemResponse resp)
        {
            Resp = resp;
        }
    }
}
