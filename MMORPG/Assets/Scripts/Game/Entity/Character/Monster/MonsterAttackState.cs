using MMORPG.Common.Proto.Entity;
using QFramework;

namespace MMORPG.Game
{
    public class MonsterAttackState : AbstractState<ActorState, MonsterBrain>
    {
        public MonsterAttackState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
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
