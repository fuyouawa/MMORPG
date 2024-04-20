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
        Logger.Initialize();
        mFSM.ChangeState(LaunchStatus.InitTool);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}