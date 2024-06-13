// Copyright (c) 2024 Synty Studios Limited. All rights reserved.
//
// Use of this software is subject to the terms and conditions of the Synty Studios End User Licence Agreement (EULA)
// available at: https://syntystore.com/pages/end-user-licence-agreement
//
// Sample scripts are included only as examples and are not intended as production-ready.

using Synty.AnimationBaseLocomotion.Samples.InputSystem;
using UnityEngine;

namespace Synty.AnimationBaseLocomotion.Samples
{
    public class SampleCameraController : MonoBehaviour
    {
        private const int _LAG_DELTA_TIME_ADJUSTMENT = 20;

        [Tooltip("The character game object")]
        [SerializeField]
        private GameObject _syntyCharacter;
        [Tooltip("Main camera used for player perspective")]
        [SerializeField]
        private Camera _mainCamera;

        [SerializeField]
        private Transform _playerTarget;
        [SerializeField]
        private Transform _lockOnTarget;

        [SerializeField]
        private bool _invertCamera;
        [SerializeField]
        private bool _hideCursor;
        [SerializeField]
        private bool _isLockedOn;
        [SerializeField]
        private float _mouseSensitivity = 5f;
        [SerializeField]
        private float _cameraDistance = 5f;
        [SerializeField]
        private float _cameraHeightOffset;
        [SerializeField]
        private float _cameraHorizontalOffset;
        [SerializeField]
        private float _cameraTiltOffset;
        [SerializeField]
        private Vector2 _cameraTiltBounds = new Vector2(-10f, 45f);
        [SerializeField]
        private float _positionalCameraLag = 1f;
        [SerializeField]
        private float _rotationalCameraLag = 1f;
        private float _cameraInversion;

        private InputReader _inputReader;
        private float _lastAngleX;
        private float _lastAngleY;

        private Vector3 _lastPosition;

        private float _newAngleX;

        private float _newAngleY;
        private Vector3 _newPosition;
        private float _rotationX;
        private float _rotationY;

        private Transform _syntyCamera;

        /// <inheritdoc cref="Start" />
        private void Start()
        {
            _syntyCamera = gameObject.transform.GetChild(0);

            _inputReader = _syntyCharacter.GetComponent<InputReader>();
            _playerTarget = _syntyCharacter.transform.Find("SyntyPlayer_LookAt");
            _lockOnTarget = _syntyCharacter.transform.Find("TargetLockOnPos");

            if (_hideCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            _cameraInversion = _invertCamera ? 1 : -1;

            transform.position = _playerTarget.position;
            transform.rotation = _playerTarget.rotation;

            _lastPosition = transform.position;

            _syntyCamera.localPosition = new Vector3(_cameraHorizontalOffset, _cameraHeightOffset, _cameraDistance * -1);
            _syntyCamera.localEulerAngles = new Vector3(_cameraTiltOffset, 0f, 0f);
        }

        /// <inheritdoc cref="Update" />
        private void Update()
        {
            float positionalFollowSpeed = 1 / (_positionalCameraLag / _LAG_DELTA_TIME_ADJUSTMENT);
            float rotationalFollowSpeed = 1 / (_rotationalCameraLag / _LAG_DELTA_TIME_ADJUSTMENT);

            _rotationX = _inputReader._mouseDelta.y * _cameraInversion * _mouseSensitivity;

            _rotationY = _inputReader._mouseDelta.x * _mouseSensitivity;

            _newAngleX += _rotationX;
            _newAngleX = Mathf.Clamp(_newAngleX, _cameraTiltBounds.x, _cameraTiltBounds.y);
            _newAngleX = Mathf.Lerp(_lastAngleX, _newAngleX, rotationalFollowSpeed * Time.deltaTime);

            if (_isLockedOn)
            {
                Vector3 aimVector = _lockOnTarget.position - _playerTarget.position;
                Quaternion targetRotation = Quaternion.LookRotation(aimVector);
                targetRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationalFollowSpeed * Time.deltaTime);
                _newAngleY = targetRotation.eulerAngles.y;
            }
            else
            {
                _newAngleY += _rotationY;
                _newAngleY = Mathf.Lerp(_lastAngleY, _newAngleY, rotationalFollowSpeed * Time.deltaTime);
            }

            _newPosition = _playerTarget.position;
            _newPosition = Vector3.Lerp(_lastPosition, _newPosition, positionalFollowSpeed * Time.deltaTime);

            transform.position = _newPosition;
            transform.eulerAngles = new Vector3(_newAngleX, _newAngleY, 0);

            _syntyCamera.localPosition = new Vector3(_cameraHorizontalOffset, _cameraHeightOffset, _cameraDistance * -1);
            _syntyCamera.localEulerAngles = new Vector3(_cameraTiltOffset, 0f, 0f);

            _lastPosition = _newPosition;
            _lastAngleX = _newAngleX;
            _lastAngleY = _newAngleY;
        }

        /// <summary>
        ///     Locks the camera to aim at a specified target.
        /// </summary>
        /// <param name="enable">Whether lock on is enabled or not.</param>
        /// <param name="newLockOnTarget">The target to lock on to.</param>
        public void LockOn(bool enable, Transform newLockOnTarget)
        {
            _isLockedOn = enable;

            if (newLockOnTarget != null)
            {
                _lockOnTarget = newLockOnTarget;
            }
        }

        /// <summary>
        /// Gets the position of the camera.
        /// </summary>
        /// <returns>The position of the camera.</returns>
        public Vector3 GetCameraPosition()
        {
            return _mainCamera.transform.position;
        }

        /// <summary>
        /// Gets the forward vector of the camera.
        /// </summary>
        /// <returns>The forward vector of the camera.</returns>
        public Vector3 GetCameraForward()
        {
            return _mainCamera.transform.forward;
        }

        /// <summary>
        /// Gets the forward vector of the camera with the Y value zeroed.
        /// </summary>
        /// <returns>The forward vector of the camera with the Y value zeroed.</returns>
        public Vector3 GetCameraForwardZeroedY()
        {
            return new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z);
        }

        /// <summary>
        /// Gets the normalised forward vector of the camera with the Y value zeroed.
        /// </summary>
        /// <returns>The normalised forward vector of the camera with the Y value zeroed.</returns>
        public Vector3 GetCameraForwardZeroedYNormalised()
        {
            return GetCameraForwardZeroedY().normalized;
        }


        /// <summary>
        /// Gets the right vector of the camera with the Y value zeroed.
        /// </summary>
        /// <returns>The right vector of the camera with the Y value zeroed.</returns>
        public Vector3 GetCameraRightZeroedY()
        {
            return new Vector3(_mainCamera.transform.right.x, 0, _mainCamera.transform.right.z);
        }

        /// <summary>
        /// Gets the normalised right vector of the camera with the Y value zeroed.
        /// </summary>
        /// <returns>The normalised right vector of the camera with the Y value zeroed.</returns>
        public Vector3 GetCameraRightZeroedYNormalised()
        {
            return GetCameraRightZeroedY().normalized;
        }

        /// <summary>
        /// Gets the X value of the camera tilt.
        /// </summary>
        /// <returns>The X value of the camera tilt.</returns>
        public float GetCameraTiltX()
        {
            return _mainCamera.transform.eulerAngles.x;
        }
    }
}
