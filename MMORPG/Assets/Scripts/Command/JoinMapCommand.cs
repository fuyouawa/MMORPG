using MMORPG.Event;
using MMORPG.Model;
using QFramework;
using UnityEngine;

namespace MMORPG.Command
{
    public class JoinMapCommand : AbstractCommand
    {
        public int MapId;
        public int CharacterId;

        public JoinMapCommand(int mapId, int characterId)
        {
            MapId = mapId;
            CharacterId = characterId;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<IMapModel>();
            Debug.Assert(model.CurrentMapId.Value == -1);
            Tool.Log.Info("Game", $"{CharacterId}加入地图:{MapId}");
            model.CurrentMapId.Value = MapId;

            this.SendEvent(new JoinedMapEvent(MapId, CharacterId));
        }
    }
}
