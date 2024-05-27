//
// using System;
// using System.Collections;
// using System.Reflection;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace MMORPG.Tool
// {
//     //TODO FeedbackGameObjectSetVariable待测试
//     [AddFeedbackMenu("GameObject/Set Property")]
//     public class FeedbackGameObjectSetProperty : AbstractFeedback
//     {
//         public enum ValueSetMode
//         {
//             Temporary,
//             Instant
//         }
//
//         [Required]
//         [FoldoutGroup("Set Value")]
//         [HideReferenceObjectPicker]
//         [HideLabel]
//         public PropertyPicker Picker = new();
//
//         [Required]
//         [FoldoutGroup("Set Value")]
//         [HideReferenceObjectPicker]
//         [HideLabel]
//         public VisualObject Object = new();
//
//         [FoldoutGroup("Mode")]
//         public ValueSetMode Mode = ValueSetMode.Instant;
//
//         [FoldoutGroup("Mode")]
//         [ShowIf("Mode", ValueSetMode.Temporary)]
//         public float RecoveryTime = 1;
//
//         protected object OriginValue;
//
//         protected override void OnFeedbackInit()
//         {
//             if (Picker == null || Object == null)
//                 return;
//             Debug.Assert(Picker.IsValid);
//         }
//
//         protected override void OnFeedbackPlay()
//         {
//             if (Picker == null || Object == null)
//                 return;
//             OriginValue = Picker.GetTargetValue();
//
//             Picker.SetTargetValue(Object.GetRawValue());
//             if (Mode == ValueSetMode.Temporary)
//                 StartCoroutine(TemporaryCo());
//         }
//
//         protected override void OnFeedbackStop()
//         {
//         }
//
//         public override float GetDuration()
//         {
//             return Mode == ValueSetMode.Temporary ? RecoveryTime : 0;
//         }
//
//         protected virtual IEnumerator TemporaryCo()
//         {
//             yield return new WaitForSeconds(RecoveryTime);
//             Picker.SetTargetValue(OriginValue);
//         }
//
// #if UNITY_EDITOR
//         public override void OnValidate()
//         {
//             Picker.RefreshInspector();
//         }
//
//         [OnInspectorGUI]
//         protected virtual void OnInspectorGUI()
//         {
//             if (Picker.IsValid && Picker.TargetMember != null)
//             {
//                 Object.Setup(ReflectHelper.GetGeneralMemberValueType(Picker.TargetMember));
//             }
//         }
// #endif
//     }
// }
