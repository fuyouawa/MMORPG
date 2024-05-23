using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using QFramework;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Tool
{
    [Serializable]
    public struct UnityValueGetter<T>
    {
        // public GameObject TargetGameObject;

        // public class MemberGetter
        // {
        //     public Component Target;
        //     public MemberInfo Member;
        //     public Func<T> Getter;
        // }
        // public MemberGetter Getter;

        [HorizontalGroup] [HideLabel] [SerializeField]
        private GameObject _targetGameObject;

        [HorizontalGroup] [HideLabel] [ValueDropdown("GetTargetGameObjectValueGetterDropdown")] [SerializeField]
        private string _memberPath;

        public enum SimpleMemberTypes
        {
            Field,
            Property,
            Method
        }

        public Component Target { get; private set; }
        public MemberInfo Member { get; private set; }
        public SimpleMemberTypes MemberType { get; private set; }

        private Func<object, T> _getter;

        private static readonly string PropertyEnd = "{ get; }";
        private static readonly string MethodEnd = "{ get; }";
        private static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public void Initialize()
        {
            if (_targetGameObject == null || _memberPath.IsNullOrEmpty())
                return;

            var split = _memberPath.Split('/');
            Debug.Assert(split.Length == 2);
            var targetName = split[0];
            var memberName = split[1];

            Target = _targetGameObject.GetComponents<Component>().FirstOrDefault(x => x.GetType().Name == targetName);
            Debug.Assert(Target);
            if (memberName.EndsWith(PropertyEnd))
            {
                MemberType = SimpleMemberTypes.Property;
                var property = Target.GetType().GetProperty(memberName[..^PropertyEnd.Length], BindingFlags);
                Debug.Assert(property != null);
                _getter = obj => (T)property.GetValue(obj);
                Member = property;
            }
            else if (memberName.EndsWith(MethodEnd))
            {
                memberName = memberName[..^MethodEnd.Length];
                MemberType = SimpleMemberTypes.Method;
                var method = Target.GetType().GetMethod(memberName[..^PropertyEnd.Length], BindingFlags);
                Debug.Assert(method != null);
                _getter = obj => (T)method.Invoke(obj, null);
                Member = method;
            }
            else
            {
                MemberType = SimpleMemberTypes.Field;
                var field = Target.GetType().GetField(memberName, BindingFlags);
                Debug.Assert(field != null);
                _getter = obj => (T)field.GetValue(obj);
                Member = field;
            }
        }


        public T GetValue()
        {
            return _getter(Target);
        }


#if UNITY_EDITOR
        public IEnumerable GetTargetGameObjectValueGetterDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (_targetGameObject == null)
                return total;
            foreach (var component in _targetGameObject.GetComponents<Component>())
            {
                var compName = component.GetType().Name;
                foreach (var field in component.GetType().GetFields(BindingFlags)
                             .Where(x => x.FieldType == typeof(T)))
                {
                    total.Add($"{compName}/{field.Name}", $"{compName}/{field.Name}");
                }

                foreach (var property in component.GetType().GetProperties(BindingFlags)
                             .Where(x => x.PropertyType == typeof(T)))
                {
                    total.Add($"{compName}/{property.Name}{PropertyEnd}", $"{compName}/{property.Name}");
                }

                foreach (var method in component.GetType().GetMethods(BindingFlags)
                             .Where(x => x.ReturnType == typeof(T) && !x.GetParameters().Any()))
                {
                    total.Add($"{compName}/{method.Name}{MethodEnd}", $"{compName}/{method.Name}");
                }
            }

            return total;
        }
#endif
    }
}

