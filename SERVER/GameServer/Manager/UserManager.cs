using Common.Tool;
using GameServer.Network;
using GameServer.Tool;
using GameServer.Model;
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

        public User NewUser(NetChannel channel, string username, int userId)
        {
            var user = new User(channel, username, userId);
            lock (_userDict)
            {
                _userDict.Add(username, user);
            }
            return user;
        }

        public User? GetUserByName(string username)
        {
            lock (_userDict)
            {
                _userDict.TryGetValue(username, out var user);
                return user;
            }
        }

        public void RemoveUser(User user)
        {
            lock (_userDict)
            {
                _userDict.Remove(user.Username);
            }
        }
    }
}
