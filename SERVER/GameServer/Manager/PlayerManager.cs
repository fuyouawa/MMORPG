using Common.Tool;
using GameServer.Network;
using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private Dictionary<string, Player> _playerDict = new();

        public Player NewPlayer(NetChannel channel, string username, int playerId)
        {
            var player = new Player(channel, username, playerId);
            lock (_playerDict)
            {
                _playerDict.Add(username, player);
            }
            return player;
        }

        public Player? GetPlayerByName(string username)
        {
            lock (_playerDict)
            {
                return _playerDict.GetValueOrDefault(username, null);
            }
        }

        public void RemovePlayer(Player player)
        {
            _playerDict.Remove(player.Username);
        }
    }
}
