
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [AddFeedbackMenu("GameObject/Set Value")]
    public class FeedbackGameObjectSetValue : AbstractFeedback
    {
        [FoldoutGroup("Set Value")]
        [HideReferenceObjectPicker]
        [HideLabel]
        public ValuePicker ValuePicker = new();

        protected override void OnFeedbackInit()
        {
            ValuePicker.Initialize();
            Debug.Assert(ValuePicker.IsValid);
        }

        protected override void OnFeedbackPlay()
        {

        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
