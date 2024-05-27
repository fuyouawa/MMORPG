using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [FeedbackHelp("这个Feedback用于选择GameObject上的一个函数, 填写参数(如果有)然后在Play Feedback时调用")]
    [AddFeedbackMenu("GameObject/Invoke Method", "调用函数")]
    public class FeedbackGameObjectInvokeMethod : AbstractFeedback
    {
        [Required]
        [FoldoutGroup("Invoke Method")]
        [HideReferenceObjectPicker]
        [HideLabel]
        [DisableInPlayMode]
        public MethodPicker Picker = new(false, MethodPickerParamTypeFilters.Visual);

        [FoldoutGroup("Invoke Method")]
        [ShowIf("HasParameter")]
        [ListDrawerSettings(IsReadOnly = true)]
        public VisualObject[] ParameterInputs;

        public bool HasParameter => ParameterInputs?.Length > 0;

        protected override void OnFeedbackInit()
        {
            Picker.Initialize();
            if (Picker.TargetMethod == null)
                throw new Exception("Invalid method");
            var parameters = Picker.TargetMethod.GetParameters();
            Debug.Assert(ParameterInputs.Length == parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInputs[i].Setup(parameters[i].ParameterType, parameters[i].Name);
            }
        }
        

        protected override void OnFeedbackPlay()
        {
            Picker.TargetMethod.Invoke(Picker.TargetComponent, ParameterInputs.Select(x => x.GetRawValue()).ToArray());
        }

        protected override void OnFeedbackStop()
        {
        }

#if UNITY_EDITOR
        private string _prevMethodName;

        public override void OnInspectorGUI()
        {
            if (Picker.TargetMemberName != _prevMethodName)
            {
                if (Picker.TryGetMethod(out var method))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length != ParameterInputs.Length)
                    {
                        ParameterInputs = method.GetParameters().Select(x =>
                        {
                            var obj = new VisualObject();
                            obj.Setup(x.ParameterType, x.Name);
                            return obj;
                        }).ToArray();
                    }
                    else
                    {
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            ParameterInputs[i].Setup(parameters[i].ParameterType, parameters[i].Name);
                        }
                    }
                }
                else
                {
                    ParameterInputs = Array.Empty<VisualObject>();
                }

                _prevMethodName = Picker.TargetMemberName;
            }
        }
#endif
    }
}
