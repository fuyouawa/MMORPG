using System;
 using System.Collections.Generic;
 using MMORPG.Tool;
 using Sirenix.OdinInspector;
 using UnityEngine;

 namespace MMORPG.Game
{
    public class SkillsEffectManager : SerializedMonoBehaviour
    {
        [Serializable]
        public struct SkillEffect
        {
            public int SkillId;
            public FeedbacksManager SpellFeedbacks;
        }

        public List<SkillEffect> Effects = new();

        public void TriggerEffect(int skillId)
        {
            var effect = Effects.Find(x => x.SkillId == skillId);
            if (effect.SpellFeedbacks != null)
            {
                effect.SpellFeedbacks.Play();
            }
        }
    }
}
