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
    /// 用户管理器
    /// 负责管理所有已登录用户
    /// 线程安全
    /// </summary>
    public class UserManager : Singleton<UserManager>
    {
        private Dictionary<string, User> _userDict = new();

        UserManager() { }

        public User NewUser(NetChannel channel, string username, int playerId)
        {
            var player = new User(channel, username, playerId);
            lock (_userDict)
            {
                _userDict.Add(username, player);
            }
            return player;
        }

        public User? GetPlayerByName(string username)
        {
            lock (_userDict)
            {
                _userDict.TryGetValue(username, out var player);
                return player;
            }
        }

        public void RemovePlayer(User player)
        {
            lock (_userDict)
            {
                _userDict.Remove(player.Username);
            }
        }
    }
}
