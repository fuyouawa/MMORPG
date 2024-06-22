using MMORPG.Common.Proto.Entity;
using QFramework;

namespace MMORPG.Game
{
    public class MonsterIdleState : AbstractState<ActorState, MonsterBrain>
    {

        public MonsterIdleState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
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
