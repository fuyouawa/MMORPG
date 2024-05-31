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
        public float Cooldown;
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
            if (Cooldown > 0) Cooldown -= Time.DeltaTime;
            if (Cooldown < 0) Cooldown = 0;

            _time += Time.DeltaTime;

            if (CurrentStage == Stage.Intonate && _time >= Define.IntonateTime)
            {
                CurrentStage = Stage.Active;
                OnActive();
            }

            if (CurrentStage == Stage.Active)
            {
                if (_time >= Define.IntonateTime + _hitDelay.Max())
                {
                    CurrentStage = Stage.Collding;
                }
            }

            if (CurrentStage == Stage.Collding)
            {
                if (_time >= Define.IntonateTime + Define.Cd)
                {
                    _time = 0;
                    CurrentStage = Stage.None;
                    OnFinish();
                }
            }
        }

        public CastResult CanRun(Target target)
        {
            if (CurrentStage != Stage.None)
            {
                return CastResult.Running;
            }
            if (Cooldown > 0)
            {
                return CastResult.Colldown;
            }
            if (Actor.IsDeath() || !Actor.IsValid())
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

        public void Run(Target target)
        {

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
