using System.Collections;
using UnityEngine;

namespace MMORPG.Game
{
    public class LocalPlayerIdle : LocalPlayerAbility
    {
        public float HangupThreshold = 5f;

        private bool _isHanging;

        public override void OnStateEnter()
        {
            Brain.AnimationController.StopMovement();
            StartCoroutine("HangupCo");
        }

        private IEnumerator HangupCo()
        {
            _isHanging = false;
            yield return new WaitForSeconds(HangupThreshold);
            _isHanging = true;
        }

        public override void OnStateExit()
        {
            StopCoroutine("HangupCo");
        }

        public override void OnStateNetworkFixedUpdate()
        {
            if (!_isHanging)
            {
                Brain.NetworkUploadTransform(OwnerStateId);
            }
        }

        [StateCondition]
        public bool CanMovement()
        {
            return !Brain.ActorController.IsPreventingMovement;
        }
    }

}
