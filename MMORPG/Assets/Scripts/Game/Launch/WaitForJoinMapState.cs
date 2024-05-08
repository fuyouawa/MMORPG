using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WaitForJoinMapState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public WaitForJoinMapState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
        this.RegisterEventInUnityThread<JoinedMapEvent>(OnEnterSpace);
    }

    private void OnEnterSpace(JoinedMapEvent e)
    {
        mFSM.ChangeState(LaunchStatus.InitMap);
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "开始等待加入地图");
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
