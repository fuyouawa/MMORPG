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
using Serilog;

namespace GameServer.Fight
{
    /// <summary>
    /// 技能释放器
    /// </summary>
    public class Spell
    {
        private Actor _actor;

        public Spell(Actor actor)
        {
            _actor = actor;
        }

        public void RunCast(CastInfo castInfo)
        {
            var skill = _actor.SkillManager.GetSkill(castInfo.SkillId);
            if (skill == null)
            {
                Log.Warning("[Spell.RunCast]: 无效的技能id.");
                return;
            }

            switch (skill.Define.TargetType)
            {
                case "None":
                    SpellNone(skill, castInfo);
                    break;
                case "Unit":
                    SpellUnit(skill, castInfo);
                    break;
                case "Position":
                    SpellPosition(skill, castInfo);
                    break;
            }
            
        }

        // 释放无目标技能
        private void SpellNone(Skill skill, CastInfo castInfo)
        {
            
        }


        // 释放单位目标技能
        private void SpellUnit(Skill skill, CastInfo castInfo)
        {
            var actor = EntityManager.Instance.GetEntity(castInfo.TargetId) as Actor;
            if (actor == null)
            {
                Log.Warning("[Spell.SpellUnit]: 对无效的实体释放技能.");
                return;
            }

            var res = new SpellResponse();
            res.CastInfoList.Add(castInfo);

            // 广播技能释放
            skill.Actor.Map?.PlayerManager.Broadcast(res, skill.Actor);

            // todo 如果受击目标不在被广播对象的关注列表中，那么还需要临时将其添加到被广播对象的关注列表中
        }

        // 释放位置目标技能
        private void SpellPosition(Skill skill, CastInfo castInfo)
        {

        }
    }
}
