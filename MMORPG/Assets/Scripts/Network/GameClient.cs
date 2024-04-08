using Common.Network;
using Common.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class GameClient : Singleton<GameClient>
{
    private NetSession _session;

    public NetSession NetSession => _session;

    public async Task ConnectAsync() {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
        _session = new NetSession(socket);
    }

    public async Task StartAsync()
    {
        await _session.StartAsync();
    }

    public bool Connected()
    {
        return _session != null;
    }
}
