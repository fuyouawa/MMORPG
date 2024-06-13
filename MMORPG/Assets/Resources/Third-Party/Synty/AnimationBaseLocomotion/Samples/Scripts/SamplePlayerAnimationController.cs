// Copyright (c) 2024 Synty Studios Limited. All rights reserved.
//
// Use of this software is subject to the terms and conditions of the Synty Studios End User Licence Agreement (EULA)
// available at: https://syntystore.com/pages/end-user-licence-agreement
//
// Sample scripts are included only as examples and are not intended as production-ready.

using Synty.AnimationBaseLocomotion.Samples.InputSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Synty.AnimationBaseLocomotion.Samples
{
    public class SamplePlayerAnimationController : MonoBehaviour
    {
        #region Enum

        private enum AnimationState
        {
            Base,
            Locomotion,
            Jump,
            Fall,
            Crouch
        }

        private enum GaitState
        {
            Idle,
            Walk,
            Run,
            Sprint
        }

        #endregion

        #region Animation Variable Hashes

        private readonly int _movementInputTappedHash = Animator.StringToHash("MovementInputTapped");
        private readonly int _movementInputPressedHash = Animator.StringToHash("MovementInputPressed");
        private readonly int _movementInputHeldHash = Animator.StringToHash("MovementInputHeld");
        private readonly int _shuffleDirectionXHash = Animator.StringToHash("ShuffleDirectionX");
        private readonly int _shuffleDirectionZHash = Animator.StringToHash("ShuffleDirectionZ");

        private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        private readonly int _currentGaitHash = Animator.StringToHash("CurrentGait");

        private readonly int _isJumpingAnimHash = Animator.StringToHash("IsJumping");
        private readonly int _fallingDurationHash = Animator.StringToHash("FallingDuration");

        private readonly int _inclineAngleHash = Animator.StringToHash("InclineAngle");

        private readonly int _strafeDirectionXHash = Animator.StringToHash("StrafeDirectionX");
        private readonly int _strafeDirectionZHash = Animator.StringToHash("StrafeDirectionZ");

        private readonly int _forwardStrafeHash = Animator.StringToHash("ForwardStrafe");
        private readonly int _cameraRotationOffsetHash = Animator.StringToHash("CameraRotationOffset");
        private readonly int _isStrafingHash = Animator.StringToHash("IsStrafing");
        private readonly int _isTurningInPlaceHash = Animator.StringToHash("IsTurningInPlace");

        private readonly int _isCrouchingHash = Animator.StringToHash("IsCrouching");

        private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
        private readonly int _isStoppedHash = Animator.StringToHash("IsStopped");
        private readonly int _isStartingHash = Animator.StringToHash("IsStarting");

        private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");

        private readonly int _leanValueHash = Animator.StringToHash("LeanValue");
        private readonly int _headLookXHash = Animator.StringToHash("HeadLookX");
        private readonly int _headLookYHash = Animator.StringToHash("HeadLookY");

        private readonly int _bodyLookXHash = Animator.StringToHash("BodyLookX");
        private readonly int _bodyLookYHash = Animator.StringToHash("BodyLookY");

        private readonly int _locomotionStartDirectionHash = Animator.StringToHash("LocomotionStartDirection");

        #endregion

        #region Player Settings Variables

        #region Scripts/Objects

        [Header("External Components")]
        [Tooltip("Script controlling camera behavior")]
        [SerializeField]
        private SampleCameraController _cameraController;
        [Tooltip("InputReader handles player input")]
        [SerializeField]
        private InputReader _inputReader;
        [Tooltip("Animator component for controlling player animations")]
        [SerializeField]
        private Animator _animator;
        [Tooltip("Character Controller component for controlling player movement")]
        [SerializeField]
        private CharacterController _controller;

        #endregion

        #region Locomotion Settings

        [Header("Player Locomotion")]
        [Header("Main Settings")]
        [Tooltip("Whether the character always faces the camera facing direction")]
        [SerializeField]
        private bool _alwaysStrafe = true;
        [Tooltip("Slowest movement speed of the player when set to a walk state or half press tick")]
        [SerializeField]
        private float _walkSpeed = 1.4f;
        [Tooltip("Default movement speed of the player")]
        [SerializeField]
        private float _runSpeed = 2.5f;
        [Tooltip("Top movement speed of the player")]
        [SerializeField]
        private float _sprintSpeed = 7f;
        [Tooltip("Damping factor for changing speed")]
        [SerializeField]
        private float _speedChangeDamping = 10f;
        [Tooltip("Rotation smoothing factor.")]
        [SerializeField]
        private float _rotationSmoothing = 10f;
        [Tooltip("Offset for camera rotation.")]
        [SerializeField]
        private float _cameraRotationOffset;

        #endregion

        #region Shuffle Settings

        [Header("Shuffles")]
        [Tooltip("Threshold for button hold duration.")]
        [SerializeField]
        private float _buttonHoldThreshold = 0.15f;
        [Tooltip("Direction of shuffling on the X-axis.")]
        [SerializeField]
        private float _shuffleDirectionX;
        [Tooltip("Direction of shuffling on the Z-axis.")]
        [SerializeField]
        private float _shuffleDirectionZ;

        #endregion

        #region Capsule Settings

        [Header("Capsule Values")]
        [Tooltip("Standing height of the player capsule.")]
        [SerializeField]
        private float _capsuleStandingHeight = 1.8f;
        [Tooltip("Standing center of the player capsule.")]
        [SerializeField]
        private float _capsuleStandingCentre = 0.93f;
        [Tooltip("Crouching height of the player capsule.")]
        [SerializeField]
        private float _capsuleCrouchingHeight = 1.2f;
        [Tooltip("Crouching center of the player capsule.")]
        [SerializeField]
        private float _capsuleCrouchingCentre = 0.6f;

        #endregion

        #region Strafing

        [Header("Player Strafing")]
        [Tooltip("Minimum threshold for forward strafing angle.")]
        [SerializeField]
        private float _forwardStrafeMinThreshold = -55.0f;
        [Tooltip("Maximum threshold for forward strafing angle.")]
        [SerializeField]
        private float _forwardStrafeMaxThreshold = 125.0f;
        [Tooltip("Current forward strafing value.")]
        [SerializeField]
        private float _forwardStrafe = 1f;

        #endregion

        #region Grounded Settings

        [Header("Grounded Angle")]
        [Tooltip("Position of the rear ray for grounded angle check.")]
        [SerializeField]
        private Transform _rearRayPos;
        [Tooltip("Position of the front ray for grounded angle check.")]
        [SerializeField]
        private Transform _frontRayPos;
        [Tooltip("Layer mask for checking ground.")]
        [SerializeField]
        private LayerMask _groundLayerMask;
        [Tooltip("Current incline angle.")]
        [SerializeField]
        private float _inclineAngle;
        [Tooltip("Useful for rough ground")]
        [SerializeField]
        private float _groundedOffset = -0.14f;

        #endregion

        #region In-Air Settings

        [Header("Player In-Air")]
        [Tooltip("Force applied when the player jumps.")]
        [SerializeField]
        private float _jumpForce = 10f;
        [Tooltip("Multiplier for gravity when in the air.")]
        [SerializeField]
        private float _gravityMultiplier = 2f;
        [Tooltip("Duration of falling.")]
        [SerializeField]
        private float _fallingDuration;

        #endregion

        #region Head Look Settings

        [Header("Player Head Look")]
        [Tooltip("Flag indicating if head turning is enabled.")]
        [SerializeField]
        private bool _enableHeadTurn = true;
        [Tooltip("Delay for head turning.")]
        [SerializeField]
        private float _headLookDelay;
        [Tooltip("X-axis value for head turning.")]
        [SerializeField]
        private float _headLookX;
        [Tooltip("Y-axis value for head turning.")]
        [SerializeField]
        private float _headLookY;
        [Tooltip("Curve for X-axis head turning.")]
        [SerializeField]
        private AnimationCurve _headLookXCurve;

        #endregion

        #region Body Look Settings

        [Header("Player Body Look")]
        [Tooltip("Flag indicating if body turning is enabled.")]
        [SerializeField]
        private bool _enableBodyTurn = true;
        [Tooltip("Delay for body turning.")]
        [SerializeField]
        private float _bodyLookDelay;
        [Tooltip("X-axis value for body turning.")]
        [SerializeField]
        private float _bodyLookX;
        [Tooltip("Y-axis value for body turning.")]
        [SerializeField]
        private float _bodyLookY;
        [Tooltip("Curve for X-axis body turning.")]
        [SerializeField]
        private AnimationCurve _bodyLookXCurve;

        #endregion

        #region Lean Settings

        [Header("Player Lean")]
        [Tooltip("Flag indicating if leaning is enabled.")]
        [SerializeField]
        private bool _enableLean = true;
        [Tooltip("Delay for leaning.")]
        [SerializeField]
        private float _leanDelay;
        [Tooltip("Current value for leaning.")]
        [SerializeField]
        private float _leanValue;
        [Tooltip("Curve for leaning.")]
        [SerializeField]
        private AnimationCurve _leanCurve;
        [Tooltip("Delay for head leaning looks.")]
        [SerializeField]
        private float _leansHeadLooksDelay;
        [Tooltip("Flag indicating if an animation clip has ended.")]
        [SerializeField]
        private bool _animationClipEnd;

        #endregion

        #endregion

        #region Runtime Properties

        private readonly List<GameObject> _currentTargetCandidates = new List<GameObject>();
        private AnimationState _currentState = AnimationState.Base;
        private bool _cannotStandUp;
        private bool _crouchKeyPressed;
        private bool _isAiming;
        private bool _isCrouching;
        private bool _isGrounded = true;
        private bool _isLockedOn;
        private bool _isSliding;
        private bool _isSprinting;
        private bool _isStarting;
        private bool _isStopped = true;
        private bool _isStrafing;
        private bool _isTurningInPlace;
        private bool _isWalking;
        private bool _movementInputHeld;
        private bool _movementInputPressed;
        private bool _movementInputTapped;
        private float _currentMaxSpeed;
        private float _locomotionStartDirection;
        private float _locomotionStartTimer;
        private float _lookingAngle;
        private float _newDirectionDifferenceAngle;
        private float _speed2D;
        private float _strafeAngle;
        private float _strafeDirectionX;
        private float _strafeDirectionZ;
        private GameObject _currentLockOnTarget;
        private GaitState _currentGait;
        private Transform _targetLockOnPos;
        private Vector3 _currentRotation = new Vector3(0f, 0f, 0f);
        private Vector3 _moveDirection;
        private Vector3 _previousRotation;
        private Vector3 _velocity;

        #endregion

        #region Base State Variables

        private const float _ANIMATION_DAMP_TIME = 5f;
        private const float _STRAFE_DIRECTION_DAMP_TIME = 20f;
        private float _targetMaxSpeed;
        private float _fallStartTime;
        private float _rotationRate;
        private float _initialLeanValue;
        private float _initialTurnValue;
        private Vector3 _cameraForward;
        private Vector3 _targetVelocity;

        #endregion

        #region Animation Controller

        #region Start

        /// <inheritdoc cref="Start" />
        private void Start()
        {
            _targetLockOnPos = transform.Find("TargetLockOnPos");

            _inputReader.onLockOnToggled += ToggleLockOn;
            _inputReader.onWalkToggled += ToggleWalk;
            _inputReader.onSprintActivated += ActivateSprint;
            _inputReader.onSprintDeactivated += DeactivateSprint;
            _inputReader.onCrouchActivated += ActivateCrouch;
            _inputReader.onCrouchDeactivated += DeactivateCrouch;
            _inputReader.onAimActivated += ActivateAim;
            _inputReader.onAimDeactivated += DeactivateAim;

            _isStrafing = _alwaysStrafe;

            SwitchState(AnimationState.Locomotion);
        }

        #endregion

        #region Aim and Lock-on

        /// <summary>
        ///     Activates the aim action of the player.
        /// </summary>
        private void ActivateAim()
        {
            _isAiming = true;

            _isStrafing = !_isSprinting;
        }

        /// <summary>
        ///     Deactivates the aim action of the player.
        /// </summary>
        private void DeactivateAim()
        {
            _isAiming = false;
            _isStrafing = !_isSprinting && (_alwaysStrafe || _isLockedOn);
        }

        /// <summary>
        ///     Adds an object to the list of target candidates.
        /// </summary>
        /// <param name="newTarget">The object to add.</param>
        public void AddTargetCandidate(GameObject newTarget)
        {
            if (newTarget != null)
            {
                _currentTargetCandidates.Add(newTarget);
            }
        }

        /// <summary>
        ///     Removes an object to the list of target candidates if present.
        /// </summary>
        /// <param name="targetToRemove">The object to remove if present.</param>
        public void RemoveTarget(GameObject targetToRemove)
        {
            if (_currentTargetCandidates.Contains(targetToRemove))
            {
                _currentTargetCandidates.Remove(targetToRemove);
            }
        }

        /// <summary>
        ///     Toggle the lock-on state.
        /// </summary>
        private void ToggleLockOn()
        {
            EnableLockOn(!_isLockedOn);
        }

        /// <summary>
        ///     Sets the lock-on state to the given state.
        /// </summary>
        /// <param name="enable">The state to set lock-on to.</param>
        private void EnableLockOn(bool enable)
        {
            _isLockedOn = enable;
            _isStrafing = false;

            _isStrafing = enable ? !_isSprinting : _alwaysStrafe || _isAiming;

            _cameraController.LockOn(enable, _targetLockOnPos);

            if (enable && _currentLockOnTarget != null)
            {
                _currentLockOnTarget.GetComponent<SampleObjectLockOn>().Highlight(true, true);
            }
        }

        #endregion

        #region Walking State

        /// <summary>
        ///     Toggle the walking state.
        /// </summary>
        private void ToggleWalk()
        {
            EnableWalk(!_isWalking);
        }

        /// <summary>
        ///     Sets the walking state to that of the passed in state.
        /// </summary>
        /// <param name="enable">The state to set.</param>
        private void EnableWalk(bool enable)
        {
            _isWalking = enable && _isGrounded && !_isSprinting;
        }

        #endregion

        #region Sprinting State

        /// <summary>
        ///     Activates sprinting behaviour.
        /// </summary>
        private void ActivateSprint()
        {
            if (!_isCrouching)
            {
                EnableWalk(false);
                _isSprinting = true;
                _isStrafing = false;
            }
        }

        /// <summary>
        ///     Deactivates sprinting behaviour.
        /// </summary>
        private void DeactivateSprint()
        {
            _isSprinting = false;

            if (_alwaysStrafe || _isAiming || _isLockedOn)
            {
                _isStrafing = true;
            }
        }

        #endregion

        #region Crouching State

        /// <summary>
        ///     Activates crouching behaviour
        /// </summary>
        private void ActivateCrouch()
        {
            _crouchKeyPressed = true;

            if (_isGrounded)
            {
                CapsuleCrouchingSize(true);
                DeactivateSprint();
                _isCrouching = true;
            }
        }

        /// <summary>
        ///     Deactivates crouching behaviour.
        /// </summary>
        private void DeactivateCrouch()
        {
            _crouchKeyPressed = false;

            if (!_cannotStandUp && !_isSliding)
            {
                CapsuleCrouchingSize(false);
                _isCrouching = false;
            }
        }

        /// <summary>
        ///     Activates sliding behaviour.
        /// </summary>
        public void ActivateSliding()
        {
            _isSliding = true;
        }

        /// <summary>
        ///     Deactivates sliding behaviour
        /// </summary>
        public void DeactivateSliding()
        {
            _isSliding = false;
        }

        /// <summary>
        ///     Adjusts the capsule size for the player, depending on the passed in boolean value.
        /// </summary>
        /// <param name="crouching">Whether the player is crouching or not.</param>
        private void CapsuleCrouchingSize(bool crouching)
        {
            if (crouching)
            {
                _controller.center = new Vector3(0f, _capsuleCrouchingCentre, 0f);
                _controller.height = _capsuleCrouchingHeight;
            }
            else
            {
                _controller.center = new Vector3(0f, _capsuleStandingCentre, 0f);
                _controller.height = _capsuleStandingHeight;
            }
        }

        #endregion

        #endregion

        #region Shared State

        #region State Change

        /// <summary>
        ///     Switch the current state to the passed in state.
        /// </summary>
        /// <param name="newState">The state to switch to.</param>
        private void SwitchState(AnimationState newState)
        {
            ExitCurrentState();
            EnterState(newState);
        }

        /// <summary>
        ///     Enter the given state.
        /// </summary>
        /// <param name="stateToEnter">The state to enter.</param>
        private void EnterState(AnimationState stateToEnter)
        {
            _currentState = stateToEnter;
            switch (_currentState)
            {
                case AnimationState.Base:
                    EnterBaseState();
                    break;
                case AnimationState.Locomotion:
                    EnterLocomotionState();
                    break;
                case AnimationState.Jump:
                    EnterJumpState();
                    break;
                case AnimationState.Fall:
                    EnterFallState();
                    break;
                case AnimationState.Crouch:
                    EnterCrouchState();
                    break;
            }
        }

        /// <summary>
        ///     Exit the current state.
        /// </summary>
        private void ExitCurrentState()
        {
            switch (_currentState)
            {
                case AnimationState.Locomotion:
                    ExitLocomotionState();
                    break;
                case AnimationState.Jump:
                    ExitJumpState();
                    break;
                case AnimationState.Crouch:
                    ExitCrouchState();
                    break;
            }
        }

        #endregion

        #region Updates

        /// <inheritdoc cref="Update" />
        private void Update()
        {
            switch (_currentState)
            {
                case AnimationState.Locomotion:
                    UpdateLocomotionState();
                    break;
                case AnimationState.Jump:
                    UpdateJumpState();
                    break;
                case AnimationState.Fall:
                    UpdateFallState();
                    break;
                case AnimationState.Crouch:
                    UpdateCrouchState();
                    break;
            }
        }

        /// <summary>
        ///     Updates the animator to have the latest values.
        /// </summary>
        private void UpdateAnimatorController()
        {
            _animator.SetFloat(_leanValueHash, _leanValue);
            _animator.SetFloat(_headLookXHash, _headLookX);
            _animator.SetFloat(_headLookYHash, _headLookY);
            _animator.SetFloat(_bodyLookXHash, _bodyLookX);
            _animator.SetFloat(_bodyLookYHash, _bodyLookY);

            _animator.SetFloat(_isStrafingHash, _isStrafing ? 1.0f : 0.0f);

            _animator.SetFloat(_inclineAngleHash, _inclineAngle);

            _animator.SetFloat(_moveSpeedHash, _speed2D);
            _animator.SetInteger(_currentGaitHash, (int) _currentGait);

            _animator.SetFloat(_strafeDirectionXHash, _strafeDirectionX);
            _animator.SetFloat(_strafeDirectionZHash, _strafeDirectionZ);
            _animator.SetFloat(_forwardStrafeHash, _forwardStrafe);
            _animator.SetFloat(_cameraRotationOffsetHash, _cameraRotationOffset);

            _animator.SetBool(_movementInputHeldHash, _movementInputHeld);
            _animator.SetBool(_movementInputPressedHash, _movementInputPressed);
            _animator.SetBool(_movementInputTappedHash, _movementInputTapped);
            _animator.SetFloat(_shuffleDirectionXHash, _shuffleDirectionX);
            _animator.SetFloat(_shuffleDirectionZHash, _shuffleDirectionZ);

            _animator.SetBool(_isTurningInPlaceHash, _isTurningInPlace);
            _animator.SetBool(_isCrouchingHash, _isCrouching);

            _animator.SetFloat(_fallingDurationHash, _fallingDuration);
            _animator.SetBool(_isGroundedHash, _isGrounded);

            _animator.SetBool(_isWalkingHash, _isWalking);
            _animator.SetBool(_isStoppedHash, _isStopped);

            _animator.SetFloat(_locomotionStartDirectionHash, _locomotionStartDirection);
        }

        #endregion

        #endregion

        #region Base State

        #region Setup

        /// <summary>
        ///     Performs the actions required when entering the base state.
        /// </summary>
        private void EnterBaseState()
        {
            _previousRotation = transform.forward;
        }

        /// <summary>
        ///     Calculates the input type and sets the required internal states.
        /// </summary>
        private void CalculateInput()
        {
            if (_inputReader._movementInputDetected)
            {
                if (_inputReader._movementInputDuration == 0)
                {
                    _movementInputTapped = true;
                }
                else if (_inputReader._movementInputDuration > 0 && _inputReader._movementInputDuration < _buttonHoldThreshold)
                {
                    _movementInputTapped = false;
                    _movementInputPressed = true;
                    _movementInputHeld = false;
                }
                else
                {
                    _movementInputTapped = false;
                    _movementInputPressed = false;
                    _movementInputHeld = true;
                }

                _inputReader._movementInputDuration += Time.deltaTime;
            }
            else
            {
                _inputReader._movementInputDuration = 0;
                _movementInputTapped = false;
                _movementInputPressed = false;
                _movementInputHeld = false;
            }

            _moveDirection = (_cameraController.GetCameraForwardZeroedYNormalised() * _inputReader._moveComposite.y)
                + (_cameraController.GetCameraRightZeroedYNormalised() * _inputReader._moveComposite.x);
        }

        #endregion

        #region Movement

        /// <summary>
        ///     Performs the movement of the player
        /// </summary>
        private void Move()
        {
            _controller.Move(_velocity * Time.deltaTime);

            if (_isLockedOn)
            {
                if (_currentLockOnTarget != null)
                {
                    _targetLockOnPos.position = _currentLockOnTarget.transform.position;
                }
            }
        }

        /// <summary>
        ///     Applies gravity to the player.
        /// </summary>
        private void ApplyGravity()
        {
            if (_velocity.y > Physics.gravity.y)
            {
                _velocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            }
        }

        /// <summary>
        ///     Calculates the movement direction of the player, and sets the relevant flags.
        /// </summary>
        private void CalculateMoveDirection()
        {
            CalculateInput();

            if (!_isGrounded)
            {
                _targetMaxSpeed = _currentMaxSpeed;
            }
            else if (_isCrouching)
            {
                _targetMaxSpeed = _walkSpeed;
            }
            else if (_isSprinting)
            {
                _targetMaxSpeed = _sprintSpeed;
            }
            else if (_isWalking)
            {
                _targetMaxSpeed = _walkSpeed;
            }
            else
            {
                _targetMaxSpeed = _runSpeed;
            }

            _currentMaxSpeed = Mathf.Lerp(_currentMaxSpeed, _targetMaxSpeed, _ANIMATION_DAMP_TIME * Time.deltaTime);

            _targetVelocity.x = _moveDirection.x * _currentMaxSpeed;
            _targetVelocity.z = _moveDirection.z * _currentMaxSpeed;

            _velocity.z = Mathf.Lerp(_velocity.z, _targetVelocity.z, _speedChangeDamping * Time.deltaTime);
            _velocity.x = Mathf.Lerp(_velocity.x, _targetVelocity.x, _speedChangeDamping * Time.deltaTime);

            _speed2D = new Vector3(_velocity.x, 0f, _velocity.z).magnitude;
            _speed2D = Mathf.Round(_speed2D * 1000f) / 1000f;

            Vector3 playerForwardVector = transform.forward;

            _newDirectionDifferenceAngle = playerForwardVector != _moveDirection
                ? Vector3.SignedAngle(playerForwardVector, _moveDirection, Vector3.up)
                : 0f;

            CalculateGait();
        }

        /// <summary>
        ///     <pre>
        ///         Calculates the character gait.
        ///         Calculate what the current locomotion gait is (Walk, Run, Sprint)
        ///         (for use in jumps, landings etc when deciding which animation to use)
        ///         Gait values will be:
        ///         Idle = 0, Walk = 1, Run = 2, Sprint = 3
        ///     </pre>
        /// </summary>
        private void CalculateGait()
        {
            float runThreshold = (_walkSpeed + _runSpeed) / 2;
            float sprintThreshold = (_runSpeed + _sprintSpeed) / 2;

            if (_speed2D < 0.01)
            {
                _currentGait = GaitState.Idle;
            }
            else if (_speed2D < runThreshold)
            {
                _currentGait = GaitState.Walk;
            }
            else if (_speed2D < sprintThreshold)
            {
                _currentGait = GaitState.Run;
            }
            else
            {
                _currentGait = GaitState.Sprint;
            }
        }

        /// <summary>
        ///     Calculates the face move direction based on the locomotion of the character.
        /// </summary>
        private void FaceMoveDirection()
        {
            Vector3 characterForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            Vector3 characterRight = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
            Vector3 directionForward = new Vector3(_moveDirection.x, 0f, _moveDirection.z).normalized;

            _cameraForward = _cameraController.GetCameraForwardZeroedYNormalised();
            Quaternion strafingTargetRotation = Quaternion.LookRotation(_cameraForward);

            _strafeAngle = characterForward != directionForward ? Vector3.SignedAngle(characterForward, directionForward, Vector3.up) : 0f;

            _isTurningInPlace = false;

            if (_isStrafing)
            {
                if (_moveDirection.magnitude > 0.01)
                {
                    if (_cameraForward != Vector3.zero)
                    {
                        // Shuffle direction values - these are separate from the strafe values as we don't want to lerp, we need to know immediately
                        // what direction to shuffle, and then lock the value so it doesn't return to zero once we lose input (so the blend tree works
                        // to the end of the anim clip)
                        _shuffleDirectionZ = Vector3.Dot(characterForward, directionForward);
                        _shuffleDirectionX = Vector3.Dot(characterRight, directionForward);

                        UpdateStrafeDirection(
                            Vector3.Dot(characterForward, directionForward),
                            Vector3.Dot(characterRight, directionForward)
                        );
                        _cameraRotationOffset = Mathf.Lerp(_cameraRotationOffset, 0f, _rotationSmoothing * Time.deltaTime);

                        float targetValue = _strafeAngle > _forwardStrafeMinThreshold && _strafeAngle < _forwardStrafeMaxThreshold ? 1f : 0f;

                        if (Mathf.Abs(_forwardStrafe - targetValue) <= 0.001f)
                        {
                            _forwardStrafe = targetValue;
                        }
                        else
                        {
                            float t = Mathf.Clamp01(_STRAFE_DIRECTION_DAMP_TIME * Time.deltaTime);
                            _forwardStrafe = Mathf.SmoothStep(_forwardStrafe, targetValue, t);
                        }
                    }

                    transform.rotation = Quaternion.Slerp(transform.rotation, strafingTargetRotation, _rotationSmoothing * Time.deltaTime);
                }
                else
                {
                    UpdateStrafeDirection(1f, 0f);

                    float t = 20 * Time.deltaTime;
                    float newOffset = 0f;

                    if (characterForward != _cameraForward)
                    {
                        newOffset = Vector3.SignedAngle(characterForward, _cameraForward, Vector3.up);
                    }

                    _cameraRotationOffset = Mathf.Lerp(_cameraRotationOffset, newOffset, t);

                    if (Mathf.Abs(_cameraRotationOffset) > 10)
                    {
                        _isTurningInPlace = true;
                    }
                }
            }
            else
            {
                UpdateStrafeDirection(1f, 0f);
                _cameraRotationOffset = Mathf.Lerp(_cameraRotationOffset, 0f, _rotationSmoothing * Time.deltaTime);

                _shuffleDirectionZ = 1;
                _shuffleDirectionX = 0;

                Vector3 faceDirection = new Vector3(_velocity.x, 0f, _velocity.z);

                if (faceDirection == Vector3.zero)
                {
                    return;
                }

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(faceDirection),
                    _rotationSmoothing * Time.deltaTime
                );
            }
        }

        /// <summary>
        ///     Checks if the player has stopped moving.
        /// </summary>
        private void CheckIfStopped()
        {
            _isStopped = _moveDirection.magnitude == 0 && _speed2D < .5;
        }

        /// <summary>
        ///     Checks if the player has started moving.
        /// </summary>
        private void CheckIfStarting()
        {
            _locomotionStartTimer = VariableOverrideDelayTimer(_locomotionStartTimer);

            bool isStartingCheck = false;

            if (_locomotionStartTimer <= 0.0f)
            {
                if (_moveDirection.magnitude > 0.01 && _speed2D < 1 && !_isStrafing)
                {
                    isStartingCheck = true;
                }

                if (isStartingCheck)
                {
                    if (!_isStarting)
                    {
                        _locomotionStartDirection = _newDirectionDifferenceAngle;
                        _animator.SetFloat(_locomotionStartDirectionHash, _locomotionStartDirection);
                    }

                    float delayTime = 0.2f;
                    _leanDelay = delayTime;
                    _headLookDelay = delayTime;
                    _bodyLookDelay = delayTime;

                    _locomotionStartTimer = delayTime;
                }
            }
            else
            {
                isStartingCheck = true;
            }

            _isStarting = isStartingCheck;
            _animator.SetBool(_isStartingHash, _isStarting);
        }

        /// <summary>
        ///     Updates the strafe direction variables to those provided.
        /// </summary>
        /// <param name="TargetZ">The value to set for Z axis.</param>
        /// <param name="TargetX">The value to set for X axis.</param>
        private void UpdateStrafeDirection(float TargetZ, float TargetX)
        {
            _strafeDirectionZ = Mathf.Lerp(_strafeDirectionZ, TargetZ, _ANIMATION_DAMP_TIME * Time.deltaTime);
            _strafeDirectionX = Mathf.Lerp(_strafeDirectionX, TargetX, _ANIMATION_DAMP_TIME * Time.deltaTime);
            _strafeDirectionZ = Mathf.Round(_strafeDirectionZ * 1000f) / 1000f;
            _strafeDirectionX = Mathf.Round(_strafeDirectionX * 1000f) / 1000f;
        }

        #endregion

        #region Ground Checks

        /// <summary>
        ///     Checks if the character is grounded.
        /// </summary>
        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(
                _controller.transform.position.x,
                _controller.transform.position.y - _groundedOffset,
                _controller.transform.position.z
            );
            _isGrounded = Physics.CheckSphere(spherePosition, _controller.radius, _groundLayerMask, QueryTriggerInteraction.Ignore);

            if (_isGrounded)
            {
                GroundInclineCheck();
            }
        }

        /// <summary>
        ///     Checks for ground incline and sets the required variables.
        /// </summary>
        private void GroundInclineCheck()
        {
            float rayDistance = Mathf.Infinity;
            _rearRayPos.rotation = Quaternion.Euler(transform.rotation.x, 0, 0);
            _frontRayPos.rotation = Quaternion.Euler(transform.rotation.x, 0, 0);

            Physics.Raycast(_rearRayPos.position, _rearRayPos.TransformDirection(-Vector3.up), out RaycastHit rearHit, rayDistance, _groundLayerMask);
            Physics.Raycast(
                _frontRayPos.position,
                _frontRayPos.TransformDirection(-Vector3.up),
                out RaycastHit frontHit,
                rayDistance,
                _groundLayerMask
            );

            Vector3 hitDifference = frontHit.point - rearHit.point;
            float xPlaneLength = new Vector2(hitDifference.x, hitDifference.z).magnitude;

            _inclineAngle = Mathf.Lerp(_inclineAngle, Mathf.Atan2(hitDifference.y, xPlaneLength) * Mathf.Rad2Deg, 20f * Time.deltaTime);
        }

        /// <summary>
        ///     Checks the height of the ceiling above the player to make sure there is room to stand up if crouching.
        /// </summary>
        private void CeilingHeightCheck()
        {
            float rayDistance = Mathf.Infinity;
            float minimumStandingHeight = _capsuleStandingHeight - _frontRayPos.localPosition.y;

            Vector3 midpoint = new Vector3(transform.position.x, transform.position.y + _frontRayPos.localPosition.y, transform.position.z);
            if (Physics.Raycast(midpoint, transform.TransformDirection(Vector3.up), out RaycastHit ceilingHit, rayDistance, _groundLayerMask))
            {
                _cannotStandUp = ceilingHit.distance < minimumStandingHeight;
            }
            else
            {
                _cannotStandUp = false;
            }
        }

        #endregion

        #region Falling

        /// <summary>
        ///     Resets the falling duration variables.
        /// </summary>
        private void ResetFallingDuration()
        {
            _fallStartTime = Time.time;
            _fallingDuration = 0f;
        }

        /// <summary>
        ///     Updates the falling duration variables.
        /// </summary>
        private void UpdateFallingDuration()
        {
            _fallingDuration = Time.time - _fallStartTime;
        }

        #endregion

        #region Checks

        /// <summary>
        ///     Checks if body turns can be enabled, and enabled or disables as required.
        /// </summary>
        private void CheckEnableTurns()
        {
            _headLookDelay = VariableOverrideDelayTimer(_headLookDelay);
            _enableHeadTurn = _headLookDelay == 0.0f && !_isStarting;
            _bodyLookDelay = VariableOverrideDelayTimer(_bodyLookDelay);
            _enableBodyTurn = _bodyLookDelay == 0.0f && !(_isStarting || _isTurningInPlace);
        }

        /// <summary>
        ///     Checks if lean can be enabled, then enabled or disables as required.
        /// </summary>
        private void CheckEnableLean()
        {
            _leanDelay = VariableOverrideDelayTimer(_leanDelay);
            _enableLean = _leanDelay == 0.0f && !(_isStarting || _isTurningInPlace);
        }

        #endregion

        #region Lean and Offsets

        /// <summary>
        ///     Calculates the required rotational additives based on the passed in parameters.
        /// </summary>
        /// <param name="leansActivated">If leans are activated or not.</param>
        /// <param name="headLookActivated">If head look is activated or not.</param>
        /// <param name="bodyLookActivated">If body look is activated or not.</param>
        private void CalculateRotationalAdditives(bool leansActivated, bool headLookActivated, bool bodyLookActivated)
        {
            if (headLookActivated || leansActivated || bodyLookActivated)
            {
                _currentRotation = transform.forward;

                _rotationRate = _currentRotation != _previousRotation
                    ? Vector3.SignedAngle(_currentRotation, _previousRotation, Vector3.up) / Time.deltaTime * -1f
                    : 0f;
            }

            _initialLeanValue = leansActivated ? _rotationRate : 0f;

            float leanSmoothness = 5;
            float maxLeanRotationRate = 275.0f;

            float referenceValue = _speed2D / _sprintSpeed;
            _leanValue = CalculateSmoothedValue(
                _leanValue,
                _initialLeanValue,
                maxLeanRotationRate,
                leanSmoothness,
                _leanCurve,
                referenceValue,
                true
            );

            float headTurnSmoothness = 5f;

            if (headLookActivated && _isTurningInPlace)
            {
                _initialTurnValue = _cameraRotationOffset;
                _headLookX = Mathf.Lerp(_headLookX, _initialTurnValue / 200, 5f * Time.deltaTime);
            }
            else
            {
                _initialTurnValue = headLookActivated ? _rotationRate : 0f;
                _headLookX = CalculateSmoothedValue(
                    _headLookX,
                    _initialTurnValue,
                    maxLeanRotationRate,
                    headTurnSmoothness,
                    _headLookXCurve,
                    _headLookX,
                    false
                );
            }

            float bodyTurnSmoothness = 5f;

            _initialTurnValue = bodyLookActivated ? _rotationRate : 0f;

            _bodyLookX = CalculateSmoothedValue(
                _bodyLookX,
                _initialTurnValue,
                maxLeanRotationRate,
                bodyTurnSmoothness,
                _bodyLookXCurve,
                _bodyLookX,
                false
            );

            float cameraTilt = _cameraController.GetCameraTiltX();
            cameraTilt = (cameraTilt > 180f ? cameraTilt - 360f : cameraTilt) / -180;
            cameraTilt = Mathf.Clamp(cameraTilt, -0.1f, 1.0f);
            _headLookY = cameraTilt;
            _bodyLookY = cameraTilt;

            _previousRotation = _currentRotation;
        }

        /// <summary>
        ///     Calculates a smoothed value between the given variable and target variable, from the given parameters.
        /// </summary>
        /// <param name="mainVariable">The variable to smooth.</param>
        /// <param name="newValue">The target new value.</param>
        /// <param name="maxRateChange">The max rate of change.</param>
        /// <param name="smoothness">The smoothness amount.</param>
        /// <param name="referenceCurve">The reference curve.</param>
        /// <param name="referenceValue">The reference value.</param>
        /// <param name="isMultiplier">If the value is a multiplier or not.</param>
        /// <returns>The smoothed value.</returns>
        private float CalculateSmoothedValue(
            float mainVariable,
            float newValue,
            float maxRateChange,
            float smoothness,
            AnimationCurve referenceCurve,
            float referenceValue,
            bool isMultiplier
        )
        {
            float changeVariable = newValue / maxRateChange;

            changeVariable = Mathf.Clamp(changeVariable, -1.0f, 1.0f);

            if (isMultiplier)
            {
                float multiplier = referenceCurve.Evaluate(referenceValue);
                changeVariable *= multiplier;
            }
            else
            {
                changeVariable = referenceCurve.Evaluate(changeVariable);
            }

            if (!changeVariable.Equals(mainVariable))
            {
                changeVariable = Mathf.Lerp(mainVariable, changeVariable, smoothness * Time.deltaTime);
            }

            return changeVariable;
        }

        /// <summary>
        ///     Provides a clamped override delay to avoid animation transition issues.
        /// </summary>
        /// <param name="timeVariable">The time variable to use.</param>
        /// <returns>A clamped override delay.</returns>
        private float VariableOverrideDelayTimer(float timeVariable)
        {
            if (timeVariable > 0.0f)
            {
                timeVariable -= Time.deltaTime;
                timeVariable = Mathf.Clamp(timeVariable, 0.0f, 1.0f);
            }
            else
            {
                timeVariable = 0.0f;
            }

            return timeVariable;
        }

        #endregion

        #region Lock-on System

        /// <summary>
        ///     Updates and sets the best target for lock on from the list of available targets.
        /// </summary>
        private void UpdateBestTarget()
        {
            GameObject newBestTarget;

            if (_currentTargetCandidates.Count == 0)
            {
                newBestTarget = null;
            }
            else if (_currentTargetCandidates.Count == 1)
            {
                newBestTarget = _currentTargetCandidates[0];
            }
            else
            {
                newBestTarget = null;
                float bestTargetScore = 0f;

                foreach (GameObject target in _currentTargetCandidates)
                {
                    target.GetComponent<SampleObjectLockOn>().Highlight(false, false);

                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    float distanceScore = 1 / distance * 100;

                    Vector3 targetDirection = target.transform.position - _cameraController.GetCameraPosition();
                    float angleInView = Vector3.Dot(targetDirection.normalized, _cameraController.GetCameraForward());
                    float angleScore = angleInView * 40;

                    float totalScore = distanceScore + angleScore;

                    if (totalScore > bestTargetScore)
                    {
                        bestTargetScore = totalScore;
                        newBestTarget = target;
                    }
                }
            }

            if (!_isLockedOn)
            {
                _currentLockOnTarget = newBestTarget;

                if (_currentLockOnTarget != null)
                {
                    _currentLockOnTarget.GetComponent<SampleObjectLockOn>().Highlight(true, false);
                }
            }
            else
            {
                if (_currentTargetCandidates.Contains(_currentLockOnTarget))
                {
                    _currentLockOnTarget.GetComponent<SampleObjectLockOn>().Highlight(true, true);
                }
                else
                {
                    _currentLockOnTarget = newBestTarget;
                    EnableLockOn(false);
                }
            }
        }

        #endregion

        #endregion

        #region Locomotion State

        /// <summary>
        ///     Sets up the locomotion state upon entry.
        /// </summary>
        private void EnterLocomotionState()
        {
            _inputReader.onJumpPerformed += LocomotionToJumpState;
        }

        /// <summary>
        ///     Updates the locomotion state.
        /// </summary>
        private void UpdateLocomotionState()
        {
            UpdateBestTarget();
            GroundedCheck();

            if (!_isGrounded)
            {
                SwitchState(AnimationState.Fall);
            }

            if (_isCrouching)
            {
                SwitchState(AnimationState.Crouch);
            }

            CheckEnableTurns();
            CheckEnableLean();
            CalculateRotationalAdditives(_enableLean, _enableHeadTurn, _enableBodyTurn);

            CalculateMoveDirection();
            CheckIfStarting();
            CheckIfStopped();
            FaceMoveDirection();
            Move();
            UpdateAnimatorController();
        }

        /// <summary>
        ///     Performs the required actions when exiting the locomotion state.
        /// </summary>
        private void ExitLocomotionState()
        {
            _inputReader.onJumpPerformed -= LocomotionToJumpState;
        }

        /// <summary>
        ///     Moves from the locomotion to the jump state.
        /// </summary>
        private void LocomotionToJumpState()
        {
            SwitchState(AnimationState.Jump);
        }

        #endregion

        #region Jump State

        /// <summary>
        ///     Sets up the Jump state upon entry.
        /// </summary>
        private void EnterJumpState()
        {
            _animator.SetBool(_isJumpingAnimHash, true);

            _isSliding = false;

            _velocity = new Vector3(_velocity.x, _jumpForce, _velocity.z);
        }

        /// <summary>
        ///     updates the jump state.
        /// </summary>
        private void UpdateJumpState()
        {
            UpdateBestTarget();
            ApplyGravity();

            if (_velocity.y <= 0f)
            {
                _animator.SetBool(_isJumpingAnimHash, false);
                SwitchState(AnimationState.Fall);
            }

            GroundedCheck();

            CalculateRotationalAdditives(false, _enableHeadTurn, _enableBodyTurn);
            CalculateMoveDirection();
            FaceMoveDirection();
            Move();
            UpdateAnimatorController();
        }

        /// <summary>
        ///     Performs the required actions upon exiting the jump state.
        /// </summary>
        private void ExitJumpState()
        {
            _animator.SetBool(_isJumpingAnimHash, false);
        }

        #endregion

        #region Fall State

        /// <summary>
        ///     Sets up the fall state upon entry.
        /// </summary>
        private void EnterFallState()
        {
            ResetFallingDuration();
            _velocity.y = 0f;

            DeactivateCrouch();
            _isSliding = false;
        }

        /// <summary>
        ///     Updates the fall state.
        /// </summary>
        private void UpdateFallState()
        {
            UpdateBestTarget();
            GroundedCheck();

            CalculateRotationalAdditives(false, _enableHeadTurn, _enableBodyTurn);

            CalculateMoveDirection();
            FaceMoveDirection();

            ApplyGravity();
            Move();
            UpdateAnimatorController();

            if (_controller.isGrounded)
            {
                SwitchState(AnimationState.Locomotion);
            }

            UpdateFallingDuration();
        }

        #endregion

        #region Crouch State

        /// <summary>
        ///     Sets up the crouch state upon entry.
        /// </summary>
        private void EnterCrouchState()
        {
            _inputReader.onJumpPerformed += CrouchToJumpState;
        }

        /// <summary>
        ///     Updates the crouch state.
        /// </summary>
        private void UpdateCrouchState()
        {
            UpdateBestTarget();

            GroundedCheck();
            if (!_isGrounded)
            {
                DeactivateCrouch();
                CapsuleCrouchingSize(false);
                SwitchState(AnimationState.Fall);
            }

            CeilingHeightCheck();

            if (!_crouchKeyPressed && !_cannotStandUp)
            {
                DeactivateCrouch();
                SwitchToLocomotionState();
            }

            if (!_isCrouching)
            {
                CapsuleCrouchingSize(false);
                SwitchToLocomotionState();
            }

            CheckEnableTurns();
            CheckEnableLean();

            CalculateRotationalAdditives(false, _enableHeadTurn, false);

            CalculateMoveDirection();
            CheckIfStarting();
            CheckIfStopped();

            FaceMoveDirection();
            Move();
            UpdateAnimatorController();
        }

        /// <summary>
        ///     Performs the required actions upon exiting the crouch state.
        /// </summary>
        private void ExitCrouchState()
        {
            _inputReader.onJumpPerformed -= CrouchToJumpState;
        }

        /// <summary>
        ///     Moves from the crouch state to the jump state.
        /// </summary>
        private void CrouchToJumpState()
        {
            if (!_cannotStandUp)
            {
                DeactivateCrouch();
                SwitchState(AnimationState.Jump);
            }
        }

        /// <summary>
        ///     Moves from the crouch state to the locomotion state.
        /// </summary>
        private void SwitchToLocomotionState()
        {
            DeactivateCrouch();
            SwitchState(AnimationState.Locomotion);
        }

        #endregion
    }
}
