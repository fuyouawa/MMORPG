//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2019 Thomas Enzenebner
//            Version 1.0.5.3
//         t.enzenebner@gmail.com
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace ThirdPersonCamera
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        #region Public Unity Variables
        public Transform target;        
        public Vector3 offsetVector = new Vector3(0, 1.0f, 0);
        public Vector3 cameraOffsetVector = new Vector3(0, 0, 0);

        public bool smartPivot = true;
        public bool occlusionCheck = true;
        public bool thicknessCheck = true;

        public float desiredDistance = 5.0f;
        public float collisionDistance = 0.5f;
        public float maxThickness = 0.3f;
        public int maxThicknessIterations = 5;
        public float zoomOutStepValue = 1.0f;
        public float zoomOutStepValuePerFrame = 0.1f;

        public bool smoothTargetMode;
        public float smoothTargetValue = 7.0f;

        public bool hideSkinnedMeshRenderers = false;
        public LayerMask collisionLayer;
        public LayerMask playerLayer;

        [HideInInspector]
        public bool playerCollision;
        [HideInInspector]
        public bool cameraNormalMode;
        [HideInInspector]
        public bool bGroundHit;
        [HideInInspector]
        public float startingY;
        #endregion

        #region Private Variables
        // usually allocated in update, moved to avoid GC problems
        private bool initDone;
        private Vector3 prevTargetPos;
        private Vector3 prevPosition;

        private SkinnedMeshRenderer[] smrs;
        private float distance;
        private float thickness;

        private Vector3 targetPosWithOffset;
        private Vector3 dirToTarget;
        private Vector3 dirToTargetSmartPivot;
        private Vector3 thicknessStartPoint;
        private Vector3 thicknessEndPoint;

        private RaycastHit? occlusionHit = null;
        private RaycastHit groundHit;
        private RaycastHit thicknessHit;
        private RaycastHit hitSmartPivot;
        private RaycastHit offsetTest;

        private int currentIterations;
        private Dictionary<string, RaycastHit> thicknessStarts;
        private Dictionary<string, RaycastHit> thicknessEnds;
        private List<RayCastWithMags> rcms;
        private SortRayCastsCamera sortMethodCamera;
        private SortRayCastsTarget sortMethodTarget;
       
        private Vector3 targetPosition;
        private Vector3 smoothedTargetPosition;
        #endregion

        #region Public Get/Set Variables
        public float Distance
        {
            get
            {
                return distance;
            }
        }

        #endregion


        void Awake()
        {
            initDone = false;          

            sortMethodCamera = new SortRayCastsCamera();
            sortMethodTarget = new SortRayCastsTarget();
            distance = desiredDistance;

            cameraNormalMode = true;

            playerCollision = false;

            currentIterations = 0;
            thicknessStarts = new Dictionary<string, RaycastHit>();
            thicknessEnds = new Dictionary<string, RaycastHit>();

            InitFromTarget();
        }

        void InitFromTarget()
        {
            InitFromTarget(target);
        }

        /// <summary>
        /// Use this to re-init a target, for example, when switching through multiple characters
        /// </summary>
        /// <param name="newTarget"></param>
        public void InitFromTarget(Transform newTarget)
        {
            target = newTarget;

            if (target != null)
            {
                prevTargetPos = target.position;
                prevPosition = transform.position;
                targetPosition = target.position;
                smoothedTargetPosition = target.position;

                if (hideSkinnedMeshRenderers)
                    smrs = target.GetComponentsInChildren<SkinnedMeshRenderer>();
                else
                    smrs = null;

                // get colliders from target
                var colliders = target.GetComponentsInChildren<Collider>();

                foreach (var col in colliders)
                {
                    if (!playerLayer.IsInLayerMask(col.gameObject))
                        Debug.LogWarning("The target \"" + col.gameObject.name + "\" has a collider which is not in the player layer. To fix: Change the layer of " + col.gameObject.name + " to the layer referenced in CameraController->Player layer");
                }

                initDone = true;
            }
        }

        void LateUpdate()
        {
            if (target == null)
                return;           

            if (!initDone)
                return;

            if (distance < 0)
                distance = 0;

            // disable player character when too close
            if (!hideSkinnedMeshRenderers && smrs != null)
            {
                if (playerCollision || distance <= collisionDistance)
                {
                    for (int i = 0; i < smrs.Length; i++)
                    {
                        smrs[i].enabled = false;
                    }
                }
                else
                {
                    for (int i = 0; i < smrs.Length; i++)
                    {
                       smrs[i].enabled = true;
                    }
                }
            }

            Vector3 offsetVectorTransformed = target.rotation * offsetVector;
            Vector3 cameraOffsetVectorTransformed = transform.rotation * cameraOffsetVector;           

            if (smoothTargetMode)
            {
                smoothedTargetPosition = Vector3.Lerp(
                                            smoothedTargetPosition,
                                            target.position,
                                            smoothTargetValue * Time.deltaTime);

                targetPosition = smoothedTargetPosition;
            }
            else
            {
                targetPosition = target.position;
            }

            targetPosWithOffset = (targetPosition + offsetVectorTransformed);
            Vector3 targetPosWithCameraOffset = targetPosition + offsetVectorTransformed + cameraOffsetVectorTransformed;
            
            Vector3 testDir = targetPosWithCameraOffset - targetPosWithOffset;

            if (Physics.SphereCast(targetPosWithOffset, collisionDistance, testDir, out offsetTest, testDir.magnitude, collisionLayer))
            {
                // offset clips into geometry, move the offset
                targetPosWithCameraOffset = offsetTest.point + offsetTest.normal * collisionDistance * 2;
            }                   

            dirToTarget = (transform.rotation * (new Vector3(0, 0, -distance) + cameraOffsetVector) + offsetVectorTransformed + targetPosition) - targetPosWithCameraOffset;
            float cameraToPlayerDistance = Mathf.Min(dirToTarget.magnitude, desiredDistance);

            dirToTarget = dirToTarget.normalized;            

            RaycastHit[] hits = Physics.SphereCastAll(targetPosWithCameraOffset, collisionDistance, dirToTarget, desiredDistance, collisionLayer);

            //Debug.DrawRay(targetPosWithCameraOffset, dirToTarget * desiredDistance, Color.blue);

            if (hits.Length > 0)
            {
                rcms = new List<RayCastWithMags>(hits.Length);

                for (int i = 0; i < hits.Length; i++)
                {
                    RayCastWithMags rcm = new RayCastWithMags();

                    rcm.hit = hits[i];
                    rcm.distanceFromCamera = (prevPosition - hits[i].point).magnitude;
                    rcm.distanceFromTarget = (targetPosWithOffset - hits[i].point).magnitude;

                    rcms.Add(rcm);
                }

                List<RayCastWithMags> sortList = new List<RayCastWithMags>(rcms);               

                sortList.Sort(sortMethodTarget);
                
                float lowestMagn = sortList[0].distanceFromTarget;

                for (int i = sortList.Count - 1; i >= 0; i--)
                {
                    if (sortList[i].distanceFromTarget > lowestMagn + collisionDistance * 2)
                        sortList.RemoveAt(i);
                }

                if (sortList[0].hit.distance > 0)
                {
                    occlusionHit = sortList[0].hit;
                }
            }
            else
            {
                occlusionHit = null;               
            }

            // get current thickness between target and camera
            if (thicknessCheck && occlusionHit != null)
                thickness = GetThickness();
            else
                thickness = float.MaxValue;

            if ((cameraNormalMode || !smartPivot))
            {
                if (distance < desiredDistance)
                {
                    // safe guard for zooming out
                    Vector3 dirToTargetDistanced = ((transform.rotation * (new Vector3(0, 0, -distance - zoomOutStepValuePerFrame) + cameraOffsetVector) + offsetVectorTransformed + targetPosition) - (targetPosWithCameraOffset));

                    RaycastHit zoomOutHit;
                    if ((thicknessCheck && thickness > maxThickness) && Physics.SphereCast(targetPosWithCameraOffset, collisionDistance, dirToTargetDistanced.normalized, out zoomOutHit, dirToTargetDistanced.magnitude, collisionLayer))
                    {
                        distance = (zoomOutHit.point - targetPosWithCameraOffset).magnitude - collisionDistance;
                    }
                    else
                    {
                        distance += zoomOutStepValuePerFrame;
                    }
                }
                if (distance > desiredDistance)
                {
                    distance -= zoomOutStepValuePerFrame;
                }
            }

            // Cast ground target to activate smartPivot
            if ((smartPivot && occlusionHit != null) || bGroundHit) // needs a small distance offset
            {
                if (Physics.Raycast(prevPosition, Vector3.down, out groundHit, collisionDistance + 0.1f) && !playerLayer.IsInLayerMask(groundHit.transform.gameObject))
                {
                    bGroundHit = true;
                }
                else
                    bGroundHit = false;
            }
            else if (occlusionHit == null && cameraNormalMode)
            {
                bGroundHit = false;
            }

            // Avoid that the character is not visible
            if (occlusionCheck && occlusionHit != null)
            {
                if (cameraNormalMode)
                {
                    if (thickness > maxThickness)
                    {
                        //Debug.DrawLine(targetPosWithOffset, occlusionHit.Value.point, Color.green);
                        //Debug.DrawLine(targetPosWithOffset, occlusionHit.Value.point + occlusionHit.Value.normal.normalized * (collisionDistance), Color.red);

                        transform.position = occlusionHit.Value.point + occlusionHit.Value.normal.normalized * (collisionDistance);
                    }
                    else
                    {
                        transform.position = (transform.rotation * (new Vector3(0, 0, -distance) + cameraOffsetVector)) + offsetVectorTransformed + targetPosition; ;
                    }
                }
            }
            else if (cameraNormalMode)
            {
                thickness = float.MaxValue;
                transform.position = transform.rotation * (new Vector3(0, 0, -distance) + cameraOffsetVector) + offsetVectorTransformed + targetPosition; ;
            }            

            // smart pivot mode collision check
            if (smartPivot && (!cameraNormalMode))
            {
                Vector3 tmpEuler = transform.rotation.eulerAngles;
                tmpEuler.x = startingY;

                dirToTargetSmartPivot = (Quaternion.Euler(tmpEuler) * (new Vector3(0, 0, -distance) + cameraOffsetVector) + offsetVectorTransformed + targetPosition) - targetPosWithOffset;
               
                if (Physics.SphereCast(targetPosWithOffset, collisionDistance, dirToTargetSmartPivot.normalized, out hitSmartPivot, dirToTargetSmartPivot.magnitude + collisionDistance, collisionLayer))
                {                   
                    Vector3 v1 = hitSmartPivot.point - targetPosWithOffset;
                    float dot = Vector3.Dot(v1, dirToTargetSmartPivot) / dirToTargetSmartPivot.magnitude;
                    
                    Vector3 v2 = (dirToTargetSmartPivot.normalized * dot) - (dirToTargetSmartPivot.normalized * collisionDistance);

                    Vector3 newAlignVector = Vector3.zero;
                    if (v2.magnitude < dirToTargetSmartPivot.magnitude)
                        newAlignVector = v2;
                    else
                        newAlignVector = dirToTargetSmartPivot;

                    if (dot > 0.1f)
                    {
                        if ((transform.position - targetPosWithOffset).magnitude + zoomOutStepValuePerFrame < newAlignVector.magnitude)
                        {
                            transform.position += newAlignVector.normalized * zoomOutStepValuePerFrame;
                            transform.position += targetPosition - prevTargetPos;
                        }
                        else
                        {
                            transform.position = targetPosWithOffset + newAlignVector;
                        }
                    }
                }
                else
                {
                    // no clipping, transition to max distance                
                    if ((transform.position - targetPosWithOffset).magnitude + zoomOutStepValuePerFrame < dirToTargetSmartPivot.magnitude)
                    {
                        transform.position += dirToTargetSmartPivot.normalized * zoomOutStepValuePerFrame;                        
                    }

                    transform.position += targetPosition - prevTargetPos;
                }                
            }

            prevTargetPos = targetPosition;
            prevPosition = transform.position;
        }

        public void RotateTo(Quaternion targetRotation, float timeModifier)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, timeModifier);
        }

        public float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        public void InitSmartPivot()
        {
            cameraNormalMode = false;
            startingY = transform.rotation.eulerAngles.x;
        }

        public void DisableSmartPivot()
        {
            cameraNormalMode = true;

            Vector3 tmpEuler = transform.rotation.eulerAngles;
            tmpEuler.x = startingY;
            transform.rotation = Quaternion.Euler(tmpEuler);
        }

        public void OnTriggerEnter(Collider c)
        {            
            if (c.transform == target)
            {
                playerCollision = true;                
            }
        }

        public void OnTriggerExit(Collider c)
        {            
            if (c.transform == target)
            {
                playerCollision = false;
            }
        }

        public float GetThickness()
        {
            currentIterations = 0;
            thicknessStarts.Clear();
            thicknessEnds.Clear();

            Vector3 dirToHit = (transform.position - targetPosWithOffset).normalized;

            Vector3 hitVector = (targetPosWithOffset - occlusionHit.Value.point);
            Vector3 targetVector = targetPosWithOffset - transform.position;

            float dotProd = Vector3.Dot(hitVector, targetVector) / targetVector.magnitude;
            Vector3 unknownPoint = occlusionHit.Value.point + targetVector.normalized * dotProd;

            thicknessStartPoint = unknownPoint + dirToHit * (desiredDistance + collisionDistance);
            thicknessEndPoint = unknownPoint;

            float length = desiredDistance + collisionDistance;

            while (Physics.SphereCast(thicknessEndPoint, collisionDistance, dirToHit, out thicknessHit, length, collisionLayer) && currentIterations < maxThicknessIterations)
            {
                length -= (thicknessEndPoint - thicknessHit.point).magnitude - 0.00001f;
                thicknessEndPoint = thicknessHit.point + dirToTarget * 0.00001f;
                if (!thicknessEnds.ContainsKey(thicknessHit.collider.name))
                    thicknessEnds.Add(thicknessHit.collider.name, thicknessHit);

                currentIterations++;
            }

            currentIterations = 0;
            length = desiredDistance;

            while (Physics.SphereCast(thicknessStartPoint, collisionDistance, -dirToHit, out thicknessHit, length, collisionLayer) && currentIterations < maxThicknessIterations)
            {
                length -= (thicknessStartPoint - thicknessHit.point).magnitude - 0.00001f;
                thicknessStartPoint = thicknessHit.point - dirToTarget * 0.00001f;

                if (!thicknessStarts.ContainsKey(thicknessHit.collider.name))
                    thicknessStarts.Add(thicknessHit.collider.name, thicknessHit);

                currentIterations++;
            }

            float tmpThickness = float.MaxValue;

            if (thicknessEnds.Count > 0 && thicknessStarts.Count > 0 && thicknessEnds.Count == thicknessStarts.Count)
            {
                bool thicknessFound = false;
                string currentColliderName = "";

                var en = thicknessEnds.GetEnumerator();
                while (en.MoveNext())
                {
                    currentColliderName = en.Current.Value.collider.name;

                    if (thicknessStarts.ContainsKey(currentColliderName))
                    {
                        if (!thicknessFound)
                        {
                            tmpThickness = 0;
                            thicknessFound = true;
                        }
                        tmpThickness += (thicknessStarts[currentColliderName].point - thicknessEnds[currentColliderName].point).magnitude;
                    }
                }
            }

            return tmpThickness;
        }
    }
}