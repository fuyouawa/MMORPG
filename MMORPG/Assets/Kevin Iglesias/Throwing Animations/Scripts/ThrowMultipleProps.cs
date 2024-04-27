///////////////////////////////////////////////////////////////////////////
//  ThrowMultipleProps                                                   //
//  Kevin Iglesias - https://www.keviniglesias.com/       			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {

    public class ThrowMultipleProps : MonoBehaviour {

        //Retargeters
        public Transform retargeter1;
        public Transform retargeter2;
    
        //Props to move
        public Transform propToThrow1;
        public Transform propToThrow2;
    
        //Hands that holds the props
        public Transform hand1;
        public Transform hand2;
    
        //Target to throw the prop
        public Transform targetPos;

        //Speed of the prop
        public float speed = 10;
        
        //Maximum arc the prop will make
        public float arcHeight = 1;
        
        //Needed for checking if prop was thrown or not
        public bool launched1 = false;
        public bool launched2 = false;
        public bool recoverProp1 = false;
        public bool recoverProp2 = false;
        
        //Needed for checking if prop landed
        public bool propLanded1 = false;
        public bool propLanded2 = false;
        
        //Character root (for parenting when prop is thrown)
        private Transform characterRoot;
        
        //Needed for calculate prop trajectory
        private Vector3 startPos1;
        private Vector3 startPos2;
        private Vector3 zeroPosition1;
        private Quaternion zeroRotation1;
        private Vector3 zeroPosition2;
        private Quaternion zeroRotation2;
        private Vector3 nextPos;
        
        void Start() {
            characterRoot = this.transform;
            
            zeroPosition1 = propToThrow1.localPosition;
            zeroRotation1 = propToThrow1.localRotation;
            zeroPosition2 = propToThrow2.localPosition;
            zeroRotation2 = propToThrow2.localRotation;
        }
        
        //This will make the prop move when launched
        void Update() 
        {
            //Arc throw prop 1
            if(launched1 && !propLanded1)
            {
                float x0 = startPos1.x;
                float x1 = targetPos.position.x;
                float dist = x1 - x0;
                float nextX = Mathf.MoveTowards(propToThrow1.position.x, x1, speed * Time.deltaTime);
                float baseY = Mathf.Lerp(startPos1.y, targetPos.position.y, (nextX - x0) / dist);
                float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
                Vector3 nextPos = new Vector3(nextX, baseY + arc, propToThrow1.position.z);
            
                propToThrow1.rotation = LookAt2D(nextPos - propToThrow1.position);
                propToThrow1.position = nextPos;
                
                float currentDistance = Mathf.Abs(targetPos.position.x - propToThrow1.position.x);
                if(currentDistance < 0.5f)
                {
                    propLanded1 = true;
                }
    
            }
            
            //Arc throw prop 2
            if(launched2 && !propLanded2)
            {
                float x0 = startPos2.x;
                float x1 = targetPos.position.x;
                float dist = x1 - x0;
                float nextX = Mathf.MoveTowards(propToThrow2.position.x, x1, speed * Time.deltaTime);
                float baseY = Mathf.Lerp(startPos2.y, targetPos.position.y, (nextX - x0) / dist);
                float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
                Vector3 nextPos = new Vector3(nextX, baseY + arc, propToThrow2.position.z);
            
                propToThrow2.rotation = LookAt2D(nextPos - propToThrow2.position);
                propToThrow2.position = nextPos;
                
                float currentDistance = Mathf.Abs(targetPos.position.x - propToThrow2.position.x);
                if(currentDistance < 0.5f)
                {
                    propLanded2 = true;
                }
     
            }
            
            if(retargeter1.localPosition.y > 0)
            {
                if(!launched1 && !recoverProp1)
                {
                    Throw1();
                }
                
                if(launched1 && recoverProp1)
                {
                    RecoverProp1();
                }
            }else{
                
                if(!recoverProp1 && launched1)
                {
                    recoverProp1 = true;
                }
                
                if(recoverProp1 && !launched1)
                {
                    recoverProp1 = false;
                }
            }
            
            if(retargeter2.localPosition.y > 0)
            {
                if(!launched2 && !recoverProp2)
                {
                    Throw2();
                }
                
                if(launched2 && recoverProp2)
                {
                    RecoverProp2();
                }
            }else{
                
                if(!recoverProp2 && launched2)
                {
                    recoverProp2 = true;
                }
                
                if(recoverProp2 && !launched2)
                {
                    recoverProp2 = false;
                }
            }
            
        }
        
        static Quaternion LookAt2D(Vector3 forward) {
            return Quaternion.Euler(0, 0, (Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg)-90);
        }   
        
       //Function called when retargeter1 is active
        public void Throw1()
        {
            startPos1 = propToThrow1.position;
            propToThrow1.SetParent(characterRoot);
            launched1 = true;
        }
        
        //Function called when retargeter2 is active
        public void Throw2()
        {
            startPos2 = propToThrow2.position;
            propToThrow2.SetParent(characterRoot);
            launched2 = true;
        }
        
        //Function called when retargeter1 is active
        public void RecoverProp1()
        {
            propLanded1 = false;
            launched1 = false;
            propToThrow1.SetParent(hand1);
            propToThrow1.localPosition = zeroPosition1;
            propToThrow1.localRotation = zeroRotation1;
        }
        
        //Function called when retargeter2 is active
        public void RecoverProp2()
        {
            propLanded2 = false;
            launched2 = false;
            propToThrow2.SetParent(hand2);
            propToThrow2.localPosition = zeroPosition2;
            propToThrow2.localRotation = zeroRotation2;
        }
    }

}