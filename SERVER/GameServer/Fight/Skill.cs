using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Fight;
using GameServer.Manager;
using GameServer.Model;
using GameServer.Tool;

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

        public Skill(Actor actor, SkillDefine define)
        {
            Actor = actor;
            Define = define;
        }

        public void Start()
        {
            _hitDelay = DataHelper.ParseJson<float[]>(Define.HitDelay);
        }

        public void Update()
        {
            if (CurrentStage == Stage.None) return;
            _time += Time.DeltaTime;

            // 如果是吟唱阶段并且吟唱已经结束
            if (CurrentStage == Stage.Intonate && _time >= Define.IntonateTime)
            {
                // 技能激活
                _time -= Define.IntonateTime;
                CurrentStage = Stage.Active;
                OnActive();
            }

            // 如果是技能激活阶段
            if (CurrentStage == Stage.Active)
            {
                // 命中延迟？后才开始计入冷却
                var max = _hitDelay.Max();
                if (_time >= max)
                {
                    _time -= max;
                    CurrentStage = Stage.Collding;
                }
            }

            // 如果是技能冷却阶段
            if (CurrentStage == Stage.Collding && _time >= Define.Cd)
            {
                // 冷却完成
                CurrentStage = Stage.None;
                OnFinish();
            } 
        }

        public CastResult CanRun(Target target)
        {
            if (CurrentStage != Stage.Collding)
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
            if (target is EntityTarget)
            {
                var targetActor = target.RealObj as Actor;
                if (targetActor == null || !targetActor.IsValid() || targetActor.IsDeath())
                {
                    return CastResult.TargetInvaild;
                }
            }
            var dist = Vector3.Distance(Actor.Position, target.Position());
            if (dist > Define.SpellRange)
            {
                return CastResult.OutOfRange;
            }
            return CastResult.Success;
        }

        public CastResult Run(Target target)
        {
            _time = 0;
            CurrentStage = Stage.Intonate;
            return CastResult.Success;
        }

        /// <summary>
        /// 技能激活
        /// </summary>
        private void OnActive()
        {

        }

        /// <summary>
        /// 技能冷却完成
        /// </summary>
        private void OnFinish()
        {

        }

    }
}
