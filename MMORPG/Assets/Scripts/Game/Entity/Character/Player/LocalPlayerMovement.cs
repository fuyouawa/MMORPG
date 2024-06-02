using MessagePack;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    [MessagePackObject]
    public class WalkStateSyncData
    {
        [Key(0)]
        public Vector2 MoveDirection { get; set; }
        [Key(1)]
        public bool IsWalkingOrRunning { get; set; }
    }

    public class LocalPlayerMovement : LocalPlayerAbility
    {
        public float BackIdleThreshold = 0.5f;

        private Vector2 _moveDirection;

        public override void OnStateInit()
        {
            Brain.AnimationController.EnableAnimatorMove();
        }

        public override void OnStateEnter()
        {
        }

        public override void OnStateUpdate()
        {
            ControlMove();
        }

        public override void OnStateNetworkFixedUpdate()
        {
            var d = new WalkStateSyncData()
            {
                MoveDirection = _moveDirection,
                IsWalkingOrRunning = Brain.AnimationController.Walking
            };
            Brain.NetworkUploadTransform(OwnerStateId, MessagePackSerializer.Serialize(d));
        }

        public override void OnStateExit()
        {
        }

        [StateCondition]
        public bool PressingMove()
        {
            return Brain.GetMoveInput().magnitude > 0.5f;
        }

        [StateCondition]
        public bool BackIdleVelocity()
        {
            return Brain.AnimationController.MovementDirection.magnitude < BackIdleThreshold;
        }

        private void ControlMove()
        {
            if (Brain.IsPressingRun())
            {
                if (!Brain.AnimationController.Running)
                {
                    Brain.AnimationController.StartRunning();
                }
            }
            else
            {
                if (!Brain.AnimationController.Walking)
                {
                    Brain.AnimationController.StartWalking();
                }
            }

            _moveDirection = Brain.GetMoveInput();
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

}
