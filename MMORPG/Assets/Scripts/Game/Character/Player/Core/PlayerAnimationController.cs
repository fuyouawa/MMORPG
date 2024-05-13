using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public GameObject Owner;

        [Title("Move Switch")]
        public float MoveSwitchVelocity = 3f;
        public float MoveSwitchAcceleration = 3f;

        private Animator _animator;
        private bool _animatorMove = true;

        [AnimatorParam]
        public bool Movement { get; set; }

        [AnimatorParam]
        public float HoriMovementDirection { get; set; }
        [AnimatorParam]
        public float VertMovementDirection { get; set; }

        public Vector2 MovementDirection
        {
            get => new(HoriMovementDirection, VertMovementDirection);
            set
            {
                HoriMovementDirection = value.x;
                VertMovementDirection = value.y;
            }
        }

        private Vector2 _targetMoveDirection;

        private AnimatorMachine _machine;

        private float _currentMoveSwitchVelocity;

        private static readonly float s_moveExtraRectCoefficient = 0.7f;
        private static readonly float s_moveExtraMaxAcceleration = 6;

        private static readonly Rect s_moveExtraAccelerationRect = new(
            -s_moveExtraRectCoefficient,
            -s_moveExtraRectCoefficient,
            s_moveExtraRectCoefficient * 2,
            s_moveExtraRectCoefficient * 2);

        private static float GetExtraAccelerationCoefficient(Vector2 pos)
        {
            if (pos.x.Abs() < 0.01f && pos.y.Abs() < 0.01f)
                return s_moveExtraMaxAcceleration;
            if (!s_moveExtraAccelerationRect.Contains(pos))
                return 0;
            var stdPos = pos * (1 / s_moveExtraRectCoefficient);

            var E = new Vector2((stdPos.x / stdPos.y).Abs(), 1);
            var radio = 1 / E.magnitude;
            var len = stdPos.magnitude * radio;

            return (1 - len) * s_moveExtraMaxAcceleration;
        }

        public void SmoothMoveDirection(Vector2 dir)
        {
            _targetMoveDirection = dir;
        }


        void Awake()
        {
            _animator = GetComponent<Animator>();
            _machine = new(this, gameObject, _animator);
            _machine.Run();
            _currentMoveSwitchVelocity = MoveSwitchVelocity;
        }

        void Update()
        {
            Moving();
        }

        public void EnableAnimatorMove()
        {
            _animatorMove = true;
        }

        public void DisableAnimatorMove()
        {
            _animatorMove = false;
        }

        private void Moving()
        {
            if (Vector2.Distance(MovementDirection, _targetMoveDirection) > 0.1f)
            {
                var targetMoveDir = _targetMoveDirection;
                if (_targetMoveDirection.y * VertMovementDirection < -0.1f ||
                     _targetMoveDirection.x * HoriMovementDirection < -0.1f)
                {
                    targetMoveDir = Vector2.zero;
                }

                _currentMoveSwitchVelocity += Time.deltaTime * MoveSwitchAcceleration;
                var velocity = MoveSwitchVelocity + GetExtraAccelerationCoefficient(MovementDirection);
                MovementDirection = Vector2.MoveTowards(MovementDirection, targetMoveDir, velocity * Time.deltaTime);
            }
            else
            {
                MovementDirection = _targetMoveDirection;
                _currentMoveSwitchVelocity = MoveSwitchVelocity;
            }
        }

        private void OnAnimatorMove()
        {
            if (_animatorMove)
            {
                Owner.transform.position += _animator.deltaPosition;
                Owner.transform.rotation *= _animator.deltaRotation;
            }
        }
    }

}
