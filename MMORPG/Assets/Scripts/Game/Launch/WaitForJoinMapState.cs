using MMORPG.Event;
using QFramework;
using MMORPG.Tool;

namespace MMORPG.Game
{
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
            Tool.Log.Info("Launch", "开始等待加入地图");
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
