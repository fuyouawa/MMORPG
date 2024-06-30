using MMORPG.Tool;

namespace MMORPG.Game
{
    public class RemotePlayerDeath : RemotePlayerAbility
    {
        public override void OnStateEnter()
        {
            OwnerState.Brain.ActorController.Animator.SetTrigger("Death");
        }

        public override void OnStateExit()
        {
            OwnerState.Brain.ActorController.Animator.Play("Idle");
        }
    }
}
