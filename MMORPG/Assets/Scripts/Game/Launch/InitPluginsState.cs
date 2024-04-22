using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InitPluginsState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public InitPluginsState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "初始化插件");
        ResKit.Init();
        mFSM.ChangeState(LaunchStatus.InitTool);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}