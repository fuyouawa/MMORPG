using MMORPG;
using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ApplicationQuitEvent {}

public class ApplicationQuitState : AbstractState<LaunchStatus, LaunchController>, IController, ICanSendEvent
{
    public ApplicationQuitState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "程序退出");
        //TODO如果还没退出地图, 就发送退出地图
        this.SendEvent(new ApplicationQuitEvent());
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
