using System;
using System.Collections.Generic;
using MMORPG.Common.Proto.Fight;
using MMORPG.Event;
using MMORPG.Tool;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public class EntityHurtEffectManager : MonoBehaviour, IController
    {
        [Serializable]
        public class EntitySkillHurtEffect
        {
            public int SkillId;
            public FeedbacksManager Feedbacks;
        }

        public EntityView Owner;

        [SerializeField]
        private List<EntitySkillHurtEffect> _entitySkillHurtEffects = new();

        private void Awake()
        {
            this.RegisterEvent<EntityHurtEvent>(OnEntityHurt).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEntityHurt(EntityHurtEvent e)
        {
            if (e.Wounded != Owner)
                return;

            switch (e.DamageInfo.AttackerInfo.AttackerType)
            {
                case AttackerType.Skill:
                    var effect = _entitySkillHurtEffects?.Find(x => x.SkillId == e.DamageInfo.AttackerInfo.SkillId);
                    effect?.Feedbacks.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
