using MMORPG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKnight : MonoBehaviour, INetworkEntityCallbacks
{
    [AnimatorParam]
    [ReadOnly]
    public bool Walking;
    [AnimatorParam]
    [ReadOnly]
    public float HoriSpeedNormalized;
    [AnimatorParam]
    [ReadOnly]
    public float VertSpeedNormalized;

    [Range(0.1f, 100f)]
    public float Acceleration = 3;

    private AnimatorMachine _animatorMachine;
    [SerializeField]
    [ReadOnly]
    private Vector2 _moveAxis;
    private GameInputControls _inputControls;
    private Entity _entity;

    void Awake()
    {
        _inputControls = new();
        _animatorMachine = new(this, GetComponent<Animator>());
        _entity = GetComponent<Entity>();
    }

    private void Update()
    {
        _animatorMachine.UpdateAnimator();
    }

    void INetworkEntityCallbacks.NetworkMineUpdate()
    {
        _moveAxis = _inputControls.Player.Move.ReadValue<Vector2>();
        var speedNormalized = Vector2.MoveTowards(
            new Vector2(HoriSpeedNormalized, VertSpeedNormalized),
            _moveAxis,
            Acceleration * Time.deltaTime);

        Walking = _moveAxis.sqrMagnitude > 0.5f || speedNormalized.sqrMagnitude > 0.5f;
        HoriSpeedNormalized = speedNormalized.x;
        VertSpeedNormalized = speedNormalized.y;

        if (!(_moveAxis.sqrMagnitude > 0.5f)) return;
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);
    }

    private void OnEnable()
    {
        _inputControls.Enable();
    }

    private void OnDisable()
    {
        _inputControls.Disable();
    }

    //private void OnAnimatorMove()
    //{
    //    if (_entity.IsMine)
    //    {
    //        transform.position = _animatorMachine.Animator.deltaPosition;
    //        transform.rotation = _animatorMachine.Animator.deltaRotation;
    //    }
    //}
}
