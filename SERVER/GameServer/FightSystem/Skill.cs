using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Fight;
using GameServer.AiSystem;
using GameServer.Manager;
using GameServer.Tool;
using Serilog;
using GameServer.EntitySystem;
using MMORPG.Common.Tool;

namespace GameServer.FightSystem
{
    public class Skill
    {
        public enum Stage
        {
            Idle = 0,
            Intonate,   // 吟唱
            Active,     // 已激活
            Cooling,   // 冷却中
        }

        public Actor Actor;
        public SkillDefine Define;
        public Stage CurrentStage;

        private float _time;
        private float[] _hitDelay;
        private int _hitDelayIndex;
        private CastTarget _castTarget;
        private Random _random = new();

        public Skill(Actor actor, SkillDefine define)
        {
            Actor = actor;
            Define = define;
        }

        public void Start()
        {
            _hitDelay = DataHelper.ParseFloats(Define.HitDelay);
            if (_hitDelay == null || _hitDelay.Length == 0)
            {
                _hitDelay = new[] { 0.0f };
            }
        }

        public void Update()
        {
            if (CurrentStage == Stage.Idle) return;
            _time += Time.DeltaTime;

            // 如果是吟唱阶段并且吟唱已经结束
            if (CurrentStage == Stage.Intonate && _time >= Define.IntonateTime)
            {
                OnActive();
            }

            // 如果是技能激活阶段
            if (CurrentStage == Stage.Active)
            {
                OnRun();
                if (CurrentStage == Stage.Cooling)
                {
                    // 技能释放完成
                    OnFinish();
                }
            }

            // 如果是技能冷却阶段
            if (CurrentStage == Stage.Cooling && _time >= Define.Cd)
            {
                // 技能冷却完成
                OnCoolingEnded();
            } 
        }

        public CastResult CanCast(CastTarget castTarget)
        {
            if (CurrentStage == Stage.Cooling)
            {
                return CastResult.Cooling;
            }
            if (CurrentStage != Stage.Idle)
            {
                return CastResult.Running;
            }
            if (!Actor.IsValid() || Actor.IsDeath())
            {
                return CastResult.EntityDead;
            }
            if (castTarget is CastTargetEntity target)
            {
                var targetActor = target.Entity as Actor;
                if (targetActor == null || !targetActor.IsValid() || targetActor.IsDeath())
                {
                    return CastResult.TargetInvaild;
                }
            }
            var dist = Vector3.Distance(Actor.Position, castTarget.Position);
            if (dist > Define.SpellRange)
            {
                return CastResult.OutOfRange;
            }
            return CastResult.Success;
        }

        public CastResult Cast(CastTarget castTarget)
        {
            _time = 0;
            CurrentStage = Stage.Intonate;
            _castTarget = castTarget;
            Actor.Spell.CurrentRunSkill = this;
            return CastResult.Success;
        }

        /// <summary>
        /// 技能激活
        /// </summary>
        private void OnActive()
        {
            CurrentStage = Stage.Active;

            // 技能激活
            _time -= Define.IntonateTime;

            Log.Debug("[Skill.OnActive]");
            if (Define.MissileUnitId != 0)
            {
                var missileUnitDefine = DataManager.Instance.UnitDict[Define.MissileUnitId];

                var missile = Actor.Map.MissileManager.NewMissile(Define.MissileUnitId, 
                    Actor.Position, Actor.Direction, 
                    Define.Area, missileUnitDefine.Speed, _castTarget,
                    entity =>
                    {
                        Log.Information("Missile命中");
                    });
            }
            else
            {
                _hitDelayIndex = 0;
            }
        }

        /// <summary>
        /// 技能每帧运行
        /// </summary>
        private void OnRun()
        {
            if (_hitDelayIndex < _hitDelay.Length)
            {
                if (_time >= _hitDelay[_hitDelayIndex])
                {
                    _time -= _hitDelay[_hitDelayIndex];
                    // 命中延迟触发
                    OnHit(_castTarget);
                    ++_hitDelayIndex;
                }
            }
            else
            {
                CurrentStage = Stage.Cooling;
            }
        }

        public void OnHit(CastTarget castTarget)
        {
            if (Define.Area == 0)
            {
                if (castTarget is CastTargetEntity target)
                {
                    CauseDamage((Actor)target.Entity);
                }
            }
            else
            {
                Actor.Map.ScanEntityFollowing(Actor, e =>
                {
                    float distance = Vector3.Distance(castTarget.Position, e.Position);
                    if (distance > Define.Area) return;
                    
                    if (e is Actor target)
                        CauseDamage(target);
                });
            }
        }

        private void CauseDamage(Actor target)
        {
            // 伤害 = 攻击 × (1 - 护甲 / (护甲 + 400 ＋ 85 × 等级))
            var a = Actor.AttributeManager.Final;
            var b = target.AttributeManager.Final;
            var amount = 0f;

            var damageInfo = new DamageInfo()
            {
                AttackerId = Actor.EntityId,
                TargetId = target.EntityId,
                SkillId = Define.ID,
                DamageType = DamageType.Physical,
            };

            var hitRate = a.HitRate - b.DodgeRate;
            var randHitRate = _random.NextSingle();
            if (hitRate >= randHitRate)
            {
                var ad = Define.Ad + a.Ad * Define.Adc;
                var ap = Define.Ap + a.Ap * Define.Apc;

                var ads = ad * (1 - b.Def / (b.Def + 400 + 85 * Actor.Level));
                var aps = ap * (1 - b.Mdef / (b.Mdef + 400 + 85 * Actor.Level));

                amount = ads + aps;

                var randCri = _random.NextSingle();
                var cri = a.Cri * 0.01f;
                if (cri >= randCri)
                {
                    damageInfo.IsCrit = true;
                    amount *= a.Crd * 0.01f;
                }
            }
            else
            {
                damageInfo.IsMiss = true;
                amount = 0;
            }
            damageInfo.Amount = (int)amount;
            target.OnHurt(damageInfo);
        }

        /// <summary>
        /// 技能释放完成
        /// </summary>
        private void OnFinish()
        {
            Actor.Spell.CurrentRunSkill = null;
        }

        /// <summary>
        /// 技能冷却完成
        /// </summary>
        private void OnCoolingEnded()
        {
            CurrentStage = Stage.Idle;
        }
    }
}
