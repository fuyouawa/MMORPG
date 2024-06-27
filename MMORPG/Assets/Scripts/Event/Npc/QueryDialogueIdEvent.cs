using MMORPG.Common.Proto.Inventory;
using MMORPG.Common.Proto.Npc;
using MMORPG.Game;

namespace MMORPG.Event
{
    public class QueryDialogueIdEvent
    {
        public QueryDialogueIdResponse Resp { get; }
        public QueryDialogueIdEvent(QueryDialogueIdResponse resp)
        {
            Resp = resp;
        }
    }
}
