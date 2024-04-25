using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour, INetworkEntityCallbacks, IController
{
    [Range(0.1f, 100f)]
    public float Acceleration = 3;

    private Vector2 _moveAxis;
    private Vector2 _velocity;

    private Animator _animator;
    private GameInputControls _inputControls;

    private AnimatorParam<bool> _animParamWalking;
    private AnimatorParam<float> _animParamHoriSpeedNormalized;
    private AnimatorParam<float> _animParamVertSpeedNormalized;

    private void Awake()
    {
        _inputControls = new();
        _animator = GetComponent<Animator>();
        _animParamWalking = new(_animator, "Walking");
        _animParamHoriSpeedNormalized = new(_animator, "HoriSpeedNormalized");
        _animParamVertSpeedNormalized = new(_animator, "VertSpeedNormalized");
    }

    void INetworkEntityCallbacks.NetworkMineUpdate()
    {
        _moveAxis = _inputControls.Player.Move.ReadValue<Vector2>();
        _velocity = Vector2.MoveTowards(_velocity, _moveAxis, Acceleration * Time.deltaTime);
        _animParamWalking.Value = _moveAxis.sqrMagnitude > 0.5f || _velocity.sqrMagnitude > 0.5f;
        
        _animParamHoriSpeedNormalized.Value = _velocity.x;
        _animParamVertSpeedNormalized.Value = _velocity.y;
        if (!(_moveAxis.sqrMagnitude > 0.5f)) return;
        // var cameraForward = Camera.main.transform.forward;
        // cameraForward.y = 0;
        // var moveDirection = new Vector3(_moveAxis.x, 0f, _moveAxis.y).normalized;
        // var targetDirection = Quaternion.LookRotation(cameraForward) * moveDirection;
        // var targetRotation = Quaternion.LookRotation(targetDirection.normalized, Vector3.up);
        // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);
    }
    private void OnEnable()
    {
        _inputControls.Enable();
    }

    private void OnDisable()
    {
        _inputControls.Disable();
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
