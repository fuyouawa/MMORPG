using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Entity;
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

        public void Cast(SpellRequest req)
        {
            var skill = _actor.SkillManager.GetSkill(req.SkillId);
            if (skill == null)
            {
                Log.Warning("[Spell.Cast]: 无效的技能id.");
                return;
            }
            switch (skill.Define.TargetType)
            {
                case "None":
                    CastNone(skill, req);
                    break;
                case "Unit":
                    CastUnit(skill, req);
                    break;
                case "Position":
                    CastPosition(skill, req);
                    break;
                default:
                    Log.Error("[Spell.Cast]无效的目标类型.");
                    break;
            }
            
        }

        // 释放无目标技能
        private void CastNone(Skill skill, SpellRequest req)
        {
            
        }

        // 释放单位目标技能
        private void CastUnit(Skill skill, SpellRequest req)
        {
            var targetActor = EntityManager.Instance.GetEntity(req.CastTarget.TargetId) as Actor;
            if (targetActor == null)
            {
                Log.Warning("[Spell.CastUnit]: 对无效的实体释放技能.");
                return;
            }
            var resp = new SpellResponse()
            {
                SkillId = req.SkillId,
                CasterId = req.CasterId,
            };

            var target = new EntityCastTarget(targetActor);
            resp.Reason = skill.CanCast(target);
            if (resp.Reason != SpellResult.Success)
            {
                var player = _actor as Player;
                if (player != null)
                {
                    player.User.Channel.Send(resp);
                }
                return;
            }
            skill.Cast(target);

            // 广播技能释放给释放者周围的玩家
            var res = new SpellResponse();
            skill.Actor.Map.PlayerManager.Broadcast(res, skill.Actor);
        }

        // 释放位置目标技能
        private void CastPosition(Skill skill, SpellRequest req)
        {
            var target = new PositionCastTarget(req.CastTarget.TargetPos.ToVector3());
        }
    }
}
