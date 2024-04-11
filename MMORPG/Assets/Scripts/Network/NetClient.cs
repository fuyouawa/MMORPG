using Common.Network;
using Common.Proto.Player;
using Common.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public static class NetClient
{
    public static NetSession Session { get; private set; }

    public static async Task StartSessionAsync(Socket socket) {
        Session = new NetSession(socket);

        var heartTask = Task.Run(async () =>
        {
            while (true)
            {
                Session.Send(new HeartBeatRequest());
                await Session.ReceiveAsync<HeartBeatResponse>();
                await Task.Delay(1000);
            }
        });
        await Session.StartAsync();
        await heartTask;
    }
}
