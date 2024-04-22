using MMORPG;
using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ExitGameState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public ExitGameState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "退出游戏");
        var net = this.GetSystem<INetworkSystem>();
        net.Close();
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}