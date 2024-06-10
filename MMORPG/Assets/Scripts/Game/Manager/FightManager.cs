using System;
using Common.Proto.Fight;
using MMORPG.System;
using MMORPG.Tool;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public class FightManager : MonoBehaviour, IController
    {
        private INetworkSystem _network;
        private IEntityManagerSystem _entityManager;

        private void Awake()
        {
            _network = this.GetSystem<INetworkSystem>();
            _entityManager = this.GetSystem<IEntityManagerSystem>();

            _network.Receive<SpellResponse>(OnReceivedSpell)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnReceivedSpell(SpellResponse response)
        {
            var caster = _entityManager.GetEntityById(response.Info.CasterId);
            var skill = caster.SkillManager.GetSkill(response.Info.SkillId);
            if (skill.IsUnitTarget)
            {
                var target = _entityManager.GetEntityById(response.Info.CastTarget.TargetId);
                skill.Use(new CastTargetEntity(target));
            }
            else if (skill.IsPositionTarget)
            {
                skill.Use(new CastTargetPosition(response.Info.CastTarget.TargetPos.ToVector3()));
            }
            else
            {
                skill.Use(new CastTargetEntity(caster));
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
