using Common.Network;
using Common.Proto.Entity;
using Common.Proto.Player;
using Common.Proto.Space;
using MMORPG;
using MoonSharp.VsCodeDebugger.SDK;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Tool;
using UnityEngine;

public class NetworkController : MonoBehaviour, IController
{
    private void Awake()
    {
        this.RegisterEvent<InitNetworkEvent>(OnInitNetwork).UnRegisterWhenGameObjectDestroyed(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private async void OnInitNetwork(InitNetworkEvent e)
    {
        var net = this.GetSystem<INetworkSystem>();
        net.RegisterEmergencyReceive<HeartBeatResponse>(resp =>
        {
            //TODO ÐÄÌø°ü
        });
        await net.ConnectAsync();
        await net.StartAsync();
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
