using MMORPG.Common.Proto.Entity;
 using QFramework;
 using Serilog;

 namespace MMORPG.Game
{
    public class MonsterDeathState : AbstractState<ActorState, MonsterBrain>
    {
        public MonsterDeathState(FSM<ActorState> fsm, MonsterBrain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Log.Information($"{mTarget.ActorController.Entity.gameObject.name}死亡");
            mTarget.ActorController.Animator.SetBool("Death", true);
        }

        protected override void OnExit()
        {
            Log.Information($"{mTarget.ActorController.Entity.gameObject.name}复活");
            mTarget.ActorController.Animator.SetBool("Death", false);
        }
    }
}
