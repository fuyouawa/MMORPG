using System.Numerics;
using Common.Proto.EventLike;
using GameServer.Fight;
using GameServer.Model;
using GameServer.Tool;

namespace GameServer.Ai
{
    public class MoveAbility : Ability
    {
        public Actor Actor;
        private Vector3 _moveTargetPos;
        private float FixedY;
        public bool Moving { get; private set; }

        public MoveAbility(Actor actor, float fixedY)
        {
            Actor = actor;
            FixedY = fixedY;
            Moving = false;
        }

        public override void Start()
        {

        }

        public override void Update()
        {
            if (_moveTargetPos == Actor.Position) Moving = false;
            if (!Moving) return;

            var direction = (_moveTargetPos - Actor.Position).Normalized();
            Actor.Direction = direction.ToEulerAngles() * new Vector3(0, 1, 0);
            float distance = Actor.Speed * Time.DeltaTime;

            if (Vector3.Distance(_moveTargetPos, Actor.Position) <= distance)
            {
                // 本次移动能到达目的地
                Actor.Position = _moveTargetPos;
            }
            else
            {
                // 向这个方向移动指定距离
                Actor.Position += distance * direction;
            }
            Actor.Map.EntityRefreshPosition(Actor);
        }

        public void Move(Vector3 targetPos)
        {
            Moving = true;
            _moveTargetPos = targetPos;
            _moveTargetPos.Y = FixedY;
        }


    }
}
