using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaunchStatus
{
    InitLog,
    InitPlugins,
    InitTool,
    InitNetwork,
    WaitForJoinMap,
    InitMap
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
        FSM.AddState(LaunchStatus.InitPlugins, new InitPluginsState(FSM, this));
        FSM.AddState(LaunchStatus.InitTool, new InitToolState(FSM, this));
        FSM.AddState(LaunchStatus.InitNetwork, new InitNetworkState(FSM, this));
        FSM.AddState(LaunchStatus.WaitForJoinMap, new WaitForEnterGameState(FSM, this));
        FSM.AddState(LaunchStatus.InitMap, new InitMapState(FSM, this));

        FSM.StartState(LaunchStatus.InitLog);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
