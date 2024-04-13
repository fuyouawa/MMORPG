//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2019 Thomas Enzenebner
//            Version 1.0.5.3
//         t.enzenebner@gmail.com
//----------------------------------------------
using ThirdPersonCamera;
using UnityEngine;

namespace ThirdPersonCamera
{
    [RequireComponent(typeof(CameraController))]
    public class OverTheShoulder : MonoBehaviour
    {
        public float maxValue;

        public float aimSpeed = 7.0f;
        public float releaseSpeed = 7.0f;

        public bool left;
        public KeyCode changeLeaning = KeyCode.LeftShift;

        public Vector3 baseOffset = new Vector3(0, 0, 0);
        public Vector3 slideAxis = new Vector3(1, 0, 0);
        public Vector3 additionalAxisMovement = new Vector3(0, 0, 0);
        public CameraController cc;

        private Vector3 offsetValue;

        void Start()
        {
            offsetValue = Vector3.zero;

            if (cc == null)
                cc = GetComponent<CameraController>();
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                // aim mode

                if (Input.GetKeyDown(changeLeaning))
                    left = !left;

                Vector3 newOffset = left ? -maxValue * slideAxis + baseOffset + additionalAxisMovement : maxValue * slideAxis + baseOffset + additionalAxisMovement;

                offsetValue = Vector3.Lerp(offsetValue, newOffset, Time.deltaTime * aimSpeed);

                cc.cameraOffsetVector = offsetValue;
            }
            else if (offsetValue != Vector3.zero)
            {
                // release mode

                offsetValue = Vector3.Lerp(offsetValue, baseOffset, Time.deltaTime * releaseSpeed);

                cc.cameraOffsetVector = offsetValue;
            }
        }
    }
}