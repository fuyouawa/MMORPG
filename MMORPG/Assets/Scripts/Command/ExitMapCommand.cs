using MMORPG.Common.Proto.Map;
using MMORPG.Event;
using MMORPG.Model;
using MMORPG.System;
using QFramework;
using Serilog;
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
            Log.Information($"退出地图:{model.CurrentMapId}");
            model.CurrentMapId.Value = -1;
            var net = this.GetSystem<INetworkSystem>();
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            net.SendToServer(new EntityLeaveRequest()
            {
                EntityId = playerManager.MineEntity.EntityId
            });
            this.SendEvent(new ExitedMapEvent(model.CurrentMapId.Value));
        }
    }
}
