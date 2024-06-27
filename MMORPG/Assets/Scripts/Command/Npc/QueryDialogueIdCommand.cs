using MMORPG.Common.Proto.Npc;
using MMORPG.Event;
using MMORPG.Model;
using MMORPG.System;
using QFramework;
using UnityEngine.SceneManagement;

namespace MMORPG.Command
{
    public class QueryDialogueIdCommand : AbstractCommand
    {
        private int _entityId;

        public QueryDialogueIdCommand(int entityId)
        {
            _entityId = entityId;
        }

        protected override async void OnExecute()
        {
            var net = this.GetSystem<INetworkSystem>();
            net.SendToServer(new QueryDialogueIdRequest()
            {
                EntityId = _entityId,
            });
            var response = await net.ReceiveAsync<QueryDialogueIdResponse>();
            this.SendEvent<QueryDialogueIdEvent>(new(response) {});
        }
    }
}
