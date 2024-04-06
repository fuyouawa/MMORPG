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
        try
        {
            Popup.Instance.Show("正在连接到服务器...");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
            Debug.Log("连接到服务器");
            _session = new Session(socket);
            Popup.Instance.Close();
            await _session.StartAsync();
        }
        catch (Exception e)
        {
            //Debug.Log($"异常类型：{e.GetType()}");
            //Debug.Log($"异常消息：{e.Message}");
            Popup.Instance.Show("连接到服务器失败");
        }
    }
    public bool Connected()
    {
        return _session != null;
    }
}
