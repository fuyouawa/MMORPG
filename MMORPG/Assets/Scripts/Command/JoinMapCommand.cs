using MMORPG.Event;
using MMORPG.Game;
using MMORPG.Model;
using QFramework;
using Serilog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMORPG.Command
{
    public class JoinMapCommand : AbstractCommand
    {
        public int MapId;
        public long CharacterId;

        public JoinMapCommand(int mapId, long characterId)
        {
            MapId = mapId;
            CharacterId = characterId;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<IMapModel>();
            Debug.Assert(model.CurrentMapId.Value == -1);
            Log.Information($"{CharacterId}加入地图:{MapId}");
            model.CurrentMapId.Value = MapId;

            this.SendEvent(new JoinMapEvent(MapId, CharacterId));
        }
    }
}
