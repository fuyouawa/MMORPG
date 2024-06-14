
using MMORPG.Common.Proto.Player;
using MMORPG.Event;
using MMORPG.Model;
using MMORPG.System;
using QFramework;
using UnityEngine.SceneManagement;

namespace MMORPG.Command
{
    public class InteractCommand : AbstractCommand
    {
        private int _entityId;

        public InteractCommand(int entityId)
        {
            _entityId = entityId;
        }

        protected override async void OnExecute()
        {
            var net = this.GetSystem<INetworkSystem>();
            net.SendToServer(new InteractRequest()
            {
                EntityId = _entityId,
            });
            var response = await net.ReceiveAsync<InteractResponse>();
            //this.SendEvent<LoadKnapsackEvent>(new(response.KnapsackInfo){});
        }
    }
}
