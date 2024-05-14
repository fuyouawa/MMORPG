using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public PlayerBrain Brain { get; set; }

        [Title("Move Switch")]
        public float MoveSwitchVelocity = 3f;
        public float MoveSwitchAcceleration = 3f;

        private Animator _animator;
        private bool _animatorMove = true;

        [AnimatorParam]
        public bool Movement { get; set; }

        [AnimatorParam]
        public bool Running { get; set; }

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

        private static readonly float s_moveExtraMaxAcceleration = 6;
        private static readonly float s_runExtraMaxAcceleration = 12;

        private static readonly Circle s_moveExtraAccelerationArea = new(0.7f);

        private static readonly Circle s_runExtraAccelerationArea = new(1.4f);

        private float GetExtraAccelerationCoefficient(Vector2 pos)
        {
            var maxAcceleration = Running ? s_runExtraMaxAcceleration : s_moveExtraMaxAcceleration;
            var accelerationRect = Running ? s_moveExtraAccelerationArea : s_runExtraAccelerationArea;
            var bound = Running ? 2 : 1;

            if (pos.x.Abs() < 0.01f && pos.y.Abs() < 0.01f)
                return maxAcceleration;
            if (!accelerationRect.Contains(pos))
                return 0;
            var stdPos = pos * (bound / accelerationRect.Radius);

            // var E = new Vector2((stdPos.x / stdPos.y).Abs(), 1);
            // var radio = 1 / E.magnitude;
            // var len = stdPos.magnitude * radio;
            var len = stdPos.magnitude;

            return (1 - len) * maxAcceleration;
        }

        public void SmoothMoveDirection(Vector2 dir)
        {
            _targetMoveDirection = dir;
        }


        private Transform _playerMoveAnimatorBlendTreePoint;
        private RectTransform _playerMoveAnimatorBlendTreeViewAccelerationArea;
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _machine = new(this, gameObject, _animator);
            _machine.Run();
        }

        void Start()
        {
            if (Brain.IsMine)
            {
                _playerMoveAnimatorBlendTreePoint = GameObject.Find("PlayerMoveAnimatorBlendTreePoint").transform;
                _playerMoveAnimatorBlendTreeViewAccelerationArea = GameObject.Find("PlayerMoveAnimatorBlendTreeViewAccelerationArea").GetComponent<RectTransform>();
            }
        }

        void Update()
        {
            Moving();
            if (Brain.IsMine)
            {
                var influence = (Running ? s_runExtraAccelerationArea.Radius : s_moveExtraAccelerationArea.Radius);
                _playerMoveAnimatorBlendTreePoint.localPosition = MovementDirection * 25;
                _playerMoveAnimatorBlendTreeViewAccelerationArea.sizeDelta = new Vector2(50, 50) * influence;
            }
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

                var velocity = MoveSwitchVelocity + GetExtraAccelerationCoefficient(MovementDirection);
                MovementDirection = Vector2.MoveTowards(MovementDirection, targetMoveDir, velocity * Time.deltaTime);
            }
            else
            {
                MovementDirection = _targetMoveDirection;
            }
        }

        private void OnAnimatorMove()
        {
            if (_animatorMove)
            {
                Brain.CharacterController.MoveDirection(_animator.deltaPosition);
                Brain.CharacterController.RelativeRotate(_animator.deltaRotation);
            }
        }
    }
}
