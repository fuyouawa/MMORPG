using MMORPG.Common.Proto.Monster;
 using QFramework;

 namespace MMORPG.Game
{
    public class MonsterHurtState : AbstractState<ActorState, MonsterBrain>
    {
        public MonsterHurtState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
        }
    }
}
