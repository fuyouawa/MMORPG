using MMORPG.Common.Proto.Entity;
using QFramework;
using UnityEngine;
using AnimationState = MMORPG.Common.Proto.Entity.AnimationState;

namespace MMORPG.Game
{
    public class MonsterMoveState : AbstractState<AnimationState, MonsterBrain>
    {
        public MonsterMoveState(FSM<AnimationState> fsm, MonsterBrain target) : base(fsm, target)
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
