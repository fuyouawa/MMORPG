using System;
using System.Collections;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [AddFeedbackMenu("Animator/Trigger Parameter")]
    public class FeedbackTriggerAnimatorParam : Feedback
    {
        [FoldoutGroup("Trigger Param")]
        [Required]
        public Animator TargetAnimator;

        [FoldoutGroup("Trigger Param")]
        public Animator[] ExtraAnimators = Array.Empty<Animator>();

        [FoldoutGroup("Trigger Param")]
        [Tooltip("Use the bool to simulate the Trigger effect, set to true and set to false in the next frame")]
        public bool TriggerLikeBool = false;

        [FoldoutGroup("Trigger Param")]
        public string ParameterName;

        protected int ParamId;
        protected AnimatorControllerParameterType ParamType;

        protected override void OnFeedbackInit()
        {
            ParamType = TriggerLikeBool
                ? AnimatorControllerParameterType.Bool
                : AnimatorControllerParameterType.Trigger;

            if (!TargetAnimator.HasParam(ParameterName, ParamType))
                throw new Exception($"Animator({TargetAnimator}) has no parameter({ParameterName})");
            ParamId = Animator.StringToHash(ParameterName);

            foreach (var animator in ExtraAnimators)
            {
                if (!animator.HasParam(ParameterName, ParamType))
                    throw new Exception($"Animator({animator}) has no parameter({ParameterName})");
            }
        }

        protected override void OnFeedbackPlay()
        {
            if (!TriggerLikeBool)
            {
                TargetAnimator.SetTrigger(ParamId);
                ExtraAnimators.ForEach(x => x.SetTrigger(ParamId));
            }
            else
            {
                StartCoroutine(TriggerLikeBoolCo());
            }
        }

        protected virtual IEnumerator TriggerLikeBoolCo()
        {
            TargetAnimator.SetBool(ParamId, true);
            ExtraAnimators.ForEach(x => x.SetBool(ParamId, true));
            yield return null;
            TargetAnimator.SetBool(ParamId, false);
            ExtraAnimators.ForEach(x => x.SetBool(ParamId, false));
        }
    }
}
