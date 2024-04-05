using Common.Network;
using Common.Proto.Player;
using Common.Proto.User;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class PlayerService : ServiceBase<PlayerService>
    {
        public void OnHandle(object? sender, PlayerEnterRequest request)
        {
            Global.Logger.Info($"用户进入游戏");
        }

    }
}
