using MMORPG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKnight : MonoBehaviour, INetworkEntityCallbacks
{
    private PlayerAnimatorController _animatorController;
    private GameInputControls _inputControls;
    private Vector2 _moveInput;
    private Entity _entity;

    void Awake()
    {
        _inputControls = new();
        _entity = GetComponent<Entity>();
        _animatorController = GetComponentInChildren<PlayerAnimatorController>();
    }

    private void Start()
    {
        _animatorController.Setup(transform, _entity.IsMine);
    }

    void INetworkEntityCallbacks.NetworkMineUpdate()
    {
        _moveInput = _inputControls.Player.Move.ReadValue<Vector2>();
        _animatorController.SetMovement(_moveInput);
    }

    private void OnEnable()
    {
        _inputControls.Enable();
    }

    private void OnDisable()
    {
        _inputControls.Disable();
    }
}
