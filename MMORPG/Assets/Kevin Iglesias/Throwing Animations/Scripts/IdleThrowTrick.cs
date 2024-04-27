///////////////////////////////////////////////////////////////////////////
//  IdleThrowTrick                                                       //
//  Kevin Iglesias - https://www.keviniglesias.com/       			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////

using  System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {
    
    public class IdleThrowTrick : MonoBehaviour {

        //Retargeter
        public Transform retargeter;
        
        //Prop to move
        public Transform propToThrow;
        
        //Hand that holds the prop
        public Transform hand;
        
        //How far will the prop launched
        public float trickDistance;
        
        //Movement speed of the prop
        public float trickTranslationSpeed;
        
        //Rotation speed of the prop
        public float trickRotationSpeed;
        
        //Needed for check if the trick is active
        public bool trickActive = false;
        
        //Character root (for parenting when prop is thrown)
        private Transform characterRoot;
        //Needed for getting the prop back
        private Vector3 zeroPosition;
        private Quaternion zeroRotation;
        //Needed for calculate prop trajectory
        private Vector3 startPosition;
        private Quaternion startRotation;
        private Vector3 endPosition;
        private Quaternion endRotation;
        //Coroutine that will make the prop move
        private IEnumerator spinCO;
    
        public void Start()
        {
            characterRoot = this.transform;
            
            zeroPosition = propToThrow.localPosition;
            zeroRotation = propToThrow.localRotation;
        }
        
        
        public void Update()
        {
            if(retargeter.localPosition.y > 0)
            {
                if(!trickActive)
                {
                    SpinProp();
                    trickActive = true;
                }
            }else{
                if(trickActive)
                {
                    if(spinCO != null)
                    {
                        StopCoroutine(spinCO);
                    }
                    propToThrow.SetParent(hand);
                    propToThrow.localPosition = zeroPosition;
                    propToThrow.localRotation = zeroRotation;
                }
                trickActive = false;
            }
        }
    
        //Function called when retargeter is active
        public void SpinProp()
        {
            if(spinCO != null)
            {
                StopCoroutine(spinCO);
            }
            spinCO = StartSpin();
            StartCoroutine(spinCO);
        }
        
        IEnumerator StartSpin()
        {
            
            
            //Get initial position/rotation
            startPosition = propToThrow.position;
            startRotation = propToThrow.localRotation;
            
            //Set end position (highest point the prop will get)
            endPosition = new Vector3(propToThrow.position.x, propToThrow.position.y+trickDistance, propToThrow.position.z);
            
            //Remove prop from hand
            propToThrow.SetParent(characterRoot);
            
            //Going up
            float i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * trickTranslationSpeed;
                propToThrow.position = Vector3.Lerp(startPosition, endPosition, Mathf.Sin(i * Mathf.PI * 0.5f));
                propToThrow.transform.Rotate(0.0f, 0.0f, -trickRotationSpeed, Space.World);
                yield return 0;
            }
            
            startPosition = new Vector3(startPosition.x, startPosition.y-0.11f, startPosition.z);

            //Going down
            i = 0;
            while(i < 1f)
            {
                i += Time.deltaTime * trickTranslationSpeed;
                propToThrow.position = Vector3.Lerp(endPosition, startPosition, 1f - Mathf.Cos(i * Mathf.PI * 0.5f));
                propToThrow.transform.Rotate(0f, 0.0f, -trickRotationSpeed, Space.World);
                yield return 0;
            }
            
        }
    }
}