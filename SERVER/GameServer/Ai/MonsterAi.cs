using MMORPG.Common.Proto.Entity;
using GameServer.Tool;
using GameServer.Model;
using System.Numerics;
using MMORPG.Common.Proto.Monster;
using GameServer.Ability;

namespace GameServer.Ai
{
    public enum MonsterAiState
    {
        None = 0,
        Walk,
        Chase,
        Goback,
    }

    public class MonsterAbilityManager
    {
        public Monster Monster;
        public ActorState SyncState;
        public IdleAbility IdleAbility;
        public MoveAbility MoveAbility;
        public Actor? ChasingTarget;
        public Random Random = new(10);

        // 相对于出生点的活动范围
        public float WalkRange = 10f;
        // 相对于出生点的追击范围
        public float ChaseRange = 10f;
        // 攻击范围
        public float AttackRange = 1f;

        public MonsterAbilityManager(Monster monster)
        {
            Monster = monster;
            MoveAbility = new(monster, Monster.InitPos.Y);
            IdleAbility = new();
        }

        public void Start()
        {
            IdleAbility.Start();
            MoveAbility.Start();
        }

        public void Update()
        {
            if (SyncState == ActorState.Move)
            {
                MoveAbility.Update();
                if (MoveAbility.Moving)
                {
                    UpdateSyncState();
                }
                else
                {
                    Idle();
                }
            }
            else if (SyncState == ActorState.Idle)
            {
                IdleAbility.Update();
            }
        }

        public void Move(Vector3 targetPos)
        {
            if (SyncState == ActorState.Idle)
            {
                SyncState = ActorState.Move;
            }
            MoveAbility.Move(targetPos);
        }

        public void Idle()
        {
            ChangeSyncState(ActorState.Idle);
        }

        public void Attack()
        {
            ChangeSyncState(ActorState.Skill);
            SyncState = ActorState.Idle;
        }

        public void Revive()
        {

        }

        


        private void ChangeSyncState(ActorState state)
        {
            if (SyncState == state) return;
            SyncState = state;
            UpdateSyncState();
        }

        private void UpdateSyncState()
        {
            var res = new EntityTransformSyncResponse()
            {
                EntityId = Monster.EntityId,
                StateId = (int)SyncState,
                Transform = ProtoHelper.ToNetTransform(Monster.Position, Monster.Direction)
            };
            Monster.Map.PlayerManager.Broadcast(res, Monster);
        }
    }


    public class MonsterAi : AiBase
    {
        // 行为状态机
        public FSM<MonsterAiState> Fsm = new();
        public MonsterAbilityManager AbilityManager;

        public MonsterAi(Monster monster)
        {
            AbilityManager = new MonsterAbilityManager(monster);
            Fsm.AddState(MonsterAiState.Walk, new WalkState(Fsm, AbilityManager));
            Fsm.AddState(MonsterAiState.Chase, new ChaseState(Fsm, AbilityManager));
            Fsm.AddState(MonsterAiState.Goback, new GobackState(Fsm, AbilityManager));
        }

        public override void Start()
        {
            AbilityManager.Start();
            Fsm.ChangeState(MonsterAiState.Walk);
        }

        public override void Update()
        {
            AbilityManager.Update();
            Fsm.Update();
        }

        /// <summary>
        /// 巡逻状态
        /// </summary>
        public class WalkState : FSMAbstractState<MonsterAiState, MonsterAbilityManager>
        {
            private float _lastTime;
            private float _waitTime;

            public WalkState(FSM<MonsterAiState> fsm, MonsterAbilityManager parameter) :
                base(fsm, parameter)
            {
                _lastTime = Time.time;
                _waitTime = 10f;
            }

            public override void OnEnter()
            {
                _target.Idle();
            }

            public override void OnUpdate()
            {
                var monster = _target.Monster;

                // 查找怪物视野范围内距离怪物最近的玩家
                var nearestPlayer = monster.Map.GetEntityFollowingNearest(monster, e => e.EntityType == EntityType.Player);

                if (nearestPlayer != null)
                {
                    // 若玩家位于怪物的追击范围内
                    float d1 = Vector2.Distance(monster.InitPos.ToVector2(), nearestPlayer.Position.ToVector2()); // 目标与出生点的距离
                    float d2 = Vector2.Distance(monster.Position.ToVector2(), nearestPlayer.Position.ToVector2()); // 自身与目标的距离
                    if (d1 <= _target.ChaseRange && d2 <= _target.ChaseRange)
                    {
                        // 切换为追击状态
                        _target.ChasingTarget = nearestPlayer as Actor;
                        _fsm.ChangeState(MonsterAiState.Chase);
                        return;
                    }
                }

                if (_target.SyncState != ActorState.Idle) return;
                if (!(_lastTime + _waitTime < Time.time)) return;

                // 状态是空闲或等待时间已结束，则尝试随机移动
                _waitTime = _target.Random.NextSingle();
                _lastTime = Time.time;
                _target.Move(RandomPointWithBirth(_target.WalkRange));
            }


            public Vector3 RandomPointWithBirth(float range)
            {
                var monster = _target.Monster;
                float x = _target.Random.NextSingle() * 2f - 1f;
                float z = _target.Random.NextSingle() * 2f - 1f;
                Vector3 direction = new Vector3(x, 0, z).Normalized();
                return monster.InitPos + direction * range * _target.Random.NextSingle();
            }
        }

        /// <summary>
        /// 追击状态
        /// </summary>
        public class ChaseState : FSMAbstractState<MonsterAiState, MonsterAbilityManager>
        {
            public ChaseState(FSM<MonsterAiState> fsm, MonsterAbilityManager parameter) :
                base(fsm, parameter)
            { }

            public override void OnUpdate()
            {
                var monster = _target.Monster;
                if (_target.ChasingTarget == null)// || monster.ChasingTarget.IsDeath())
                {
                    _fsm.ChangeState(MonsterAiState.Goback);
                    return;
                }

                var player = _target.ChasingTarget as Player;
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
                    _target.Attack();
                }
                else
                {
                    _target.Move(player.Position);
                }
            }
        }

        /// <summary>
        /// 返回状态
        /// </summary>
        public class GobackState : FSMAbstractState<MonsterAiState, MonsterAbilityManager>
        {
            public GobackState(FSM<MonsterAiState> fsm, MonsterAbilityManager parameter) :
                base(fsm, parameter)
            { }

            public override void OnEnter()
            {
                _target.Move(_target.Monster.InitPos);

                // 切回巡逻状态，使得回出生点的过程也能继续寻敌
                _fsm.ChangeState(MonsterAiState.Walk);
            }
        }
    }
}
