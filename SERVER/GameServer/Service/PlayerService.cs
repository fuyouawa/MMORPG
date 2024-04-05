using Common.Network;
using Common.Proto.Player;
using GameServer.Network;
using GameServer.Tool;

namespace GameServer.Service
{
    public class PlayerService : ServiceBase<PlayerService>
    {
        public void OnHandle(Channel? sender, PlayerEnterRequest request)
        {
            Global.Logger.Info($"用户进入游戏");
        }

    }
}