// #if UNITY_EDITOR
//     public class UnityValueGetterDrawer<T> : OdinValueDrawer<UnityValueGetter<T>>
//     {
//         private readonly Dictionary<Component, List<UnityValueGetter<T>.MemberGetter>> _valueGettersMap = new();
//         private Component[] _prevComponents;
//         private GameObject _prevGameObject;
//         private Func<UnityValueGetter<T>.MemberGetter, string> _menuItemNameGetter;
//
//         protected override void Initialize()
//         {
//             _menuItemNameGetter = x =>
//             {
//                 var str = $"{x.Target.GetType().Name}/{x.Member.Name}";
//                 switch (x.Member.MemberType)
//                 {
//                     case MemberTypes.Field:
//                         break;
//                     case MemberTypes.Method:
//                         str += "()";
//                         break;
//                     case MemberTypes.Property:
//                         str += "{ get; }";
//                         break;
//                     default:
//                         throw new NotSupportedException();
//                 }
//                 return str;
//             };
//         }
//
//         protected override void DrawPropertyLayout(GUIContent label)
//         {
//             var value = ValueEntry.SmartValue;
//             UpdateValueGettersMapIfNeeded();
//
//             var rect = EditorGUILayout.GetControlRect();
//
//             value.TargetGameObject = (GameObject)EditorGUI.ObjectField(rect.AlignLeft(rect.width * 0.5f), value.TargetGameObject, typeof(GameObject), true);
//
//             value.Getter = OdinSelector<UnityValueGetter<T>.MemberGetter>
//                 .DrawSelectorDropdown(
//                     rect.AlignRight(rect.width * 0.5f),
//                     value.Getter?.Member == null ? "None" : value.Getter.Member.Name,
//                     CreateSelector)?.FirstOrDefault();
//
//             ValueEntry.SmartValue = value;
//         }
//
//         private OdinSelector<UnityValueGetter<T>.MemberGetter> CreateSelector(Rect rect)
//         {
//             GenericSelector<UnityValueGetter<T>.MemberGetter> genericSelector;
//             if (_valueGettersMap.Count > 0)
//             {
//                 var collection = _valueGettersMap.SelectMany(kv => kv.Value);
//
//                 genericSelector = new GenericSelector<UnityValueGetter<T>.MemberGetter>(
//                     "", false,
//                     _menuItemNameGetter,
//                     collection);
//             }
//             else
//             {
//                 genericSelector = new GenericSelector<UnityValueGetter<T>.MemberGetter>();
//             }
//             genericSelector.SetSelection(ValueEntry.SmartValue.Getter);
//             genericSelector.ShowInPopup(rect);
//
//             return genericSelector;
//         }
//
//
//         private void UpdateValueGettersMapIfNeeded()
//         {
//             const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
//             var value = ValueEntry.SmartValue;
//             if (value.TargetGameObject == null)
//             {
//                 _valueGettersMap.Clear();
//                 return;
//             }
//
//             var components = value.TargetGameObject.GetComponents<Component>();
//
//             if (value.TargetGameObject == _prevGameObject && components.Length == _prevComponents.Length)
//             {
//                 for (int i = 0; i < components.Length; i++)
//                 {
//                     if (components[i] != _prevComponents[i])
//                     {
//                         return;
//                     }
//                 }
//             }
//             _prevGameObject = value.TargetGameObject;
//             _prevComponents = components;
//
//             _valueGettersMap.Clear();
//             foreach (var component in components)
//             {
//                 var list = new List<UnityValueGetter<T>.MemberGetter>();
//
//                 list.AddRange(
//                     from field in component.GetType().GetFields(bindingFlags)
//                     where field.FieldType == typeof(T)
//                     select new UnityValueGetter<T>.MemberGetter()
//                     {
//                         Target = component,
//                         Member = field,
//                         Getter = () => (T)field.GetValue(component)
//                     }
//                 );
//
//                 list.AddRange(
//                     from property in component.GetType().GetProperties(bindingFlags)
//                     where property.PropertyType == typeof(T)
//                     select new UnityValueGetter<T>.MemberGetter()
//                     {
//                         Target = component,
//                         Member = property,
//                         Getter = () => (T)property.GetValue(component)
//                     }
//                 );
//
//                 list.AddRange(
//                     from method in component.GetType().GetMethods(bindingFlags)
//                     where method.ReturnType == typeof(T) && !method.GetParameters().Any()
//                     select new UnityValueGetter<T>.MemberGetter()
//                     {
//                         Target = component,
//                         Member = method,
//                         Getter = () => (T)method.Invoke(component, null)
//                     }
//                 );
//
//                 if (list.Count > 0)
//                 {
//                     _valueGettersMap[component] = list;
//                 }
//             }
//         }
//     }
// #endif
// }
