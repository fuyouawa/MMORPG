using Common.Network;
using GameServer.Network;
using Network;
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

            //MessageRouter.Instance.On<Vector3>(ABC);

            Console.ReadKey();
        }

        //void ABC<Vector3>(NetConnection sender, Vector3 msg)
        //{

        //}

    }
}
