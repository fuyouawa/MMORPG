using MMORPG.Common.Proto.Entity;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public class MonsterMoveState : AbstractState<ActorState, MonsterBrain>
    {
        public MonsterMoveState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
        {

        }

        protected override void OnEnter()
        {
            mTarget.ActorController.Animator.SetBool("Walking", true);
        }

        protected override void OnExit()
        {
            mTarget.ActorController.Animator.SetBool("Walking", false);
        }

        protected override void OnUpdate()
        {
        }
    }
}
