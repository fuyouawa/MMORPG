using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InitNetworkState : AbstractState<LaunchStatus, LaunchController>, IController
{
    private INetworkSystem _network;

    public InitNetworkState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
        _network = this.GetSystem<INetworkSystem>();
    }

    protected override async void OnEnter()
    {
        await _network.ConnectAsync();
        _network.StartAsync();
        mFSM.ChangeState(LaunchStatus.WaitForJoinMap);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}