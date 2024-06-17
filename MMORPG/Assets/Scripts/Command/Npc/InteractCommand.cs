using MMORPG.Common.Proto.Npc;
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
        private int _selectIdx;

        public InteractCommand(int entityId, int selectIndex = 0)
        {
            _entityId = entityId;
            _selectIdx = selectIndex;
        }

        protected override async void OnExecute()
        {
            var net = this.GetSystem<INetworkSystem>();
            net.SendToServer(new InteractRequest()
            {
                EntityId = _entityId,
                SelectIdx = _selectIdx,
            });
            var response = await net.ReceiveAsync<InteractResponse>();
            this.SendEvent<InteractEvent>(new(response) {});
        }
    }
}
