using MMORPG.Common.Proto.Entity;
 using QFramework;

 namespace MMORPG.Game
{
    public class MonsterHurtState : AbstractState<AnimationState, MonsterBrain>
    {
        public MonsterHurtState(FSM<AnimationState> fsm, MonsterBrain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
        }
    }
}
