using System.Diagnostics;
using MMORPG.Common.Proto.Entity;
using GameServer.Tool;
using System.Numerics;
using MMORPG.Common.Proto.Entity;
using GameServer.PlayerSystem;
using GameServer.MonsterSystem;
using GameServer.EntitySystem;
using GameServer.AiSystem.Ability;
using GameServer.Manager;
using System.Threading;
using MMORPG.Common.Proto.Fight;
using Org.BouncyCastle.Ocsp;
using GameServer.FightSystem;
using static GameServer.AiSystem.MonsterAbilityManager;

namespace GameServer.AiSystem
{
    public enum MonsterAiState
    {
        None = 0,
        Walk,
        Hurt,
        Chase,
        Goback,
        Death,
    }

    public class MonsterAbilityManager
    {
        public Monster OwnerMonster;
        public ActorState SyncState;
        public IdleAbility IdleAbility;
        public MoveAbility MoveAbility;
        public Actor? ChasingTarget;
        public Random Random = new();

        // 相对于出生点的活动范围
        public float WalkRange = 10f;
        // 相对于出生点的追击范围
        public float ChaseRange = 10f;
        // 攻击范围
        public float AttackRange = 1f;

        public MonsterAbilityManager(Monster ownerMonster)
        {
            OwnerMonster = ownerMonster;
            MoveAbility = new(OwnerMonster, OwnerMonster.InitPos.Y, OwnerMonster.Speed);
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
            if (!OwnerMonster.SkillManager.SkillDict.Any() || ChasingTarget == null) return;
            var first = OwnerMonster.SkillManager.SkillDict.First();
            
            var castInfo = new CastInfo()
            {
                SkillId = first.Value.Define.ID,
                CasterId = OwnerMonster.EntityId,
                CastTarget = new NetCastTarget()
                {
                    TargetId = ChasingTarget.EntityId,
                },
            };

            ChangeSyncState(ActorState.Skill);
            SyncState = ActorState.Idle;

            OwnerMonster.Spell.Cast(castInfo);
        }

        public void Revive()
        {
            OwnerMonster.ChangeHP(OwnerMonster.AttributeManager.Final.MaxHp);
        }

        public void OnHurt()
        {
            ChangeSyncState(ActorState.Hurt);
            SyncState = ActorState.Idle;

            // 能拿到攻击者，施加一个力
            Debug.Assert(OwnerMonster.DamageSourceInfo != null);
            var skillDefine = DataManager.Instance.SkillDict[OwnerMonster.DamageSourceInfo.SkillId];
            var target = EntityManager.Instance.GetEntity(OwnerMonster.DamageSourceInfo.AttackerId);
            if (target == null) return;

            var tmpTargetPos = target.Position;
            tmpTargetPos.Y = OwnerMonster.Position.Y;
            AddForce((OwnerMonster.Position - tmpTargetPos).Normalized(), skillDefine.Force);
        }

        public void OnDeath()
        {
            ChangeSyncState(ActorState.Death);
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
                EntityId = OwnerMonster.EntityId,
                StateId = (int)SyncState,
                Transform = ProtoHelper.ToNetTransform(OwnerMonster.Position, OwnerMonster.Direction)
            };
            OwnerMonster.Map.PlayerManager.Broadcast(res, OwnerMonster);
        }

