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
    InitMap,
    ExitMap,
    ApplicationQuit
}

public class LaunchController : MonoBehaviour, IController
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
        FSM.AddState(LaunchStatus.InitTool, new InitToolState(FSM, this));
        FSM.AddState(LaunchStatus.InitNetwork, new InitNetworkState(FSM, this));
        FSM.AddState(LaunchStatus.WaitForJoinMap, new WaitForJoinMapState(FSM, this));
        FSM.AddState(LaunchStatus.InitMap, new InitMapState(FSM, this));
        FSM.AddState(LaunchStatus.ExitMap, new ExitMapState(FSM, this));
        FSM.AddState(LaunchStatus.ApplicationQuit, new ApplicationQuitState(FSM, this));

        FSM.StartState(LaunchStatus.InitLog);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

    protected void OnApplicationQuit()
    {
        FSM.ChangeState(LaunchStatus.ApplicationQuit);
        FSM.Clear();
    }
}
