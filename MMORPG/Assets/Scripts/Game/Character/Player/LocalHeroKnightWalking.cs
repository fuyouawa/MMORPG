using MessagePack;
using UnityEngine;
using UnityEngine.EventSystems;

[MessagePackObject]
public struct WalkStateSyncData
{
    [Key(0)]
    public Vector2 MoveDirection;
}

public class LocalHeroKnightWalking : LocalPlayerAbility, IAnimatorAutoUpdateParams
{
    public float IdleThreshold = 0.05f;

    [AnimatorParam]
    public bool Walking { get; set; }
    [AnimatorParam]
    public float HoriMovementNormalized { get; set; }
    [AnimatorParam]
    public float VertMovementNormalized { get; set; }

    private Vector2 _moveDirection;

    public override void OnStateInit()
    {
        this.StartAnimatorAutoUpdate(gameObject, Brain.CharacterController.Animator);
    }

    public override void OnStateEnter()
    {
        Walking = true;
        HoriMovementNormalized = 0;
        VertMovementNormalized = 0;

        Brain.CharacterController.AnimationController.EnableAnimatorMove();
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
        Walking = false;
        HoriMovementNormalized = 0;
        VertMovementNormalized = 0;
    }

    [StateCondition]
    public bool ReachIdleThreshold()
    {
        return Brain.CurrentMovementDirection.magnitude > IdleThreshold;
    }

    [StateCondition]
    public bool FinishInertia()
    {
        return new Vector2(HoriMovementNormalized, VertMovementNormalized).magnitude < IdleThreshold;
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
        HoriMovementNormalized = Mathf.MoveTowards(HoriMovementNormalized, _moveDirection.x, acc);
        VertMovementNormalized = Mathf.MoveTowards(VertMovementNormalized, _moveDirection.y, acc);
    }
}

