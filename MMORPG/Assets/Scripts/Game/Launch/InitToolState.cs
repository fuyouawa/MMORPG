using QFramework;
using MMORPG.Tool;
using UnityEngine;

namespace MMORPG.Game
{
    public class InitToolState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public InitToolState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Tool.Log.Info("Launch", "初始化工具");
            new GameObject(nameof(UIToolController)).AddComponent<UIToolController>();
            mFSM.ChangeState(LaunchStatus.InitNetwork);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
