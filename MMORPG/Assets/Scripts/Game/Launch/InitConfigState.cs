using PimDeWitte.UnityMainThreadDispatcher;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InitConfigState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public InitConfigState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "初始化配置");
        var resLoader = ResLoader.Allocate();
        Config.GameConfig = resLoader.LoadSync<GameConfigObject>("GameConfig");
        resLoader.Recycle2Cache();
        mFSM.ChangeState(LaunchStatus.InitTool);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
