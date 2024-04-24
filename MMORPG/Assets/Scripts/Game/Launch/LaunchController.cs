using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaunchStatus
{
    InitLog,
    InitPlugins,
    InitConfig,
    InitTool,
    InitNetwork,
    WaitForJoinMap,
    InitMap,
    ExitGame
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
        Logger.Info("Launch", "开始生命周期");
        FSM.AddState(LaunchStatus.InitLog, new InitLogState(FSM, this));
        FSM.AddState(LaunchStatus.InitPlugins, new InitPluginsState(FSM, this));
        FSM.AddState(LaunchStatus.InitConfig, new InitConfigState(FSM, this));
        FSM.AddState(LaunchStatus.InitTool, new InitToolState(FSM, this));
        FSM.AddState(LaunchStatus.InitNetwork, new InitNetworkState(FSM, this));
        FSM.AddState(LaunchStatus.WaitForJoinMap, new WaitForJoinMapState(FSM, this));
        FSM.AddState(LaunchStatus.InitMap, new InitMapState(FSM, this));
        FSM.AddState(LaunchStatus.ExitGame, new ExitGameState(FSM, this));

        FSM.StartState(LaunchStatus.InitLog);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        FSM.ChangeState(LaunchStatus.ExitGame);
    }
}
