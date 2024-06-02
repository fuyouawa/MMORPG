//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2019 Thomas Enzenebner
//            Version 1.0.5.3
//         t.enzenebner@gmail.com
//----------------------------------------------
using UnityEngine;

namespace ThirdPersonCamera
{
    public class Follow : MonoBehaviour
    {
        public bool follow = true;
        public bool alignOnSlopes = true;

        public float rotationSpeed = 1.0f;
        public float rotationSpeedSlopes = 0.5f;
        public bool lookBackwards = false;

        public bool checkMotionForBackwards = true;
        public float backwardsMotionThreshold = 0.05f;
        public float angleThreshold = 170.0f;

        public Vector3 tiltVector;
        public LayerMask layerMask;

        private Vector3 prevPosition;

        CameraController cc;

        void Start()
        {
            cc = GetComponent<CameraController>();
        }

        void Update()
        {
            if (cc == null || cc.target == null)
                return;

            if (follow)
            {
                if (checkMotionForBackwards)
                {
                    Vector3 motionVector = cc.target.transform.position - prevPosition;

                    if (motionVector.magnitude > backwardsMotionThreshold)
                    {
                        float angle = Vector3.Angle(motionVector, cc.target.transform.forward);

                        if (angle > angleThreshold)
                        {
                            lookBackwards = true;
                        }
                        else
                            lookBackwards = false;
                    }

                    prevPosition = cc.target.transform.position;
                }

                Quaternion toRotation = Quaternion.LookRotation((!lookBackwards ? cc.target.transform.forward + tiltVector : -cc.target.transform.forward + tiltVector), Vector3.up);

                if (alignOnSlopes)
                {
                    RaycastHit raycastHit;
                    Vector3 upVector = Vector3.up;

                    if (Physics.Raycast(cc.target.transform.position, Vector3.down, out raycastHit, 25.0f, layerMask)) // if the range of 15.0 is not enough, increase the value
                    {
                        upVector = raycastHit.normal;
                    }

                    float angle = AngleSigned(Vector3.up, upVector, cc.target.transform.right);

                    toRotation = Quaternion.Slerp(toRotation, toRotation * Quaternion.AngleAxis(angle, Vector3.right), Time.deltaTime * rotationSpeedSlopes);
                }

                cc.transform.rotation = Quaternion.Slerp(cc.transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
            }
        }

        public float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }
    }
}