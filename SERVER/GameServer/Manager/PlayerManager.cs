using Common.Tool;
using GameServer.Network;
using GameServer.Tool;
using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    /// <summary>
    /// 玩家管理器
    /// 负责管理所有已登录玩家
    /// 线程安全
    /// </summary>
    public class PlayerManager : Singleton<PlayerManager>
    {
        private Dictionary<string, Player> _playerDict = new();

        PlayerManager() { }

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
                _playerDict.TryGetValue(username, out var player);
                return player;
            }
        }

        public void RemovePlayer(Player player)
        {
            lock (_playerDict)
            {
                _playerDict.Remove(player.Username);
            }
        }
    }
}
