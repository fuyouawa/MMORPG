using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Fight;
using GameServer.Tool;
using Org.BouncyCastle.Asn1.X509;
using Serilog;
using GameServer.EntitySystem;
using GameServer.PlayerSystem;
using GameServer.MonsterSystem;

namespace GameServer.FightSystem
{
    /// <summary>
    /// 技能释放器
    /// </summary>
    public class Spell
    {
        public Actor OwnerActor;
        public Skill? CurrentRunSkill;

        public Spell(Actor ownerActor)
        {
            OwnerActor = ownerActor;
        }

        public void Cast(CastInfo info)
        {
            if (CurrentRunSkill != null)
            {
                ResponseSpellFail(info, CastResult.Running);
                return;
            }
            var skill = OwnerActor.SkillManager.GetSkill(info.SkillId);
            if (skill == null)
            {
                ResponseSpellFail(info, CastResult.InvalidSkillId);
                return;
            }

            if (OwnerActor is Player player)
            {
                Log.Information($"玩家({player.User.Channel})请求释放技能: TargetType:{skill.Define.TargetType}, SkillId:{info.SkillId}, CastId:{info.CasterId}");
            }
            else if (OwnerActor is Monster monster)
            {
                Log.Information($"怪物({monster.Name})请求释放技能: TargetType:{skill.Define.TargetType}, SkillId:{info.SkillId}, CastId:{info.CasterId}");
            }
            switch (skill.Define.TargetType)
            {
                case "None":
                    CastNone(skill, info);
                    break;
                case "Unit":
                    CastUnit(skill, info);
                    break;
                case "Position":
                    CastPosition(skill, info);
                    break;
                default:
                    Log.Error("[Spell.Cast]无效的目标类型.");
                    break;
            }
        }

        // 释放无目标技能
        private void CastNone(Skill skill, CastInfo info)
        {
            var target = new CastTargetEntity(OwnerActor);
            CastTarget(skill, info, target);
        }

        // 释放单位目标技能
        private void CastUnit(Skill skill, CastInfo info)
        {
            var targetActor = EntityManager.Instance.GetEntity(info.CastTarget.TargetId) as Actor;
            if (targetActor == null)
            {
                ResponseSpellFail(info, CastResult.InvalidCastTarget);
                return;
            }
            var target = new CastTargetEntity(targetActor);
            CastTarget(skill, info, target);
        }

        // 释放位置目标技能
        private void CastPosition(Skill skill, CastInfo info)
        {
            var target = new CastTargetPosition(info.CastTarget.TargetPos.ToVector3().ToVector2());
            CastTarget(skill, info, target);
        }


        private void CastTarget(Skill skill, CastInfo info, CastTarget target)
        {
            var res = skill.CanCast(target);
            ResponseSpellFail(info, res);
            if (res != CastResult.Success)
            {
                return;
            }
            skill.Cast(target);
            skill.OwnerActor.Map.PlayerManager.Broadcast(new SpellResponse() { Info = info }, skill.OwnerActor);
        }


        private void ResponseSpellFail(CastInfo info, CastResult result)
        {
            if (OwnerActor is Player player)
            {
                if (result != CastResult.Success)
                {
                    Log.Debug($"{player.User.Channel}请求攻击时出现错误: {result}");
                }
                player.User.Channel.Send(new SpellFailResponse()
                {
                    CasterId = info.CasterId,
                    SkillId = info.SkillId,
                    Reason = result
                });
            }
        }
    }
}
