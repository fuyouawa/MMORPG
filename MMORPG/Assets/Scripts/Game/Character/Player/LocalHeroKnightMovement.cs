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
    }

    public class LocalHeroKnightMovement : LocalPlayerAbility
    {
        public float IdleThreshold = 0.05f;
        public float BackIdleThreshold = 0.5f;

        private Vector2 _moveDirection;
        private bool _prevMoveForward;
        private bool? _forwardSidle;

        private Vector2 InputDirection => Brain.InputControls.Player.Move.ReadValue<Vector2>();

        public override void OnStateInit()
        {
        }

        public override void OnStateEnter()
        {
            Brain.AnimationController.EnableAnimatorMove();
            Brain.AnimationController.Movement = true;
            _forwardSidle = false;
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
            return InputDirection.magnitude > IdleThreshold;
        }

        [StateCondition]
        public bool ReachBackIdleThreshold()
        {
            return Brain.AnimationController.MovementDirection.magnitude < BackIdleThreshold;
        }

        private void ControlMove()
        {
            var dir = InputDirection;
            TransformMoveDirection(dir);
            if (_prevMoveForward)
            {
                if (dir.y < -0.5f)
                {
                    _prevMoveForward = false;
                }
            }
            else
            {
                if (dir.y > 0.5f)
                {
                    _prevMoveForward = true;
                }
            }
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

        private void TransformMoveDirection(Vector2 dir)
        {
            _moveDirection = dir;

            var xMove = !Mathf.Approximately(dir.x, 0);
            var yMove = !Mathf.Approximately(dir.y, 0);

            if ((!xMove && yMove) || (xMove && yMove))
            {
                _forwardSidle = null;
                return;
            }

            _forwardSidle ??= _prevMoveForward;

            _moveDirection.y = _forwardSidle == true ? 0.33f : -0.33f;
        }
    }

}
