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
            var targetActor = EntityManager.Instance.GetEntity(castInfo.TargetId) as Actor;
            if (targetActor == null)
            {
                Log.Warning("[Spell.SpellUnit]: 对无效的实体释放技能.");
                return;
            }
            var target = new EntityTarget(targetActor);
            var reason = skill.CanRun(target);
            if (reason != CastResult.Success)
            {
                // 不可释放
                var failRes = new SpellFailResponse()
                {
                    CasterId = castInfo.CasterId,
                    SkillId = castInfo.SkillId,
                    Reason = reason,
                };
                var player = _actor as Player;
                if (player != null)
                {
                    player.User.Channel.Send(failRes);
                }
                return;
            }
            skill.Run(target);

            // 广播技能释放给释放者周围的玩家
            var res = new SpellResponse();
            res.CastInfoList.Add(castInfo);
            //Entity[] arr = { skill.Actor, targetActor };
            //Debug.Assert(skill.Actor.Map != null);
            //var entitySet = skill.Actor.Map.GetEntityArrayViewEntitySet(arr, entity => entity.EntityType == EntityType.Player);
            //foreach (var entity in entitySet)
            //{
            //    var player = (Player)entity;
            //    player.User.Channel.Send(res);
            //    Log.Debug($"[Spell.SpellUnit]: 响应{skill.Actor.EntityId}的技能同步请求, 广播给:{player.EntityId}");
            //}
            skill.Actor.Map.PlayerManager.Broadcast(res, skill.Actor);

            // 受击者则在技能命中时广播，投掷物移动当成实体广播
        }

        // 释放位置目标技能
        private void SpellPosition(Skill skill, CastInfo castInfo)
        {
            var target = new PositionTarget(castInfo.TargetPos.ToVector3());
        }
    }
}
