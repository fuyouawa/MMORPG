using QFramework;
using Serilog;
using Serilog.Sinks.Unity3D;

namespace MMORPG.Game
{
    public class InitLogState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public InitLogState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
                .WriteTo.Unity3D()
                .CreateLogger();
            mFSM.ChangeState(LaunchStatus.InitPlugins);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
