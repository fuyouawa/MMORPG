using UnityEngine;

namespace MMORPG.Game
{
    public class LocalPlayerIdle : LocalPlayerAbility
    {
        public override void OnStateEnter()
        {
            Brain.CharacterController.NetworkUploadTransform(OwnerStateId, null);
            Brain.AnimationController.StopMovement();
        }

        [StateCondition]
        public bool CanMovement()
        {
            return !Brain.CharacterController.PreventMovement;
        }
    }

}
