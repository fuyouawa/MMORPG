///////////////////////////////////////////////////////////////////////////
//  Archer Animations - BowLoadScript                                    //
//  Kevin Iglesias - https://www.keviniglesias.com/     			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////

// This script makes the bow animate and the arrow look ready to shot when 
// drawing it from the quiver.

// To do the pulling animation, the bow mesh needs a BlendShape/ShapeKey named 'Load' 
// and the character needs empty Gameobjects in your Unity scene named 'LeftHandProp' 
// and 'RightHandProp' (depending of the hand to use) as a child of another empty
// GameObject called 'Retargeters', see character dummies hierarchy from the demo 
// scene as example. More information at Documentation PDF file.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {
	public class BowLoadScript : MonoBehaviour
	{
	   
		public Transform bow;
        
        //Retargeters
		public Transform arrowHandRetargeter;
        public Transform bowHandRetargeter;
		
		//Bow Blendshape
		SkinnedMeshRenderer bowSkinnedMeshRenderer;
		
		//Arrow draw & rotation
		public bool arrowOnHand;
		public Transform arrowToDraw;
		public Transform arrowToShoot; 
	   
		void Awake()
		{
			
			if(bow != null)
			{
				bowSkinnedMeshRenderer = bow.GetComponent<SkinnedMeshRenderer>();
			}
			
			if(arrowToDraw != null)
			{
				arrowToDraw.gameObject.SetActive(false);
			}
			if(arrowToShoot != null)
			{
				arrowToShoot.gameObject.SetActive(false);
			}
		}

		void Update()
		{
			//Bow blendshape animation
            if(bowSkinnedMeshRenderer != null && bow != null && bowHandRetargeter != null)
            {
                float bowWeight = Mathf.InverseLerp(0, -1f, bowHandRetargeter.localPosition.z);
                bowSkinnedMeshRenderer.SetBlendShapeWeight(0, bowWeight*100);
            }
			
			//Draw arrow from quiver and rotate it
            if(arrowToDraw != null && arrowToShoot != null && arrowHandRetargeter != null)
            {
                
                if(arrowHandRetargeter.localPosition.y <= 0.99f)
                {
                    if(arrowToShoot != null && arrowToDraw != null)
                    {
                        arrowToDraw.gameObject.SetActive(false);
                        arrowToShoot.gameObject.SetActive(false);
                        arrowOnHand = false;
                    }
                }else{
                    if(arrowHandRetargeter.localPosition.y <= 1.01f)
                    {
                        arrowToShoot.gameObject.SetActive(true);
                        arrowToDraw.gameObject.SetActive(false);
                        arrowOnHand = true; 
                    }else{
                        if(arrowToShoot != null && arrowToDraw != null)
                        {
                            arrowToDraw.gameObject.SetActive(true);
                            arrowToShoot.gameObject.SetActive(false);
                            arrowOnHand = true;
                        }
                    }
                }
            }
		}
	}
}
