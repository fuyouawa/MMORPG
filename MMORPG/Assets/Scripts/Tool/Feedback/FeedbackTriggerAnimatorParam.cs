using System;
using System.Collections;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [FeedbackHelp("这个Feedback用于在Play Feedback时触发指定Animator的Trigger参数")]
    [AddFeedbackMenu("Animator/Trigger Parameter", "触发Trigger参数")]
    public class FeedbackTriggerAnimatorParam : AbstractFeedback
    {
        [FoldoutGroup("Trigger Param")]
        [Required]
        public Animator TargetAnimator;

        [FoldoutGroup("Trigger Param")]
        public Animator[] ExtraAnimators = Array.Empty<Animator>();

        [FoldoutGroup("Trigger Param")]
        [Tooltip("使用Bool参数模拟Trigger的效果, 在Play Feedback时设为True, 然后在下一帧设为False")]
        public bool TriggerLikeBool = false;

        [FoldoutGroup("Trigger Param")]
        [Tooltip("要进行触发的Trigger参数名称")]
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

        protected override void OnFeedbackStop()
        {
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
