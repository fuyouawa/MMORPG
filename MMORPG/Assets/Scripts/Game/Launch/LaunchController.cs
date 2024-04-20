using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaunchStatus
{
    InitLog,
    InitTool,
    InitNetwork
}

public class LaunchController : MonoSingleton<LaunchController>, IController
{
    public FSM<LaunchStatus> FSM = new();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        FSM.AddState(LaunchStatus.InitLog, new InitLogState(FSM, this));
        FSM.AddState(LaunchStatus.InitTool, new InitToolState(FSM, this));
        FSM.AddState(LaunchStatus.InitNetwork, new InitNetworkState(FSM, this));

        FSM.StartState(LaunchStatus.InitLog);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
