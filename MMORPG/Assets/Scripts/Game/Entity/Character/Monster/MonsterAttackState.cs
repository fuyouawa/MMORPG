using MMORPG.Common.Proto.Entity;
using QFramework;
using UnityEngine;
using AnimationState = MMORPG.Common.Proto.Entity.AnimationState;

namespace MMORPG.Game
{
    public class MonsterAttackState : AbstractState<AnimationState, MonsterBrain>
    {
        public MonsterAttackState(FSM<AnimationState> fsm, MonsterBrain target) : base(fsm, target)
        {

        }

        protected override void OnEnter()
        {
            mTarget.ActorController.Animator.SetTrigger("Attack");
        }

        protected override void OnExit()
        {
        }
    }
}
