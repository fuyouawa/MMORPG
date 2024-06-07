using QFramework;
using MMORPG.Tool;
using Serilog;

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
            Log.Information("程序退出");
            this.SendEvent(new ApplicationQuitEvent());
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
