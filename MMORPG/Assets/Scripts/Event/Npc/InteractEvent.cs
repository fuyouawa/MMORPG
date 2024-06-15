using MMORPG.Common.Proto.Inventory;
using MMORPG.Common.Proto.Npc;
using MMORPG.Game;

namespace MMORPG.Event
{
    public class InteractEvent
    {
        public InteractResponse Resp { get; }
        public InteractEvent(InteractResponse resp)
        {
            Resp = resp;
        }
    }
}
