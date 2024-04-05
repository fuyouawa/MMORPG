using Common.Network;
using Common.Proto.User;
using Common.Tool;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class NetService : ServiceBase<NetService>
    {
        public void OnHandle(object? sender, UserLoginRequest request)
        {
            Global.Logger.Info($"[Service] 用户注册请求: Username={request.Username}, Password={request.Password}");
        }
    }
}
