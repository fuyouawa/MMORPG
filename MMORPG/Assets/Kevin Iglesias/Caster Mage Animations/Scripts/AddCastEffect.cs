using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {

    public class AddCastEffect : StateMachineBehaviour {

        CastSpells cS;

        public CastHand castHand;
        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            
            if(cS == null)
            {
                cS = animator.GetComponent<CastSpells>();
            }
            
            if(cS != null)
            {
               cS.SpawnEffect(castHand);
            }
        }
    }
}
