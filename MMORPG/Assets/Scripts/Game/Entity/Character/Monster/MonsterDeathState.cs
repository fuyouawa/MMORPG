using System.Collections;
using MMORPG.Common.Proto.Entity;
 using QFramework;
 using Serilog;

 namespace MMORPG.Game
{
    public class MonsterDeathState : AbstractState<AnimationState, MonsterBrain>
    {
        public MonsterDeathState(FSM<AnimationState> fsm, MonsterBrain target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Log.Information($"{mTarget.ActorController.Entity.gameObject.name}死亡");
            if (mTarget.DeathFeedbacks != null)
            {
                mTarget.DeathFeedbacks.Play();
            }
            mTarget.ActorController.Animator.SetTrigger("Death");
        }

        protected override void OnExit()
        {
            mTarget.ResurrectionFeedbacks.Play();
            mTarget.ActorController.Animator.Play("Idle");
            if (mTarget.MonsterCanvas != null)
            {
                mTarget.MonsterCanvas.gameObject.SetActive(true);
            }
        }
    }
}
