using PimDeWitte.UnityMainThreadDispatcher;
using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InitPluginsState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public InitPluginsState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "初始化插件");
        ResKit.Init();
        new GameObject(nameof(UnityMainThreadDispatcher)).AddComponent<UnityMainThreadDispatcher>();
        mFSM.ChangeState(LaunchStatus.InitConfig);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
