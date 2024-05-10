using QFramework;
using MMORPG.Tool;

namespace MMORPG.Game
{
    public class ApplicationQuitEvent
    {
    }

    public class ApplicationQuitState : AbstractState<LaunchStatus, LaunchController>, IController, ICanSendEvent
    {
        public ApplicationQuitState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Tool.Log.Info("Launch", "程序退出");
            this.SendEvent(new ApplicationQuitEvent());
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
