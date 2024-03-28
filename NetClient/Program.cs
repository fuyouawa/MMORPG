using Common.Network;
using Google.Protobuf;
using System.Net;
using System.Net.Sockets;

static void SendMessage(Socket socket, byte[] body)
{
    byte[] lenBytes = BitConverter.GetBytes(body.Length);
    socket.Send(lenBytes);
    socket.Send(body);
}

var host = "127.0.0.1";
var port = 32510;

IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Connect(ipe);

Console.WriteLine("成功连接到服务器");

Connection connection = new(socket, null, null);
connection.Request.UserLogin = new();
connection.Request.UserLogin.Username = "admin";
connection.Request.UserLogin.Password = "123456";
connection.Send();

Console.ReadLine();