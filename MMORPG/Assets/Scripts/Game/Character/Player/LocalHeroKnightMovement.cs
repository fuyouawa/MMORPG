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

public class LocalHeroKnightMovement : LocalPlayerAbility
{
    public float IdleThreshold = 0.05f;

    private Vector2 _moveDirection;

    public override void OnStateInit()
    {
    }

    public override void OnStateEnter()
    {
        Brain.AnimationController.EnableAnimatorMove();
        Brain.AnimationController.Movement = true;
    }

    public override void OnStateUpdate()
    {
        ControlMove();
    }

    public override void OnStateNetworkFixedUpdate()
    {
        var d = new WalkStateSyncData() { MoveDirection = _moveDirection };
        Brain.CharacterController.NetworkUploadTransform(OwnerStateId, MessagePackSerializer.Serialize(d));
    }

    public override void OnStateExit()
    {
        Brain.AnimationController.Movement = false;
    }

    [StateCondition]
    public bool ReachIdleThreshold()
    {
        return Brain.CurrentMovementDirection.magnitude > IdleThreshold;
    }

    [StateCondition]
    public bool FinishInertia()
    {
        return Brain.AnimationController.MovementDirection.magnitude < IdleThreshold;
    }

    private void ControlMove()
    {
        _moveDirection = Brain.CurrentMovementDirection;
        if (Brain.InputControls.Player.Run.inProgress)
        {
            _moveDirection *= 2;
        }
        Brain.AnimationController.SmoothMoveDirection(_moveDirection);
        ForwardCamera();
    }

    private void ForwardCamera()
    {
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        Brain.CharacterController.SmoothRotate(targetRotation);
    }
}

