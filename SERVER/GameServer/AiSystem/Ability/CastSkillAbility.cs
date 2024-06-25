using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.EntitySystem;

namespace GameServer.AiSystem.Ability
{
    public class CastSkillAbility : Ability
    {
        public Actor OwnerActor;

        public CastSkillAbility(Actor ownerActor)
        {
            OwnerActor = ownerActor;
        }

        public override void Start()
        {

        }

        public override void Update()
        {

        }

        public CastResult CastSkill(int skillId, Actor target)
        {
            var castInfo = new CastInfo()
            {
                SkillId = skillId,
                CasterId = OwnerActor.EntityId,
                CastTarget = new NetCastTarget()
                {
                    TargetId = target.EntityId,
                },
            };
            return OwnerActor.Spell.Cast(castInfo);
        }
    }
}
