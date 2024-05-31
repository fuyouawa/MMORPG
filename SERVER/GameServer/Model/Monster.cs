using Common.Proto.EventLike;
using GameServer.Ai;
using GameServer.Tool;
using System.Numerics;

namespace GameServer.Model
{
    public class Monster : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        private Vector3 _moveCurrentPos;
        private Vector3 _moveTargetPos;
        private AiBase? _ai;
        private Random _random = new();

        public Vector3 InitPos;
        public Actor? ChasingTarget;

        public Monster(Map map, string name, Vector3 initPos) : base(map, name)
        {
            InitPos = initPos;
        }

        public override void Start()
        {
            base.Start();

            switch (DataHelper.GetUnitDefine(UnitId).Ai)
            {
                case "Monster":
                    _ai = new MonsterAi(this);
                    break;
            }

            _ai?.Start();
        }

        public override void Update()
        {
            base.Update();
            _ai?.Update();

            if (Map == null)
            {
                return;
            }
            if (State == ActorState.Move)
            {
                if (_moveTargetPos == _moveCurrentPos)
                {
                    Idle();
                    return;
                }

                var direction = (_moveTargetPos - _moveCurrentPos).Normalized();
                Direction = direction.ToEulerAngles() * new Vector3(0, 1, 0);
                //float distance = Speed * Time.DeltaTime;
                float distance = 2 * Time.DeltaTime;
                
                if (Vector3.Distance(_moveTargetPos, _moveCurrentPos) <= distance)
                {
                    // 本次移动能到达目的地
                    _moveCurrentPos = _moveTargetPos;
                }
                else
                {
                    // 向这个方向移动指定距离
                    _moveCurrentPos += distance * direction;
                }
                Position = _moveCurrentPos;
                var res = new EntityTransformSyncResponse()
                {
                    EntityId = EntityId,
                    StateId = (int)State,
                    Transform = ProtoHelper.ToNetTransform(Position, Direction)
                };
                Map.EntityRefreshPosition(this);
                Map.PlayerManager.Broadcast(res, this);
            }
        }

        public void Revive()
        {

        }

        public void Move(Vector3 targetPos)
        {
            if (State == ActorState.Idle)
            {
                State = ActorState.Move;
            }
            if (_moveTargetPos != targetPos)
            {
                _moveTargetPos = targetPos;
                _moveTargetPos.Y = InitPos.Y;
                _moveCurrentPos = Position;
            }
        }

        public void Idle()
        {
            ChangeState(ActorState.Idle);
        }

        public void Attack()
        {
            ChangeState(ActorState.Attack);
            State = ActorState.Idle;
        }

        private void ChangeState(ActorState state)
        {
            if (State == state) return;
            State = state;
            if (Map != null)
            {
                var res = new EntityTransformSyncResponse()
                {
                    EntityId = EntityId,
                    StateId = (int)State,
                    Transform = ProtoHelper.ToNetTransform(Position, Direction)
                };
                Map.PlayerManager.Broadcast(res, this);
            }
        }

        public Vector3 RandomPointWithBirth(float range)
        {
            float x = _random.NextSingle() * 2f - 1f;
            float z = _random.NextSingle() * 2f - 1f;
            Vector3 direction = new Vector3(x, 0, z).Normalized();
            return InitPos + direction * range * _random.NextSingle();
        }
    } 
}
