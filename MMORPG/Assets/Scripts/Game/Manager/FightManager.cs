using System;
using MMORPG.Common.Proto.Fight;
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
            var skill = caster.GetComponent<CharacterController>().SkillManager.GetSkill(response.Info.SkillId);
            switch (skill.TargetType)
            {
                case SkillTargetTypes.Unit:
                    var target = _entityManager.GetEntityById(response.Info.CastTarget.TargetId);
                    skill.Use(new CastTargetEntity(target));
                    break;
                case SkillTargetTypes.Position:
                    skill.Use(new CastTargetPosition(response.Info.CastTarget.TargetPos.ToVector3()));
                    break;
                default:
                    skill.Use(new CastTargetEntity(caster));
                    break;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
