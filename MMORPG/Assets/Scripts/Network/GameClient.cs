using Common.Network;
using Common.Tool;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class GameClient : Singleton<GameClient>
{
    private Session _session;

    public Session Session => _session;

    public async Task ConnectAsync() {
        while (true)
        {
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
                Debug.Log("连接到服务器");
                _session = new Session(socket);
                await _session.StartAsync();
                break;
            }
            catch (Exception e)
            {
                await MessageBox.ShowInfoAsync($"服务器连接失败!\n原因:{e.Message}", buttonText:"重新连接");
                continue;
            }
        }
    }
    public bool Connected()
    {
        return _session != null;
    }
}
