using MMORPG.Common.Proto.Monster;
using QFramework;

namespace MMORPG.Game
{
    public class IdleState : AbstractState<ActorState, MonsterBrain>
    {

        public IdleState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
        {

        }

        protected override void OnEnter()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
}
