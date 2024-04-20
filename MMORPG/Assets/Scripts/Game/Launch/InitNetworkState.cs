using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InitNetworkState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public InitNetworkState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {

    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}