using System;
using System.Collections.Generic;
using System.Linq;
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

        private float _runTime;
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

            _runTime += Time.DeltaTime;

            if (CurrentStage == Stage.Intonate && _runTime >= Define.IntonateTime)
            {
                CurrentStage = Stage.Active;
                OnActive();
            }

            if (CurrentStage == Stage.Active)
            {
                if (_runTime >= Define.IntonateTime + _hitDelay.Max())
                {
                    CurrentStage = Stage.Collding;
                }
            }

            if (CurrentStage == Stage.Collding)
            {
                if (_runTime >= Define.IntonateTime + Define.Cd)
                {
                    _runTime = 0;
                    CurrentStage = Stage.None;
                }
            }
        }

        public CastResult CanUse()
        {


            return CastResult.Success;
        }

        public void Use()
        {

        }

        /// <summary>
        /// 技能激活
        /// </summary>
        private void OnActive()
        {

        }

    }
}
