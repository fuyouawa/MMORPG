using Common.Proto.Entity;
using Common.Proto.Space;
using GameServer.Ai;
using GameServer.Manager;
using GameServer.System;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Unit
{
    public class Monster : Actor
    {
        private Vector3 _moveCurrentPos;
        private Vector3 _moveTargetPos;
        private MonsterAi _ai;
        private Random _random;

        public Vector3 InitPos;
        public Actor ChasingTarget;

        public Monster(Vector3 initPos)
        {
            InitPos = initPos;

            _moveCurrentPos = new();
            _moveTargetPos = new();
            _ai = new(this);
        }

        public override void Update()
        {
            if (State == ActorState.Move)
            {
                if (_moveTargetPos == _moveCurrentPos)
                {
                    MoveStop();
                    return;
                }

                var direction = (_moveTargetPos - _moveCurrentPos).Normalize();
                Direction = direction.ToEulerAngles() * new Vector3(0, 1, 0);
                float distance = Speed * EntityManager.Instance.Time.deltaTime;
                if (Vector3.Distance(_moveTargetPos, _moveCurrentPos) < distance)
                {
                    // 走到了目的地
                    _moveCurrentPos = _moveTargetPos;
                    MoveStop();
                }
                else
                {
                    _moveCurrentPos += distance * direction;
                }
                Position = _moveCurrentPos;
                Space.EntityRefreshPosition(this);

                var res = new EntitySyncResponse() { EntitySync = new() };
                res.EntitySync.Entity = this.ToNetEntity();
                Space.CharacterManager.Broadcast(res, this);
            }
        }

        public void MoveTo(Vector3 targetPos)
        {
            if (State == ActorState.Idle)
            {
                State = ActorState.Move;
            }
            if (_moveTargetPos != targetPos)
            {
                _moveTargetPos = targetPos;
                _moveCurrentPos = Position;
            }
        }

        public void MoveStop()
        {
            State = ActorState.Idle;
        }

        public Vector3 RandomPointWithBirth(float range)
        {
            float x = _random.NextSingle() * 2f - 1f;
            float z = _random.NextSingle() * 2f - 1f;
            Vector3 direction = new Vector3(x, 0, z).Normalize();
            return InitPos + direction * range * _random.NextSingle();
        }
    } 
}
