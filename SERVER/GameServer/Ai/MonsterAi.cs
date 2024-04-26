using Common.Proto.Entity;
using GameServer.Manager;
using GameServer.Tool;
using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Ai
{

    public enum MonsterAiState
    {
        Walk,
        Chase,
        Goback,
    }

    public class MonsterAi : AiBase
    {
        public FSM<MonsterAiState> FSM;

        public MonsterAi(Monster monster)
        {
            FSM = new();
            var parameter = new StateParameter(monster);
            FSM.AddState(MonsterAiState.Walk, new WalkState(FSM, parameter));
            FSM.AddState(MonsterAiState.Chase, new ChaseState(FSM, parameter));
            FSM.AddState(MonsterAiState.Goback, new GobackState(FSM, parameter));
        }


        public override void Update()
        {
            FSM.Update();
        }


        public class StateParameter
        {
            public Monster Monster;
            public Random Random = new();

            public StateParameter(Monster monster)
            {
                Monster = monster;
            }
        }

        /// <summary>
        /// 巡逻状态
        /// </summary>
        public class WalkState : FSMAbstractState<MonsterAiState, StateParameter>
        {
            private float _lastTime;
            private float _waitTime;

            // 相对于出生点的活动范围
            private float _walkRange = 100f;

            public WalkState(FSM<MonsterAiState> fsm, StateParameter parameter) :
                base(fsm, parameter)
            {
                _lastTime = EntityManager.Instance.Time.time;
                _waitTime = 10f;
            }

            public override void OnEnter()
            {
                _target.Monster.MoveStop();
            }

            public override void OnUpdate()
            {
                var monster = _target.Monster;
                var list = monster.Map?.GetEntityViewEntityList(monster, e => e.EntityType == EntityType.Player);
                var nearestPlayer = list?.Aggregate((minEntity, nextEntity) =>
                {
                    var minDistance = Vector3.Distance(minEntity.Position, monster.Position);
                    var nextDistance = Vector3.Distance(nextEntity.Position, monster.Position);
                    return minDistance < nextDistance ? minEntity : nextEntity;
                });
                if (nearestPlayer != null &&
                    Vector3.Distance(nearestPlayer.Position, monster.Position) <= monster.ViewRange)
                {
                    monster.ChasingTarget = nearestPlayer as Actor;
                    _fsm.ChangeState(MonsterAiState.Chase);
                    return;
                }

                if (monster.State != ActorState.Idle) return;
                if (!(_lastTime + _waitTime < EntityManager.Instance.Time.time)) return;

                _waitTime = _target.Random.NextSingle();
                _lastTime = EntityManager.Instance.Time.time;
                // 移动到随机位置
                monster.MoveTo(monster.RandomPointWithBirth(10));
            }
        }

        /// <summary>
        /// 追击状态
        /// </summary>
        public class ChaseState : FSMAbstractState<MonsterAiState, StateParameter>
        {
            // 相对于出生点的追击范围
            private float _chaseRange = 100f;

            public ChaseState(FSM<MonsterAiState> fsm, StateParameter parameter) :
                base(fsm, parameter)
            { }

            public override void OnUpdate()
            {
                var monster = _target.Monster;
                if (monster.ChasingTarget == null) return;
                // 自身与出生点的距离
                float d1 = Vector3.Distance(monster.Position, monster.InitPos);
                // 自身与目标的距离
                float d2 = Vector3.Distance(monster.Position, monster.ChasingTarget.Position);
                if (d1 > _chaseRange || d2 > monster.ViewRange)
                {
                    _fsm.ChangeState(MonsterAiState.Goback);
                    return;
                }

                if (d2 < 1)
                {
                    // 距离足够，可以发起攻击了
                    if (monster.State == ActorState.Move)
                    {
                        monster.MoveStop();
                    }
                }
                else
                {
                    monster.MoveTo(monster.Position);
                }
            }
        }

        /// <summary>
        /// 返回状态
        /// </summary>
        public class GobackState : FSMAbstractState<MonsterAiState, StateParameter>
        {
            public GobackState(FSM<MonsterAiState> fsm, StateParameter parameter) :
                base(fsm, parameter)
            { }

            public override void OnEnter()
            {
                _target.Monster.MoveTo(_target.Monster.InitPos);
            }

            public override void OnUpdate()
            {
                if (_target.Monster.State == ActorState.Idle)
                {
                    _fsm.ChangeState(MonsterAiState.Walk);
                }
            }
        }
    }
}
