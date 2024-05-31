using MMORPG.Command;
using QFramework;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
    public class PlayingState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public PlayingState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
            this.SendCommand(new ExitMapCommand());
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
