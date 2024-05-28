using Common.Proto.Entity;
using GameServer.Manager;
using GameServer.Tool;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Proto.EventLike;

namespace GameServer.Ai
{

    public enum MonsterAiState
    {
        None = 0,
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

        public override void Start()
        {
            FSM.ChangeState(MonsterAiState.Walk);
        }

        public override void Update()
        {
            FSM.Update();
        }

        public class StateParameter
        {
            public Monster Monster;
            public Random Random = new();

            // 相对于出生点的活动范围
            public float WalkRange = 10f;
            // 相对于出生点的追击范围
            public float ChaseRange = 10f;
            // 攻击范围
            public float AttackRange = 1f;

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

            public WalkState(FSM<MonsterAiState> fsm, StateParameter parameter) :
                base(fsm, parameter)
            {
                _lastTime = Time.time;
                _waitTime = 10f;
            }

            public override void OnEnter()
            {
                _target.Monster.Idle();
            }

            public override void OnUpdate()
            {
                var monster = _target.Monster;

                // 查找怪物视野范围内距离怪物最近的玩家
                var list = monster.Map?.GetEntityViewEntityList(monster, e => e.EntityType == EntityType.Player);
                if (list != null && list.Any())
                {
                    var nearestPlayer = list.Aggregate((minEntity, nextEntity) =>
                    {
                        var minDistance = Vector3.Distance(minEntity.Position, monster.Position);
                        var nextDistance = Vector3.Distance(nextEntity.Position, monster.Position);
                        return minDistance < nextDistance ? minEntity : nextEntity;
                    });
                    Vector3.Distance(monster.Position, monster.InitPos);
                    // 若玩家位于怪物的追击范围内
                    if (nearestPlayer != null)
                    {
                        float d1 = Vector3.Distance(monster.InitPos, nearestPlayer.Position);  // 目标与出生点的距离
                        float d2 = Vector3.Distance(monster.Position, nearestPlayer.Position); // 自身与目标的距离
                        if (d1 <= _target.ChaseRange && d2 <= _target.ChaseRange)
                        {
                            // 切换为追击状态
                            monster.ChasingTarget = nearestPlayer as Actor;
                            _fsm.ChangeState(MonsterAiState.Chase);
                            return;
                        }
                    }
                }

                if (monster.State != ActorState.Idle) return;
                if (!(_lastTime + _waitTime < Time.time)) return;

                // 状态是空闲或等待时间已结束，则尝试随机移动
                _waitTime = _target.Random.NextSingle();
                _lastTime = Time.time;
                monster.Move(monster.RandomPointWithBirth(_target.WalkRange));
            }
        }

        /// <summary>
        /// 追击状态
        /// </summary>
        public class ChaseState : FSMAbstractState<MonsterAiState, StateParameter>
        {
            
            public ChaseState(FSM<MonsterAiState> fsm, StateParameter parameter) :
                base(fsm, parameter)
            { }

            public override void OnUpdate()
            {
                var monster = _target.Monster;
                if (monster.ChasingTarget == null)// || monster.ChasingTarget.IsDeath())
                {
                    _fsm.ChangeState(MonsterAiState.Goback);
                    return;
                }

                var player = monster.ChasingTarget as Player;
                if (player == null || !player.IsValid())
                {
                    _fsm.ChangeState(MonsterAiState.Goback);
                    return;
                }

                var monsterPos = new Vector2(monster.Position.X, monster.Position.Z);
                var playerPos = new Vector2(player.Position.X, player.Position.Z);
                float d1 = Vector2.Distance(monsterPos, playerPos);  // 自身与目标的距离
                float d2 = Vector3.Distance(monster.Position, monster.InitPos); // 自身与出生点的距离
                if (d1 > _target.ChaseRange || d2 > _target.ChaseRange)
                {
                    _fsm.ChangeState(MonsterAiState.Goback);
                    return;
                }

                if (d1 <= _target.AttackRange)
                {
                    // 距离足够，可以发起攻击了
                    monster.Attack();
                }
                else
                {
                    monster.Move(player.Position);
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
                _target.Monster.Move(_target.Monster.InitPos);

                // 切回巡逻状态，使得回出生点的过程也能继续寻敌
                _fsm.ChangeState(MonsterAiState.Walk);
            }
        }
    }
}