        private void AddForce(Vector3 distance, float force)
        {
            MoveAbility.Speed = force;
            Move(OwnerMonster.Position + distance * force);
        }
    }

    public class MonsterAi : AiBase
    {
        public FSM<MonsterAiState> Fsm = new();
        public MonsterAbilityManager AbilityManager;

        public MonsterAi(Monster monster)
        {
            AbilityManager = new MonsterAbilityManager(monster);
            Fsm.AddState(MonsterAiState.Walk, new WalkState(Fsm, AbilityManager));
            Fsm.AddState(MonsterAiState.Chase, new ChaseState(Fsm, AbilityManager));
            Fsm.AddState(MonsterAiState.Goback, new GobackState(Fsm, AbilityManager));
            Fsm.AddState(MonsterAiState.Hurt, new HurtState(Fsm, AbilityManager));
            Fsm.AddState(MonsterAiState.Death, new DeathState(Fsm, AbilityManager));
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
                _waitTime = _target.Random.NextSingle() * 10;
            }

            public override void OnEnter()
            {
                _lastTime = Time.time;
                _target.Idle();
            }

            public override void OnUpdate()
            {
                var monster = _target.OwnerMonster;

                if (monster.IsDeath())
                {
                    _fsm.ChangeState(MonsterAiState.Death);
                    return;
                }
                if (monster.DamageSourceInfo != null)
                {
                    _fsm.ChangeState(MonsterAiState.Hurt);
                    return;
                }

                // 查找怪物视野范围内距离怪物最近的玩家
                var nearestPlayer = monster.Map.GetEntityFollowingNearest(monster, 
                    e =>
                    {
                        if (e.EntityType != EntityType.Player) return false;
                        var player = (Player)e;
                        return player.IsValid() && !player.IsDeath();
                    });

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
                _waitTime = _target.Random.NextSingle() * 10;
                _lastTime = Time.time;
                _target.Move(RandomPointWithBirth(_target.WalkRange));
            }


            public Vector3 RandomPointWithBirth(float range)
            {
                var monster = _target.OwnerMonster;
                float x = _target.Random.NextSingle() * 2f - 1f;
                float z = _target.Random.NextSingle() * 2f - 1f;
                Vector3 direction = new Vector3(x, 0, z).Normalized();
                return monster.InitPos + direction * range * _target.Random.NextSingle();
            }
        }

        /// <summary>
        /// 受击状态
        /// </summary>
        public class HurtState : FSMAbstractState<MonsterAiState, MonsterAbilityManager>
        {
            private float _endTime;
            private DamageInfo _currentDamageInfo;

            public HurtState(FSM<MonsterAiState> fsm, MonsterAbilityManager parameter) :
                base(fsm, parameter)
            {
            }

            public override void OnEnter()
            {
                _target.MoveAbility.LockDirection = true;

                _endTime = Time.time + _target.OwnerMonster.UnitDefine.HurtTime;
                Debug.Assert(_target.OwnerMonster.DamageSourceInfo != null);
                _currentDamageInfo = _target.OwnerMonster.DamageSourceInfo;
                _target.OnHurt();
            }

            public override void OnExit()
            {
                _target.MoveAbility.LockDirection = false;
                _target.OwnerMonster.DamageSourceInfo = null;
                _target.MoveAbility.Speed = _target.OwnerMonster.Speed;
            }

            public override void OnUpdate()
            {
                if (_target.OwnerMonster.IsDeath())
                {
                    _fsm.ChangeState(MonsterAiState.Death);
                    return;
                }
                if (!(_endTime < Time.time)) return;
                if (_currentDamageInfo != _target.OwnerMonster.DamageSourceInfo)
                {
                    OnEnter();
                    return;
                }
                _fsm.ChangeState(MonsterAiState.Walk);
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
                var monster = _target.OwnerMonster;
                if (monster.IsDeath())
                {
                    _fsm.ChangeState(MonsterAiState.Death);
                    return;
                }
                if (monster.DamageSourceInfo != null)
                {
                    _fsm.ChangeState(MonsterAiState.Hurt);
                    return;
                }
                if (_target.ChasingTarget == null)// || monster.ChasingTarget.IsDeath())
                {
                    _fsm.ChangeState(MonsterAiState.Goback);
                    return;
                }

                var player = _target.ChasingTarget as Player;
                if (player == null || !player.IsValid() || player.IsDeath())
                {
                    _fsm.ChangeState(MonsterAiState.Goback);
                    return;
                }

                var monsterPos = new Vector2(monster.Position.X, monster.Position.Z);
                float d1 = Vector2.Distance(monsterPos, player.Position.ToVector2());  // 自身与目标的距离
                float d2 = Vector2.Distance(monsterPos, monster.InitPos.ToVector2()); // 自身与出生点的距离
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
                _target.Move(_target.OwnerMonster.InitPos);

                // 切回巡逻状态，使得回出生点的过程也能继续寻敌
                _fsm.ChangeState(MonsterAiState.Walk);
            }
        }

        /// <summary>
        /// 死亡状态
        /// </summary>
        public class DeathState : FSMAbstractState<MonsterAiState, MonsterAbilityManager>
        {
            public DeathState(FSM<MonsterAiState> fsm, MonsterAbilityManager parameter) :
                base(fsm, parameter)
            { }

            public override void OnEnter()
            {
                _target.OnDeath();
            }

            public override void OnUpdate()
            {
                if (_target.OwnerMonster.IsDeath()) return;
                _fsm.ChangeState(MonsterAiState.Walk);
            }
        }

    }
}
