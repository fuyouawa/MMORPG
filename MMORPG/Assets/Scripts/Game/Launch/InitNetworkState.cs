using QFramework;
using MMORPG.System;


namespace MMORPG.Game
{
    public class InitNetworkState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public InitNetworkState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override async void OnEnter()
        {
            Tool.Log.Info("Launch", "初始化网络");
            var net = this.GetSystem<INetworkSystem>();
            await net.ConnectAsync();
            net.StartAsync();
            mFSM.ChangeState(LaunchStatus.WaitForJoinMap);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
