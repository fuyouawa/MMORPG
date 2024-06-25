using System.Numerics;
using GameServer.EntitySystem;
using GameServer.Tool;
using MMORPG.Common.Proto.Entity;

namespace GameServer.AiSystem.Ability
{
    public class MoveAbility : Ability
    {
        public Entity Entity { get; }
        public float Speed { get; set; }
        public Vector2 Velocity { get; private set; }
        public bool Moving => Velocity.Length() > 0.1f;
        public Entity? LookAtTarget { get; set; }
        public bool LookAtMoveDirection { get; set; } = true;
        public bool LockDirection { get; set; }

        private Vector2 _force;
        private Vector2 _destination;
        private Vector2 _lastPosition;

        public MoveAbility(Entity entity, float fixedY, float speed)
        {
            Entity = entity;
            Speed = speed;
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            var actor = Entity as Actor;
            if (actor != null)
            {
                if ((actor.FlagState & FlagState.Root) == FlagState.Root || (actor.FlagState & FlagState.Stun) == FlagState.Stun)
                {
                    // 不可运动
                    return;
                }
            }

            _lastPosition = Entity.Position;

            if (!LockDirection)
            {
                // 面朝目标实体
                if (LookAtTarget != null)
                {
                    var direction = (LookAtTarget.Position - Entity.Position);
                    Entity.Direction.Y = direction.ToEulerAngles();
                }

                // 面朝移动方向
                if (LookAtMoveDirection)
                {
                    Entity.Direction.Y = (_destination - Entity.Position).ToEulerAngles();
                }
            }

            Entity.Position = VectorHelper.MoveTowards(
                Entity.Position,
                _destination,
                Speed * Time.DeltaTime);

            // 力的衰减
            var friction = Entity.Map.Define.Friction * Time.DeltaTime;
            var forceLen = _force.Length();

            if (forceLen > friction)
            {
                Entity.Position += (_force * Time.DeltaTime);
                forceLen -= friction;
                _force = forceLen * _force.Normalized();
            }
            else
            {
                _force = Vector2.Zero;
            }

            Velocity = Entity.Position - _lastPosition;

            Entity.Map.EntityRefreshPosition(Entity);
        }

        public void AddForce(Vector2 force)
        {
            _force += force;
        }

        public void Move(Vector2 destination)
        {
            _destination = destination;
        }
    }
}
