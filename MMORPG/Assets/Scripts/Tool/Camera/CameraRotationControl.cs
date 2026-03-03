using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MMORPG.Tool
{
    /// <summary>
    /// 相机自由旋转控制组件，处理鼠标输入来控制相机视角
    /// </summary>
    [RequireComponent(typeof(ThirdPersonCamera))]
    public class CameraRotationControl : MonoBehaviour
    {
        [SerializeField] private bool _enable = true;
        [SerializeField] private CameraControlMode _mode = CameraControlMode.InputOnly;
        [SerializeField] private List<int> _mouseButtons = new List<int>() { 1 };
        [SerializeField] private bool _isMouseCursorLocked = true;
        [SerializeField] private float _minDistance = 1;
        [SerializeField] private float _maxDistance = 5;
        [SerializeField] private Vector2 _mouseSensitivity = new Vector2(1.5f, 1.0f);
        [SerializeField] private Vector2 _verticalAngleRange = new Vector2(0f, 180.0f);

        private Vector2 _input;

        private float _verticalAngle;
        private float _currentAngle;

        private Vector3 _upVector;
        private Vector3 _downVector;

        private ThirdPersonCamera _camera;

        void Start()
        {
            _camera = GetComponent<ThirdPersonCamera>();

            _upVector = Vector3.up;
            _downVector = Vector3.down;
        }

        void LateUpdate()
        {
            if (_camera == null || _camera.Target == null)
                return;

            if (_enable && !EventSystem.current.IsPointerOverGameObject())
            {
                bool inputFreeLook = _mode == CameraControlMode.Always;

                CheckMouseInput(ref inputFreeLook);

                ProcessInput(inputFreeLook);

                ApplyZoom();

                ApplyRotation();
            }
        }

        private void CheckMouseInput(ref bool inputFreeLook)
        {
            if (inputFreeLook || _mouseButtons.Count == 0)
                return;

            for (int i = 0; i < _mouseButtons.Count && !inputFreeLook; i++)
            {
                if (Input.GetMouseButton(_mouseButtons[i]))
                    inputFreeLook = true;
            }
        }

        private void ProcessInput(bool inputFreeLook)
        {
            if (inputFreeLook)
            {
                var x = Input.GetAxis("Mouse X") * _mouseSensitivity.x;
                var y = Input.GetAxis("Mouse Y") * _mouseSensitivity.y;
                _input = new Vector2(x, y);

                if (_isMouseCursorLocked)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else
            {
                _input = Vector2.zero;

                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        private void ApplyZoom()
        {
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scrollWheel) > 0.01f)
            {
                // 鼠标滚轮向上(正值)缩小距离，向下(负值)增大距离
                float delta = -scrollWheel * _camera.ZoomStepSize;
                _camera.AdjustZoom(delta, _minDistance, _maxDistance);
            }
        }

        private void ApplyRotation()
        {
            Vector3 targetOffsetTransformed = _camera.Target.transform.rotation * _camera.TargetOffset;

            transform.RotateAround(_camera.Target.position + targetOffsetTransformed, _camera.Target.up, _input.x);

            _verticalAngle = -_input.y;
            _currentAngle = Vector3.Angle(transform.forward, _upVector);

            ClampVerticalAngle();

            RotateVertical(targetOffsetTransformed);
        }

        private void ClampVerticalAngle()
        {
            if (_currentAngle <= _verticalAngleRange.x && _verticalAngle < 0)
            {
                _verticalAngle = 0;
            }

            if (_currentAngle >= _verticalAngleRange.y && _verticalAngle > 0)
            {
                _verticalAngle = 0;
            }

            if (_verticalAngle > 0)
            {
                if (_currentAngle + _verticalAngle > 180.0f)
                {
                    _verticalAngle = Vector3.Angle(transform.forward, _upVector) - 180;

                    if (_verticalAngle < 0)
                        _verticalAngle = 0;
                }
            }
            else
            {
                if (_currentAngle + _verticalAngle < 0.0f)
                {
                    _verticalAngle = Vector3.Angle(transform.forward, _downVector) - 180;
                }
            }
        }

        private void RotateVertical(Vector3 targetOffsetTransformed)
        {
            transform.RotateAround(_camera.Target.position + targetOffsetTransformed, transform.right, _verticalAngle);
        }
    }
}
