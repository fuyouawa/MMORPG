using UnityEngine;

public class HeroKnightWalking : PlayerAbility, IAnimatorAutoUpdateParams
{
    public float IdleThreshold = 0.05f;

    [Range(0.1f, 100f)]
    public float Acceleration = 3;

    [AnimatorParam]
    public bool Walking { get; set; }
    [AnimatorParam]
    public float HoriMovementNormalized { get; set; }
    [AnimatorParam]
    public float VertMovementNormalized { get; set; }

    private Vector3 _targetSyncPosition;
    private Quaternion _targetSyncRotation;
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

        if (IsMine)
        {
            Brain.CharacterController.AnimationController.EnableAnimatorMove();
        }
        else
        {
            _targetSyncPosition = transform.position;
            _targetSyncRotation = transform.rotation;
            Brain.CharacterController.AnimationController.DisableAnimatorMove();
        }
    }

    public override void OnStateUpdate()
    {
        if (IsMine)
        {
            _moveDirection = Brain.CurrentMovementDirection;
            ForwardCamera();
        }
        else
        {
            //TODO 动画同步moveDirection
            Brain.CharacterController.SmoothMove(_targetSyncPosition);
            Brain.CharacterController.SmoothRotate(_targetSyncRotation);
        }
        var acc = Acceleration * Time.deltaTime;
        HoriMovementNormalized = Mathf.MoveTowards(HoriMovementNormalized, _moveDirection.x, acc);
        VertMovementNormalized = Mathf.MoveTowards(VertMovementNormalized, _moveDirection.y, acc);
    }

    public override void OnStateNetworkFixedUpdate()
    {
        if (IsMine)
        {
            Brain.CharacterController.NetworkUploadTransform(OwnerStateId, null);
        }
    }

    public override void OnStateNetworkSyncTransform(EntityTransformSyncData data)
    {
        _targetSyncPosition = data.Position;
        _targetSyncRotation = data.Rotation;
    }

    public override void OnStateExit()
    {
        Walking = false;
        HoriMovementNormalized = 0;
        VertMovementNormalized = 0;
        if (!IsMine)
        {
            Brain.CharacterController.Rigidbody.linearVelocity = Vector3.zero;
        }
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

    private void ForwardCamera()
    {
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        Brain.CharacterController.SmoothRotate(targetRotation);
    }
}

