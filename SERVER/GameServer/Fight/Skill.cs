using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Proto.Fight;
using GameServer.Ai;
using GameServer.Manager;
using GameServer.Model;
using GameServer.Tool;
using Serilog;

namespace GameServer.Fight
{
    public class Skill
    {
        public enum Stage
        {
            None = 0,
            Intonate,   // 吟唱
            Active,     // 已激活
            Collding,   // 冷却中
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
            _hitDelay = DataHelper.ParseJson<float[]>(Define.HitDelay);
            if (_hitDelay == null || _hitDelay.Length == 0)
            {
                _hitDelay = new[] { 0.0f };
            }
        }

        public void Update()
        {
            if (CurrentStage == Stage.None) return;
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
            }

            // 如果是技能冷却阶段
            if (CurrentStage == Stage.Collding && _time >= Define.Cd)
            {
                // 冷却完成
                OnFinish();
            } 
        }

        public CastResult CanCast(CastTarget castTarget)
        {
            if (CurrentStage == Stage.Collding)
            {
                return CastResult.Colldown;
            }
            if (CurrentStage != Stage.None)
            {
                return CastResult.Running;
            }
            if (!Actor.IsValid() || Actor.IsDeath())
            {
                return CastResult.EntityDead;
            }
            if (castTarget is EntityCastTarget)
            {
                var targetActor = castTarget.RealObj as Actor;
                if (targetActor == null || !targetActor.IsValid() || targetActor.IsDeath())
                {
                    return CastResult.TargetInvaild;
                }
            }
            var dist = Vector3.Distance(Actor.Position, castTarget.Position());
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
            if (Define.IsMissile)
            {
                var missile = Actor.Map.MissileManager.NewMissile(Define.UnitID, Vector3.Zero, Vector3.Zero);
            }
            else
            {
                _hitDelayIndex = 0;
            }
        }

        private void OnRun()
        {
            if (_hitDelayIndex < _hitDelay.Length)
            {
                if (_time >= _hitDelay[_hitDelayIndex] * 1000)
                {
                    _time = _hitDelay[_hitDelayIndex] * 1000;
                    // 命中延迟触发
                    OnHit(_castTarget);
                }
            }
            else
            {
                CurrentStage = Stage.Collding;
            }
        }

        public void OnHit(CastTarget castTarget)
        {
            if (Define.Area == 0)
            {
                var target = castTarget.RealObj as Actor;
                if (target != null) CauseDamage(target);
            }
            else
            {
                var list = Actor.Map.GetEntityFollowingList(Actor, e =>
                {
                    float distance = Vector3.Distance(castTarget.Position(), e.Position);
                    return distance <= Define.Area;
                });
                foreach (var entity in list)
                {
                    var target = entity as Actor;
                    if (target != null) CauseDamage(target);
                }
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
            if (hitRate < randHitRate)
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
            target.OnDamage(damageInfo);
        }

        /// <summary>
        /// 技能冷却完成
        /// </summary>
        private void OnFinish()
        {
            CurrentStage = Stage.None;
        }

    }
}
