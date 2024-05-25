
using System;
using System.Collections;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [AddFeedbackMenu("GameObject/Set Value")]
    public class FeedbackGameObjectSetValue : AbstractFeedback
    {
        public enum ValueSetMode
        {
            Temporary,
            Instant
        }

        [Required]
        [FoldoutGroup("Set Value")]
        [HideReferenceObjectPicker]
        [HideLabel]
        public ValuePicker Picker = new();

        [Required]
        [FoldoutGroup("Set Value")]
        [HideReferenceObjectPicker]
        [HideLabel]
        public ValueSetter Setter = new();

        [FoldoutGroup("Mode")]
        public ValueSetMode Mode = ValueSetMode.Instant;

        [FoldoutGroup("Mode")]
        [ShowIf("Mode", ValueSetMode.Temporary)]
        public float RecoveryTime = 1;

        protected object OriginValue;

        protected override void OnFeedbackInit()
        {
            if (Picker == null || Setter == null)
                return;
            Picker.InitializeAndCheckValid();
            Debug.Assert(Picker.IsValid);
            Setter.Initialize();
        }

        protected override void OnFeedbackPlay()
        {
            if (Picker == null || Setter == null)
                return;
            OriginValue = Picker.GetTargetValue();

            Picker.SetTargetValue(Setter.GetValueToSet());
            if (Mode == ValueSetMode.Temporary)
                StartCoroutine(TemporaryCo());
        }

        protected override void OnFeedbackStop()
        {
        }

        public override float GetDuration()
        {
            return Mode == ValueSetMode.Temporary ? RecoveryTime : 0;
        }

        protected virtual IEnumerator TemporaryCo()
        {
            yield return new WaitForSeconds(RecoveryTime);
            Picker.SetTargetValue(OriginValue);
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            Picker.OnValidate();
            Setter.Setup(Picker);
        }
#endif
    }
}
