using UnityEngine;

namespace MMORPG.Game
{
    public class LocalHeroKnightIdle : LocalPlayerAbility
    {
        public override void OnStateEnter()
        {
            Brain.CharacterController.NetworkUploadTransform(OwnerStateId, null);

            Brain.AnimationController.Movement = false;
            Brain.AnimationController.SmoothMoveDirection(Vector2.zero);
        }

        [StateCondition]
        public bool CanMovement()
        {
            return true;
        }
    }

}
