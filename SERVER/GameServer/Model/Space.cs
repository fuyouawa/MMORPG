using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Space
    {
        public int SpaceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Music { get; set; }

        private Dictionary<int, Player> _playersSet = new();

        /// <summary>
        /// 角色进入场景
        /// </summary>
        public void PlayerJoin(Player player)
        {
            //_playersSet[player.Character.EntityId] = player;
        }
    }
}
