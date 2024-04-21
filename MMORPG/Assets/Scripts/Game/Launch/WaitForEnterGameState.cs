using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WaitForEnterGameState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public WaitForEnterGameState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
        this.RegisterEvent<JoinedMapEvent>(OnEnterSpace);
    }

    private void OnEnterSpace(JoinedMapEvent e)
    {
        mFSM.ChangeState(LaunchStatus.InitMap);
    }

    protected override void OnEnter()
    {

    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}