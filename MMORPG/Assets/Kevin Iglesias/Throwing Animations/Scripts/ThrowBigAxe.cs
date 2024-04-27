///////////////////////////////////////////////////////////////////////////
//  ThrowBigAxe                                                          //
//  Kevin Iglesias - https://www.keviniglesias.com/       			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {
    
    public class ThrowBigAxe : MonoBehaviour {

        //Retargeter
        public Transform retargeter;

        //Prop to move
        public Transform propToSpin;
        
        //Hand that holds the prop
        public Transform hand;
        
        //How far will the prop launched
        public float spinDistance;
        
        //Movement speed of the prop
        public float translationSpeed;
        
        //Rotation speed of the prop
        public float spinSpeed;
        
        //Needed for check if the trick is active
        public bool spinActive = false;
        
        //Offset for fitting the prop in end distance
        public Vector3 endPositionOffset;
        
        //Offset for fitting the prop in hand when returning
        public Vector3 returningPositionOffset;
        
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

        public void Awake()
        {
            characterRoot = this.transform;
            
            zeroPosition = propToSpin.localPosition;
            zeroRotation = propToSpin.localRotation;
        }
        
        public void Update()
        {
            if(retargeter.localPosition.y > 0)
            {
                if(!spinActive)
                {
                    SpinProp();
                    spinActive = true;
                }
            }else{
                
                if(spinActive)
                {
                    if(spinCO != null)
                    {
                        StopCoroutine(spinCO);
                    }
                    propToSpin.SetParent(hand);
                    propToSpin.localPosition = zeroPosition;
                    propToSpin.localRotation = zeroRotation;
                }
                spinActive = false;
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
            //Remove prop from hand
            propToSpin.SetParent(characterRoot);
            
            //Get initial position/rotation
            startPosition = propToSpin.position;
            startRotation = propToSpin.localRotation;

            //Set end position (farthest point the prop will get)
            endPosition = new Vector3(propToSpin.position.x-spinDistance, propToSpin.position.y, propToSpin.position.z);
            endPosition = endPosition+endPositionOffset;
            
            //Going away
            float i = 0;
            while(i < 1f)
            {
                
                i += Time.deltaTime * translationSpeed;

                propToSpin.position = Vector3.Lerp(startPosition, endPosition, Mathf.Sin(i * Mathf.PI * 0.5f));
                propToSpin.transform.Rotate(0.0f, -spinSpeed, 0.0f, Space.World);
                yield return 0;
            }
            
            //Coming back
            i = 0;
            while(i < 1f)
            {
                i += Time.deltaTime * translationSpeed;
                
                propToSpin.position = Vector3.Lerp(endPosition, startPosition+returningPositionOffset, 1f - Mathf.Cos(i * Mathf.PI * 0.5f));
                propToSpin.transform.Rotate(0f, -spinSpeed, 0.0f, Space.World);
                
                yield return 0;
            }
        }
        
    }
}