
using Common.Network;
using Common.Proto;
using GameServer.Service;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpServer server = new(NetConfig.ServerPort);
            server.Start();
            MessageRouter.Instance.Reigster<UserRegisterRequest>(OnUserRegister);
            Console.ReadLine();
        }

        private static void OnUserRegister(object? sender, UserRegisterRequest e)
        {
            throw new NotImplementedException();
        }
    }
}