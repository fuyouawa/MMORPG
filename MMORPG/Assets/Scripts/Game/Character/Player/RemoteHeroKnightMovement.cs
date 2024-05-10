using MessagePack;
using UnityEngine;

namespace MMORPG.Game
{
    public class RemoteHeroKnightMovement : RemotePlayerAbility
    {
        private Vector3 _targetSyncPosition;
        private Quaternion _targetSyncRotation;
        private Vector2 _moveDirection;

        public override void OnStateEnter()
        {
            _targetSyncPosition = transform.position;
            _targetSyncRotation = transform.rotation;
            Brain.AnimationController.DisableAnimatorMove();
            Brain.AnimationController.Movement = true;
        }

        public override void OnStateUpdate()
        {
            SyncMove();
        }

        public override void OnStateNetworkSyncTransform(EntityTransformSyncData data)
        {
            var d = MessagePackSerializer.Deserialize<WalkStateSyncData>(data.Data);
            _moveDirection = d.MoveDirection;
            _targetSyncPosition = data.Position;
            _targetSyncRotation = data.Rotation;

            Brain.AnimationController.SmoothMoveDirection(_moveDirection);
        }

        public override void OnStateExit()
        {
            Brain.AnimationController.Movement = false;
        }

        private void SyncMove()
        {
            Brain.CharacterController.SmoothMove(_targetSyncPosition);
            Brain.CharacterController.SmoothRotate(_targetSyncRotation);
        }
    }
}

