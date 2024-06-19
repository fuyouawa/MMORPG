using MMORPG.Common.Proto.Monster;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public class MoveState : AbstractState<ActorState, MonsterBrain>
    {
        public MoveState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
        {

        }

        protected override void OnEnter()
        {
            mTarget.Animator.SetBool("Walking", true);
        }

        protected override void OnExit()
        {
            mTarget.Animator.SetBool("Walking", false);
        }

        protected override void OnUpdate()
        {
        }
    }
}
