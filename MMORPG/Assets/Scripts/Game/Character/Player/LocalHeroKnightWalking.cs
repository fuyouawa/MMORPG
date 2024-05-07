using MessagePack;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

[MessagePackObject]
public class WalkStateSyncData
{
    [Key(0)]
    public Vector2 MoveDirection { get; set; }
}

public class WalkAnimParams
{
    [AnimatorParam]
    public bool Walking { get; set; }
    [AnimatorParam]
    public float HoriMovementNormalized { get; set; }
    [AnimatorParam]
    public float VertMovementNormalized { get; set; }

    public void Enter()
    {
        Walking = true;
        HoriMovementNormalized = 0f;
        VertMovementNormalized = 0f;
    }

    public void Exit()
    {
        Walking = false;
        HoriMovementNormalized = 0f;
        VertMovementNormalized = 0f;
    }

    public void SmoothToDirection(Vector2 moveDirection)
    {
        var acc = 3f * Time.deltaTime;
        HoriMovementNormalized = Mathf.MoveTowards(HoriMovementNormalized, moveDirection.x, acc);
        VertMovementNormalized = Mathf.MoveTowards(VertMovementNormalized, moveDirection.y, acc);
    }
}


public class LocalHeroKnightWalking : LocalPlayerAbility
{
    public float IdleThreshold = 0.05f;

    private WalkAnimParams _walkAnimParams = new();

    private Vector2 _moveDirection;
    private AnimatorMachine _animatorMachine;

    public override void OnStateInit()
    {
        _animatorMachine = new(_walkAnimParams, gameObject, Brain.CharacterController.Animator);
    }

    public override void OnStateEnter()
    {
        _walkAnimParams.Enter();

        Brain.CharacterController.AnimationController.EnableAnimatorMove();
        _animatorMachine.Run();
    }

    public override void OnStateUpdate()
    {
        ControlMove();
        UpdateAnimation();
    }

    public override void OnStateNetworkFixedUpdate()
    {
        var d = new WalkStateSyncData() { MoveDirection = _moveDirection };
        Brain.CharacterController.NetworkUploadTransform(OwnerStateId, MessagePackSerializer.Serialize(d));
    }

    public override void OnStateExit()
    {
        _walkAnimParams.Exit();
        _animatorMachine.StopInNextFrame();
    }

    [StateCondition]
    public bool ReachIdleThreshold()
    {
        return Brain.CurrentMovementDirection.magnitude > IdleThreshold;
    }

    [StateCondition]
    public bool FinishInertia()
    {
        return new Vector2(_walkAnimParams.HoriMovementNormalized, _walkAnimParams.VertMovementNormalized).magnitude < IdleThreshold;
    }

    private void ControlMove()
    {
        _moveDirection = Brain.CurrentMovementDirection;
        ForwardCamera();
    }

    private void ForwardCamera()
    {
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        Brain.CharacterController.SmoothRotate(targetRotation);
    }

    private void UpdateAnimation()
    {
        var acc = 3f * Time.deltaTime;
        _walkAnimParams.SmoothToDirection(_moveDirection);
    }
}

