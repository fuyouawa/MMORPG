using MMORPG.Event;
using MMORPG.Model;
using QFramework;
using UnityEngine;

namespace MMORPG.Command
{
    public class JoinMapCommand : AbstractCommand
    {
        public int MapId;

        public JoinMapCommand(int mapId)
        {
            MapId = mapId;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<IMapModel>();
            Debug.Assert(model.CurrentMapId.Value == -1);
            Tool.Log.Info("Game", $"加入地图:{MapId}");
            model.CurrentMapId.Value = MapId;

            this.SendEvent(new JoinedMapEvent(MapId));
        }
    }
}
