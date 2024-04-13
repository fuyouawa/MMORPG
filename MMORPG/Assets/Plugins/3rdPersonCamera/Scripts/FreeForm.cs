//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2019 Thomas Enzenebner
//            Version 1.0.5.3
//         t.enzenebner@gmail.com
//----------------------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonCamera
{
    public enum CameraMode
    {
        Always,
        Input
    }

    [RequireComponent(typeof(CameraController))]
    public class FreeForm : MonoBehaviour
    {
        public bool cameraEnabled = true;
        public CameraMode cameraMode = CameraMode.Input;

        public List<int> mouseInput = new List<int>() { 0, 1 }; // default input is left and right mouse button
        public List<KeyCode> keyboardInput = new List<KeyCode>();

        public bool controllerEnabled = false;
        public bool controllerInvertY = true;
        public bool mouseInvertY = false;
        public bool lockMouseCursor = true;

        public float minDistance = 1;
        public float maxDistance = 5;

        public Vector2 mouseSensitivity = new Vector2(1.5f, 1.0f);
        public Vector2 controllerSensitivity = new Vector2(1.0f, 0.7f);

        public float yAngleLimitMin = 0.0f;
        public float yAngleLimitMax = 180.0f;

        [HideInInspector]
        public float x;
        [HideInInspector]
        public float y;
        private float yAngle;
        private float angle;

        private string rightAxisXName;
        private string rightAxisYName;

        private Vector3 upVector;
        private Vector3 downVector;

        private bool smartPivotInit;

        private float smoothX;
        private float smoothY;
        public bool smoothing = false;
        public float smoothSpeed = 3.0f;

        public bool forceCharacterDirection = false;

        private CameraController cameraController;

        public void Start()
        {
            cameraController = GetComponent<CameraController>();
            
            x = 0;
            y = 0;

            smartPivotInit = true;

            upVector = Vector3.up;
            downVector = Vector3.down;

            string platform = Application.platform.ToString().ToLower();

            if (platform.Contains("windows") || platform.Contains("linux"))
            {
                rightAxisXName = "Right_4";
                rightAxisYName = "Right_5";
            }
            else
            {
                rightAxisXName = "Right_3";
                rightAxisYName = "Right_4";
            }

            // test if the controller axis are setup
            try
            {
                Input.GetAxis(rightAxisXName);
                Input.GetAxis(rightAxisYName);
            }
            catch
            {
                Debug.LogWarning("Controller Error - Right axis not set in InputManager. Controller is disabled!");
                controllerEnabled = false;
            }
        }

        public void LateUpdate()
        {
            if (cameraController == null || cameraController.target == null)
                return;

            if (cameraEnabled)
            {
                bool inputFreeLook = cameraMode == CameraMode.Always;

                // sample mouse inputs
                if (!inputFreeLook && mouseInput.Count > 0)
                {
                    for (int i = 0; i < mouseInput.Count && !inputFreeLook; i++)
                    {
                        if (Input.GetMouseButton(mouseInput[i]))
                            inputFreeLook = true;
                    }
                }

                // sample keyboard inputs
                if (!inputFreeLook && keyboardInput.Count > 0)
                {
                    for (int i = 0; i < keyboardInput.Count && !inputFreeLook; i++)
                    {
                        if (Input.GetKey(keyboardInput[i]))
                            inputFreeLook = true;
                    }
                }

                if (inputFreeLook)
                {
                    x = Input.GetAxis("Mouse X") * mouseSensitivity.x;
                    y = Input.GetAxis("Mouse Y") * mouseSensitivity.y;                   

                    if (mouseInvertY)
                        y *= -1.0f;

                    if (lockMouseCursor)
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
                else
                {
                    x = 0;
                    y = 0;

                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
                
                if (controllerEnabled && x == 0 && y == 0)
                {
                    // sample controller input
                    x = Input.GetAxis(rightAxisXName) * controllerSensitivity.x;
                    y = Input.GetAxis(rightAxisYName) * controllerSensitivity.y;

                    if (controllerInvertY)
                        y *= -1.0f;
                }

                // sample mouse scrollwheel for zooming in/out
                if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
                {
                    cameraController.desiredDistance += cameraController.zoomOutStepValue;

                    if (cameraController.desiredDistance > maxDistance)
                        cameraController.desiredDistance = maxDistance;
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
                {
                    cameraController.desiredDistance -= cameraController.zoomOutStepValue;
                    
                    if (cameraController.desiredDistance < minDistance)
                        cameraController.desiredDistance = minDistance;
                }

                if (cameraController.desiredDistance < 0)
                    cameraController.desiredDistance = 0;

                if (smoothing)
                {
                    smoothX = Mathf.Lerp(smoothX, x, Time.deltaTime * smoothSpeed);
                    smoothY = Mathf.Lerp(smoothY, y, Time.deltaTime * smoothSpeed);
                }
                else
                {
                    smoothX = x;
                    smoothY = y;
                }
                
                Vector3 offsetVectorTransformed = cameraController.target.transform.rotation * cameraController.offsetVector;

                transform.RotateAround(cameraController.target.position + offsetVectorTransformed, cameraController.target.up, smoothX);

                yAngle = -smoothY;
                // Prevent camera flipping
                angle = Vector3.Angle(transform.forward, upVector);

                if (angle <= yAngleLimitMin && yAngle < 0)
                {
                    yAngle = 0;
                }
                if (angle >= yAngleLimitMax && yAngle > 0)
                {
                    yAngle = 0;
                }

                if (yAngle > 0)
                {
                    if (angle + yAngle > 180.0f)
                    {
                        yAngle = Vector3.Angle(transform.forward, upVector) - 180;

                        if (yAngle < 0)
                            yAngle = 0;
                    }
                }
                else
                {
                    if (angle + yAngle < 0.0f)
                    {
                        yAngle = Vector3.Angle(transform.forward, downVector) - 180;
                    }
                }                               

                if (!cameraController.smartPivot || cameraController.cameraNormalMode
                    && (!cameraController.bGroundHit || (cameraController.bGroundHit && y < 0) || transform.position.y > (cameraController.target.position.y + cameraController.offsetVector.y)))
                {
                    // normal mode                   
                    transform.RotateAround(cameraController.target.position + offsetVectorTransformed, transform.right, yAngle);
                }
                else
                {
                    // smart pivot mode
                    if (smartPivotInit)
                    {
                        smartPivotInit = false;
                        cameraController.InitSmartPivot();
                    }

                    transform.RotateAround(transform.position, transform.right, yAngle);

                    if (transform.rotation.eulerAngles.x > cameraController.startingY || (transform.rotation.eulerAngles.x >= 0 && transform.rotation.eulerAngles.x < 90))
                    {
                        smartPivotInit = true;

                        cameraController.DisableSmartPivot();
                    }
                }

                if (forceCharacterDirection)
                {
                    cameraController.target.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up);
                }
            }
        }
    }
}

