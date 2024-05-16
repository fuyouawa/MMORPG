using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public PlayerBrain Brain { get; private set; }

        [Title("Move Switch")]
        public float MoveSwitchVelocity = 3f;

        [Title("Animation Speed")]

        [ShowInInspector]
        [AnimatorStateSpeed("Walking")]
        public float WalkSpeed { get; set; } = 1f;

        [ShowInInspector]
        [AnimatorStateSpeed("Running")]
        public float RunSpeed { get; set; } = 1.3f;

        [AnimatorParam]
        public bool Walking { get; set; }

        [AnimatorParam]
        public bool Running { get; set; }

        [AnimatorParam]
        public bool Moving => Walking || Running;

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
        public Animator Animator { get; private set; }
        private bool _animatorMove = true;


        private static readonly float s_moveExtraMaxAcceleration = 9;

        private static readonly Circle s_moveExtraAccelerationArea = new(0.8f);

        private float GetMoveExtraAcceleration(Vector2 point)
        {
            if (point.x.Abs() < 0.01f && point.y.Abs() < 0.01f)
                return s_moveExtraMaxAcceleration;
            if (!s_moveExtraAccelerationArea.Contains(point))
                return 0;
            var stdPos = point * (1 / s_moveExtraAccelerationArea.Radius);

            // var E = new Vector2((stdPos.x / stdPos.y).Abs(), 1);
            // var radio = 1 / E.magnitude;
            // var len = stdPos.magnitude * radio;
            var len = stdPos.magnitude;

            return (1 - len) * s_moveExtraMaxAcceleration;
        }

        public void SmoothMoveDirection(Vector2 dir)
        {
            _targetMoveDirection = dir;
        }

        public void Setup(PlayerBrain brain)
        {
            Brain = brain;
        }


        void Awake()
        {
            Animator = GetComponent<Animator>();
            _machine = new(this, gameObject, Animator);
            _machine.Run();
        }

        void Update()
        {
            
            Move();
        }

        private void Move()
        {
            var velocity = MoveSwitchVelocity + GetMoveExtraAcceleration(MovementDirection);
            MovementDirection = Vector2.MoveTowards(MovementDirection, _targetMoveDirection, velocity * Time.deltaTime);
        }

        public void EnableAnimatorMove()
        {
            _animatorMove = true;
        }

        public void DisableAnimatorMove()
        {
            _animatorMove = false;
        }

        public void StartWalking()
        {
            Walking = true;
            Running = false;
        }


        public void StartRunning()
        {
            Walking = false;
            Running = true;
        }

        public void StopMovement()
        {
            Walking = false;
            Running = false;
            MovementDirection = Vector2.zero;
        }

        private void OnAnimatorMove()
        {
            if (_animatorMove)
            {
                Brain.CharacterController.MoveDirection(Animator.deltaPosition);
                Brain.CharacterController.RelativeRotate(Animator.deltaRotation);
            }
        }
    }
}
