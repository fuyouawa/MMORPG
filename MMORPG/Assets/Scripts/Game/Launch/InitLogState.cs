using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InitLogState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public InitLogState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "初始化日志");
        Logger.Initialize();
        mFSM.ChangeState(LaunchStatus.InitPlugins);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}