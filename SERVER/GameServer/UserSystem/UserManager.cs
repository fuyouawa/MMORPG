using MMORPG.Common.Tool;
using GameServer.Network;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Db;

namespace GameServer.UserSystem
{
    /// <summary>
    /// 用户管理器
    /// 负责管理所有已登录用户
    /// </summary>
    public class UserManager : Singleton<UserManager>
    {
        private Dictionary<string, User> _userDict = new();

        UserManager() { }

        public void Start() { }

        public void Update()
        {
        }

        public User NewUser(NetChannel channel, DbUser dbUser)
        {
            var user = new User(channel, dbUser);
            _userDict.Add(dbUser.Username, user);
            
            user.Start();
            return user;
        }

        public User? GetUserByName(string username)
        {
            _userDict.TryGetValue(username, out var user);
            return user;
        }

        public void RemoveUser(string username)
        {
            _userDict.Remove(username);
        }
    }
}
