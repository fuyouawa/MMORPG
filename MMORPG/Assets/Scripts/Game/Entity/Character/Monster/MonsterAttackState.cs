using MMORPG.Common.Proto.Monster;
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
            mTarget.Animator.SetBool("Attack", true);
        }

        protected override void OnExit()
        {
            mTarget.Animator.SetBool("Attack", false);
        }
    }
}
