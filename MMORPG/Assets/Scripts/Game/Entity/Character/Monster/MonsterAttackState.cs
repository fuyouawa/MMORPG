using MMORPG.Common.Proto.Entity;
using QFramework;

namespace MMORPG.Game
{
    public class AttackState : AbstractState<ActorState, MonsterBrain>
    {
        public AttackState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
        {

        }

        protected override void OnEnter()
        {
            mTarget.Animator.SetTrigger("Attack");
        }

        protected override void OnExit()
        {
        }
    }
}
