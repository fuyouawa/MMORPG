using MessagePack;
using UnityEngine;

public class RemoteHeroKnightWalking : RemotePlayerAbility, IAnimatorAutoUpdateParams
{
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

        _targetSyncPosition = transform.position;
        _targetSyncRotation = transform.rotation;
        Brain.CharacterController.AnimationController.DisableAnimatorMove();
    }

    public override void OnStateUpdate()
    {
        SyncMove();
        UpdateAnimation();
    }

    public override void OnStateNetworkSyncTransform(EntityTransformSyncData data)
    {
        var d = MessagePackSerializer.Deserialize<WalkStateSyncData>(data.Data);
        _moveDirection = d.MoveDirection;
        _targetSyncPosition = data.Position;
        _targetSyncRotation = data.Rotation;
    }

    public override void OnStateExit()
    {
        Walking = false;
        HoriMovementNormalized = 0;
        VertMovementNormalized = 0;
    }
    private void SyncMove()
    {
        Brain.CharacterController.SmoothMove(_targetSyncPosition);
        Brain.CharacterController.SmoothRotate(_targetSyncRotation);
    }

    private void UpdateAnimation()
    {
        var acc = 3f * Time.deltaTime;
        HoriMovementNormalized = Mathf.MoveTowards(HoriMovementNormalized, _moveDirection.x, acc);
        VertMovementNormalized = Mathf.MoveTowards(VertMovementNormalized, _moveDirection.y, acc);
    }
}

