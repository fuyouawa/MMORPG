using MMORPG;
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

    protected override async void OnEnter()
    {
        Logger.Info("Launch", "初始化网络");
        var net = this.GetSystem<INetworkSystem>();
        await net.ConnectAsync();
        net.StartAsync();
        mFSM.ChangeState(LaunchStatus.WaitForJoinMap);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}