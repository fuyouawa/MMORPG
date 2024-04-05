using Common.Network;
using Common.Tool;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class GameClient : Singleton<GameClient>
{
    private Connection _connection;

    public Connection Connection => _connection;

    public async Task ConnectAsync() {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
        Debug.Log("连接到服务器");
        _connection = new Connection(socket);
        await _connection.StartAsync();
    }
}
