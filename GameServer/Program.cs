
using Common.Network;
using Common.Proto;
using GameServer.Service;
using GameServer.Tool;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpServer server = new(NetConfig.ServerPort);
            server.Start();
            MessageRouter.Instance.Reigster<UserRegisterRequest>(OnUserRegister);
            MessageRouter.Instance.Reigster<UserLoginRequest>(OnUserLogin);
            Console.ReadLine();
        }

        private static void OnUserLogin(object? sender, UserLoginRequest msg)
        {
            Global.Logger.Info($"[Router] 用户登录请求:UserName={msg.Username}, Password={msg.Password}");
        }

        private static void OnUserRegister(object? sender, UserRegisterRequest msg)
        {
            Global.Logger.Info($"[Router] 用户注册请求:UserName={msg.Username}, Password={msg.Password}");
        }
    }
}