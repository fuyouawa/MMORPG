using QFramework;
using MMORPG.Tool;

namespace MMORPG.Game
{
    public class InitLogState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public InitLogState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Tool.Log.Info("Launch", "初始化日志");
            Tool.Log.Initialize();
            mFSM.ChangeState(LaunchStatus.InitPlugins);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
