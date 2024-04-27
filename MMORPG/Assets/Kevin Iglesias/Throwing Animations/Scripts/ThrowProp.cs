///////////////////////////////////////////////////////////////////////////
//  ThrowProp                                                            //
//  Kevin Iglesias - https://www.keviniglesias.com/       			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {
    
    public enum PropType
	{
		Spear,
        Knife,
		Tomahawk,
	}

    public class ThrowProp : MonoBehaviour {

        //Retargeter
        public Transform retargeter;

        //Different movements for different prop types
        public PropType propType;

        //Prop to move
        public Transform propToThrow;
        //Hand that holds the prop
        public Transform hand;

        //Target to throw the prop
        public Transform targetPos;
        
        //Speed of the prop
        public float speed = 10;
        
        //Maximum arc the prop will make
        public float arcHeight = 1;
        
        //Needed for checking if prop was thrown or not
        public bool launched = false;
        public bool recoverProp = false;
        
        //Needed for checking if prop landed
        public bool propLanded = false;

        //Character root (for parenting when prop is thrown)
        private Transform characterRoot;
        //Needed for calculate prop trajectory
        private Vector3 startPos; 
        private Vector3 zeroPosition;
        private Quaternion zeroRotation;
        private Vector3 nextPos;
        
        void Start() 
        {
            characterRoot = this.transform;
            
            zeroPosition = propToThrow.localPosition;
            zeroRotation = propToThrow.localRotation;
        }
        
        //This will make the prop move when launched
        void Update() 
        {
            //Arc throw facing the target
            if(launched && (propType == PropType.Spear || propType == PropType.Knife) && !propLanded)
            {
                float x0 = startPos.x;
                float x1 = targetPos.position.x;
                float dist = x1 - x0;
                float nextX = Mathf.MoveTowards(propToThrow.position.x, x1, speed * Time.deltaTime);
                float baseY = Mathf.Lerp(startPos.y, targetPos.position.y, (nextX - x0) / dist);
                float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
                Vector3 nextPos = new Vector3(nextX, baseY + arc, propToThrow.position.z);
            
                propToThrow.rotation = LookAt2D(nextPos - propToThrow.position);
                propToThrow.position = nextPos;
                
                float currentDistance = Mathf.Abs(targetPos.position.x - propToThrow.position.x);
                if(currentDistance < 0.5f)
                {
                    propLanded = true;
                }
     
            }
            
            //Arc throw rotating forwards
            if(launched && propType == PropType.Tomahawk && !propLanded)
            {
                float x0 = startPos.x;
                float x1 = targetPos.position.x;
                float dist = x1 - x0;
                float nextX = Mathf.MoveTowards(propToThrow.position.x, x1, speed * Time.deltaTime);
                float baseY = Mathf.Lerp(startPos.y, targetPos.position.y, (nextX - x0) / dist);
                float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
                Vector3 nextPos = new Vector3(nextX, baseY + arc, propToThrow.position.z);
            
                propToThrow.transform.Rotate(19f, 0.0f, 0.0f, Space.Self);
                propToThrow.position = nextPos;
                
                float currentDistance = Mathf.Abs(targetPos.position.x - propToThrow.position.x);
                if(currentDistance < 0.5f)
                {
                    propLanded = true;
                }
            }

            if(retargeter.localPosition.y > 0)
            {
                if(!launched && !recoverProp)
                {
                    Throw();
                }
                
                if(launched && recoverProp)
                {
                    RecoverProp();
                }
            }else{
                
                if(!recoverProp && launched)
                {
                    recoverProp = true;
                }
                
                if(recoverProp && !launched)
                {
                    recoverProp = false;
                }
            }
        }
        
        static Quaternion LookAt2D(Vector3 forward) {
            return Quaternion.Euler(0, 0, (Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg)-90f);
        }   
        
        //Function called when retargeter is active
        public void Throw()
        {
            launched = true;
            startPos = propToThrow.position;
            propToThrow.SetParent(characterRoot);
        }

        //Function called when retargeter is active
        public void RecoverProp()
        {
            propLanded = false;
            launched = false;
            propToThrow.SetParent(hand);
            propToThrow.localPosition = zeroPosition;
            propToThrow.localRotation = zeroRotation;
        }
    }

}