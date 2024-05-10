using QFramework;
using MMORPG.System;

namespace MMORPG.Game
{
    public class ExitMapState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public ExitMapState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Tool.Log.Info("Launch", "退出地图");
            var mapMgr = this.GetSystem<IMapManagerSystem>();
            UnityEngine.Debug.Assert(mapMgr.IsInMap);
            mapMgr.ExitMap();

            mFSM.ChangeState(LaunchStatus.WaitForJoinMap);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
