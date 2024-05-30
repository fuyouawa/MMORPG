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

        public SkillDefine Define;
        public Actor Actor;
        public float Cooldown;
        public Stage CurrentStage;

        private float _runTime;

        public Skill(Actor actor, SkillDefine define)
        {
            Actor = actor;
            Define = define;
        }

        public void Update()
        {
            if (Cooldown > 0) Cooldown -= Time.DeltaTime;
            if (Cooldown < 0) Cooldown = 0;

            _runTime += Time.DeltaTime;

            if (CurrentStage == Stage.Intonate && _runTime >= Define.IntonateTime)
            {
                CurrentStage = Stage.Active;
            }

        }

        public CastResult CanUse()
        {
            return CastResult.Success;
        }

        public void Use()
        {

        }


    }
}
