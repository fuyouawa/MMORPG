using Common.Network;
using GameServer.Network;
using Proto;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetService netService = new NetService();
            netService.Init(32510);
            netService.Start();

            MessageRouter.Instance.On<UserLoginRequest>(OnUserLoginRequest);
            MessageRouter.Instance.Start(4);
            

            Console.ReadKey();
        }

        private static void OnUserLoginRequest(Connection sender, UserLoginRequest msg)
        {
            Console.WriteLine("用户登录请求：{0}, {1}", msg.Username, msg.Password);
        }

        //void ABC<Vector3>(NetConnection sender, Vector3 msg)
        //{

        //}

    }
}
