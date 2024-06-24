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
        public struct SkillFeedback
        {
            public int SkillId;
            public FeedbacksManager SpellFeedbacks;
        }

        [SerializeField]
        private List<SkillFeedback> _skillFeedbacks = new();

        public void TriggerEffect(int skillId)
        {
            var feedback = _skillFeedbacks.Find(x => x.SkillId == skillId);
            if (feedback.SpellFeedbacks != null)
            {
                feedback.SpellFeedbacks.Play();
            }
        }
    }
}
