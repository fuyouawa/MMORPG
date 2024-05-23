using Common.Proto.EventLike.Map;
using MMORPG.Event;
using MMORPG.Model;
using MMORPG.System;
using QFramework;
using UnityEngine;

namespace MMORPG.Command
{
    public class ExitMapCommand : AbstractCommand
    {
        public ExitMapCommand()
        {
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<IMapModel>();

            Debug.Assert(model.CurrentMapId.Value != -1);
            Tool.Log.Info("Game", $"退出地图:{model.CurrentMapId}");
            model.CurrentMapId.Value = -1;
            var net = this.GetSystem<INetworkSystem>();
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            net.SendToServer(new EntityLeaveRequest()
            {
                EntityId = playerManager.MineId
            });
            this.SendEvent(new ExitedMapEvent(model.CurrentMapId.Value));
        }
    }
}
