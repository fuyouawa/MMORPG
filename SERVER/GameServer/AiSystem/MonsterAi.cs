using System.Diagnostics;
using MMORPG.Common.Proto.Entity;
using GameServer.Tool;
using System.Numerics;
using GameServer.PlayerSystem;
using GameServer.MonsterSystem;
using GameServer.EntitySystem;
using GameServer.AiSystem.Ability;
using GameServer.Manager;
using MMORPG.Common.Proto.Fight;

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
        public Monster OwnerMonster { get; }
        public AnimationState AnimationState;
        public IdleAbility IdleAbility { get; }
        public MoveAbility MoveAbility { get; }
        public CastSkillAbility CastSkillAbility { get; }

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
            CastSkillAbility = new(OwnerMonster);
        }

        public void Start()
        {
            IdleAbility.Start();
            MoveAbility.Start();
        }

        public void Update()
        {
            if (AnimationState == AnimationState.Move)
            {
                MoveAbility.Update();
                if (MoveAbility.Moving)
                {
                    UpdateAnimationState();
                }
                else
                {
                    Idle();
                }
            }
            else if (AnimationState == AnimationState.Idle)
            {
                IdleAbility.Update();
            }
        }

        public void Move(Vector2 destination)
        {
            if (AnimationState == AnimationState.Idle)
            {
                AnimationState = AnimationState.Move;
            }
            MoveAbility.Move(destination);
        }

        public void AddForce(Vector2 force)
        {
            if (AnimationState == AnimationState.Idle)
            {
                AnimationState = AnimationState.Move;
            }
            MoveAbility.AddForce(force);
        }

        public void Idle()
        {
            ChangeAnimationState(AnimationState.Idle);
        }

        public void CastSkill()
        {
            if (!OwnerMonster.SkillManager.SkillDict.Any() || ChasingTarget == null) return;
            var first = OwnerMonster.SkillManager.SkillDict.First();

            if (CastSkillAbility.CastSkill(first.Value.Define.ID, ChasingTarget) == CastResult.Success)
            {
                ChangeAnimationState(AnimationState.Skill);
                AnimationState = AnimationState.Idle;
            }
        }

        public void Revive()
        {
            OwnerMonster.ChangeHP(OwnerMonster.AttributeManager.Final.MaxHp);
        }

        public void OnHurt()
        {
            ChangeAnimationState(AnimationState.Hurt);
            AnimationState = AnimationState.Idle;

            Debug.Assert(OwnerMonster.DamageSourceInfo != null);
            var attackInfo = OwnerMonster.DamageSourceInfo.AttackerInfo;

            switch (attackInfo.AttackerType)
            {
                case AttackerType.Skill:
                    // 能拿到攻击者，施加一个力
                    var skillDefine = DataManager.Instance.SkillDict[attackInfo.SkillId];
                    var target = EntityManager.Instance.GetEntity(attackInfo.AttackerId);
                    if (target == null) return;

                    var direction = OwnerMonster.Position - target.Position;
                    AddForce(direction.Normalized() * skillDefine.Force);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnDeath()
        {
            ChangeAnimationState(AnimationState.Death);
        }

        private void ChangeAnimationState(AnimationState state)
        {
            if (AnimationState == state) return;
            AnimationState = state;
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            var res = new EntityTransformSyncResponse()
            {
                EntityId = OwnerMonster.EntityId,
                StateId = (int)AnimationState,
                Transform = ProtoHelper.ToNetTransform(OwnerMonster.Position, OwnerMonster.Direction)
            };
            OwnerMonster.Map.PlayerManager.Broadcast(res, OwnerMonster);
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
            private Vector2 _lastRandomPointWithBirth;

            public WalkState(FSM<MonsterAiState> fsm, MonsterAbilityManager parameter) :
                base(fsm, parameter)
            {
                _waitTime = _target.Random.NextSingle() * 10;
            }

            public override void OnEnter()
            {
                _lastTime = Time.time;
                if (_target.AnimationState != AnimationState.Move)
                {
                    _target.Idle();
                }
            }

            public override void OnUpdate()
            {
                var monster = _target.OwnerMonster;

                if (monster.IsDeath())
                {
                    _fsm.ChangeState(MonsterAiState.Death);
                    return;
                }
                if (monster.DamageSourceInfo != null && !monster.DamageSourceInfo.IsMiss)
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
                    float d1 = Vector2.Distance(monster.InitPos, nearestPlayer.Position); // 目标与出生点的距离
                    float d2 = Vector2.Distance(monster.Position, nearestPlayer.Position); // 自身与目标的距离
                    if (d1 <= _target.ChaseRange && d2 <= _target.ChaseRange)
                    {
                        // 切换为追击状态
                        _target.ChasingTarget = nearestPlayer as Actor;
                        _fsm.ChangeState(MonsterAiState.Chase);
                        return;
                    }
                }

                if (_target.AnimationState != AnimationState.Idle) return;
                if (!(_lastTime + _waitTime < Time.time)) return;

                _lastRandomPointWithBirth = RandomPointWithBirth(_target.WalkRange);

                // 状态是空闲或等待时间已结束，则尝试随机移动
                _waitTime = _target.Random.NextSingle() * 10;
                _lastTime = Time.time;
                _target.Move(_lastRandomPointWithBirth);
            }


            public Vector2 RandomPointWithBirth(float range)
            {
                var monster = _target.OwnerMonster;
                float x = _target.Random.NextSingle() * 2f - 1f;
                float y = _target.Random.NextSingle() * 2f - 1f;
                var direction = new Vector2(x, y).Normalized();
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
                if (monster.DamageSourceInfo != null && !monster.DamageSourceInfo.IsMiss)
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

                float d1 = Vector2.Distance(monster.Position, player.Position);  // 自身与目标的距离
                float d2 = Vector2.Distance(monster.Position, monster.InitPos); // 自身与出生点的距离
                if (d1 > _target.ChaseRange || d2 > _target.ChaseRange)
                {
                    _fsm.ChangeState(MonsterAiState.Goback);
                    return;
                }

                if (d1 <= _target.AttackRange)
                {
                    // 距离足够，可以发起攻击了
                    _target.CastSkill();
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
