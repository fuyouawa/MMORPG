using PimDeWitte.UnityMainThreadDispatcher;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InitToolState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public InitToolState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        new GameObject(nameof(UIToolController)).AddComponent<UIToolController>();
        new GameObject(nameof(UnityMainThreadDispatcher)).AddComponent<UnityMainThreadDispatcher>();
        mFSM.ChangeState(LaunchStatus.InitNetwork);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}