///////////////////////////////////////////////////////////////////////////
//  ChangeSpear                                                          //
//  Kevin Iglesias - https://www.keviniglesias.com/       			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {
    
    public class ChangeSpear : MonoBehaviour {

        //Retargeter
        public Transform retargeter;

        //Prop to move
        public Transform spear;
        
        //Hand that holds the prop
        public Transform hand;
        
        //Needed for check if the change is active
        public bool changeActive = false;
        
        //Needed for check if the change is even or not
        public bool secondTime = false;
        
        //Character root (for parenting when prop is thrown)
        private Transform characterRoot;
        //Needed for getting the prop back
        private Vector3 zeroPosition;
        private Vector3 zeroRotation;
        //Needed for calculate prop trajectory
        private Vector3 startPosition;
        private Quaternion startRotation;
        private Vector3 endPosition;
        private Quaternion endRotation;
        //Coroutine that will make the prop move
        private IEnumerator changeCO;

        public void Start()
        {
            characterRoot = this.transform;
            
            zeroPosition = spear.localPosition;
            zeroRotation = spear.localEulerAngles;
        }
        
        public void Update()
        {
            if(retargeter.localPosition.y > 0)
            {
                if(!changeActive)
                {
                    DoChangeSpear();
                    changeActive = true;
                }
            }else{
                changeActive = false;
            }
        }
        
        //Function called when retargeter is active
        public void DoChangeSpear()
        {
            if(changeCO != null)
            {
                StopCoroutine(changeCO);
            }
            changeCO = StartChange();
            StartCoroutine(changeCO);
        }
        
        IEnumerator StartChange()
        {
            //Remove prop from hand
            spear.SetParent(characterRoot);
            
            //Get initial position/rotation
            startPosition = spear.position;
            startRotation = spear.localRotation;
            
            //Set end position (highest point the prop will get)
            endPosition = new Vector3(spear.position.x, spear.position.y+0.2f, spear.position.z);

            //Rotation is different each time
            float yRotation = 0.70f;
            if(secondTime)
            {
                yRotation = -0.25f;
            }
            
            //Going up
            float i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * 5f;
                spear.position = Vector3.Lerp(startPosition, endPosition, Mathf.Sin(i * Mathf.PI * 0.5f));
                spear.transform.Rotate(0.70f, yRotation, 0f, Space.World);
                yield return 0;
            }
            
            startPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z);
            
            //Going down
            i = 0;
            while(i < 1)
            {
                i += Time.deltaTime * 5f;
                spear.position = Vector3.Lerp(endPosition, startPosition, 1f - Mathf.Cos(i * Mathf.PI * 0.5f));
                spear.transform.Rotate(0.70f, yRotation, 0f, Space.World);
                yield return 0;
            }
            
            //Back to the hand
            spear.SetParent(hand);
            spear.localPosition = zeroPosition;
            spear.localEulerAngles = zeroRotation;
            
            //Get the correct rotation of the spearhead
            if(!secondTime)
            {
                spear.localEulerAngles = new Vector3(spear.localEulerAngles.x+180f, spear.localEulerAngles.y, spear.localEulerAngles.z);
                secondTime = true;
            }else{
                secondTime = false;
            }
        }
    }
}