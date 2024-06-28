using System.Collections;
using UnityEngine;

namespace MMORPG.Game
{
    public class LocalPlayerHurt : LocalPlayerAbility
    {
        public override void OnStateEnter()
        {
            OwnerState.Brain.ActorController.Animator.SetTrigger("Hurt");
            StartCoroutine("HurtTimeCo");
        }

        public override void OnStateExit()
        {
            StopCoroutine("HurtTimeCo");
        }

        private IEnumerator HurtTimeCo()
        {
            yield return new WaitForSeconds(OwnerState.Brain.ActorController.Entity.UnitDefine.HurtTime);
            OwnerState.Brain.ChangeStateByName("Idle");
        }
    }
}
