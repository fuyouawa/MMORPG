using MMORPG;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour, INetworkEntityCallbacks, IController
{
    [SerializeField]
    [ReadOnly]
    private Vector2 _moveAxis;

    private Animator _animator;
    private GameInputControls _inputControls;

    private AnimatorParam<bool> _animatorParamWalking;
    //private AnimatorParam<bool> _animatorParamMoveForward;
    //private AnimatorParam<bool> _animatorParamMoveForwardLeft;
    //private AnimatorParam<bool> _animatorParamMoveForwardRight;
    //private AnimatorParam<bool> _animatorParamMoveBackward;
    //private AnimatorParam<bool> _animatorParamMoveBackwardLeft;
    //private AnimatorParam<bool> _animatorParamMoveBackwardRight;
    //private AnimatorParam<bool> _animatorParamMoveLeft;
    //private AnimatorParam<bool> _animatorParamMoveRight;
    private AnimatorParam<float> _animatorParamHoriAxis;
    private AnimatorParam<float> _animatorParamVertAxis;

    private void Awake()
    {
        _inputControls = new();
        _animator = GetComponent<Animator>();
        _animatorParamWalking = new(_animator, "Walking");
        //_animatorParamMoveForward = new(_animator, "MoveForward");
        //_animatorParamMoveForwardLeft = new(_animator, "MoveForwardLeft");
        //_animatorParamMoveForwardRight = new(_animator, "MoveForwardRight");
        //_animatorParamMoveBackward = new(_animator, "MoveBackward");
        //_animatorParamMoveBackwardLeft = new(_animator, "MoveBackwardLeft");
        //_animatorParamMoveBackwardRight = new(_animator, "MoveBackwardRight");
        //_animatorParamMoveLeft = new(_animator, "MoveLeft");
        //_animatorParamMoveRight = new(_animator, "MoveRight");
        _animatorParamHoriAxis = new(_animator, "HoriAxis");
        _animatorParamVertAxis = new(_animator, "VertAxis");
    }

    public void NetworkControlFixedUpdate(NetworkControlData data)
    {
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        _moveAxis = _inputControls.Player.Move.ReadValue<Vector2>();

        _animatorParamWalking.Value = Mathf.Approximately(_moveAxis.x, 1f) || Mathf.Approximately(_moveAxis.y, 1f);

        _animatorParamHoriAxis.Value = _moveAxis.x;
        _animatorParamVertAxis.Value = _moveAxis.y;
    }

    public void NetworkSyncUpdate(NetworkSyncData data)
    {
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
