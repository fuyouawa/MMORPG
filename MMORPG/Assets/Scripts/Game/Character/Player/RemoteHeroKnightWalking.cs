using MessagePack;
using UnityEngine;

public class RemoteHeroKnightWalking : RemotePlayerAbility
{
    private WalkAnimParams _walkAnimParams = new();

    private Vector3 _targetSyncPosition;
    private Quaternion _targetSyncRotation;
    private Vector2 _moveDirection;

    private AnimatorMachine _animatorMachine;

    public override void OnStateInit()
    {
        _animatorMachine = new(_walkAnimParams, gameObject, Brain.CharacterController.Animator);
    }

    public override void OnStateEnter()
    {
        _walkAnimParams.Enter();

        _targetSyncPosition = transform.position;
        _targetSyncRotation = transform.rotation;
        Brain.CharacterController.AnimationController.DisableAnimatorMove();

        _animatorMachine.Run();
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
        _walkAnimParams.Exit();
        _animatorMachine.StopInNextFrame();
    }
    private void SyncMove()
    {
        Brain.CharacterController.SmoothMove(_targetSyncPosition);
        Brain.CharacterController.SmoothRotate(_targetSyncRotation);
    }

    private void UpdateAnimation()
    {
        _walkAnimParams.SmoothToDirection(_moveDirection);
    }
}

