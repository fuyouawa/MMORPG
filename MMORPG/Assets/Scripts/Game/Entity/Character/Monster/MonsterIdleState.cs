using MMORPG.Common.Proto.Entity;
using QFramework;

namespace MMORPG.Game
{
    public class MonsterIdleState : AbstractState<AnimationState, MonsterBrain>
    {

        public MonsterIdleState(FSM<AnimationState> fsm, MonsterBrain target) : base(fsm, target)
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
