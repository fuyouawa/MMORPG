using MMORPG;
using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ExitMapState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public ExitMapState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "退出地图");
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}