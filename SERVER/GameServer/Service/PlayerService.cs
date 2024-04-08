using Common.Network;
using Common.Proto.Base;
using Common.Proto.Player;
using GameServer.Model;
using GameServer.Network;
using GameServer.Tool;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Channels;

namespace GameServer.Service
{
    public class PlayerService : ServiceBase<PlayerService>
    {
        private Dictionary<string, Player> _playerSet = new();

        public void OnChannelClosed(NetChannel sender)
        {
            if (sender.Player == null)
                return;
            lock (_playerSet)
            {
                _playerSet.Remove(sender.Player.Character.Name);
            }
        }

        public void OnHandle(NetChannel sender, LoginRequest request)
        {
            //if (_userSet.ContainsKey(request.Username))
            //{

            //}
            LoginResponse a;
            Global.Logger.Info($"玩家登录请求: Username={request.Username}, Password={request.Password}");

            var player = new Player(sender, new());
            player.Character.Name = request.Username;
            lock (_playerSet)
            {
                _playerSet[request.Username] = player;
            }
            sender.Player = player;

            sender.SendAsync(new LoginResponse() { Status = Status.Ok, Message = "登录成功" }, null);
        }

        public void OnHandle(NetChannel sender, EnterGameRequest request)
        {
            Global.Logger.Info($"玩家进入游戏");
        }

        public void OnHandle(NetChannel sender, HeartBeatRequest request)
        {
            Global.Logger.Debug($"玩家发送心跳请求");
            sender.SendAsync(new HeartBeatResponse() { }, null);
        }

        public void OnConnect(NetChannel sender)
        {
        }
    }
}
