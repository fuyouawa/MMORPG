using System.Numerics;
using GameServer.EntitySystem;
using GameServer.Tool;

namespace GameServer.AiSystem.Ability
{
    public class MoveAbility : Ability
    {
        private Vector3 _moveTargetPos;
        private float FixedY;

        public Entity Entity;
        public float Speed;
        public bool Moving { get; private set; }
        public bool LockDirection;

        public MoveAbility(Entity entity, float fixedY, float speed)
        {
            Entity = entity;
            FixedY = fixedY;
            Moving = false;
            Speed = speed;
            LockDirection = false;
        }

        public override void Start()
        {

        }

        public override void Update()
        {
            if (_moveTargetPos == Entity.Position) Moving = false;
            if (!Moving) return;

            var direction = (_moveTargetPos - Entity.Position).Normalized();
            if (!LockDirection)
            {
                Entity.Direction = direction.ToEulerAngles() * new Vector3(0, 1, 0);
            }

            float distance = Speed * Time.DeltaTime;

            if (Vector3.Distance(_moveTargetPos, Entity.Position) <= distance)
            {
                // 本次移动能到达目的地
                Entity.Position = _moveTargetPos;
            }
            else
            {
                // 向这个方向移动指定距离
                Entity.Position += distance * direction;
            }
            Entity.Map.EntityRefreshPosition(Entity);
        }

        public void Move(Vector3 targetPos)
        {
            Moving = true;
            _moveTargetPos = targetPos;
            _moveTargetPos.Y = FixedY;
        }

        public void AddForce(Vector3 distance, float force)
        {
            _moveTargetPos += distance * force;
        }
    }
}
