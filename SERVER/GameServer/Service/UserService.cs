using Common.Network;
using Common.Proto.User;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class UserService : ServiceBase<UserService>
    {
        public void OnHandle(Connection? sender, UserLoginRequest request)
        {
            UserLoginResponse a;
            Global.Logger.Info($"用户登录请求: Username={request.Username}, Password={request.Password}");
            sender.SendAsync(new UserLoginResponse() { Status = LoginStatus.Ok, Message = "登录成功" });
        }

    }
}
