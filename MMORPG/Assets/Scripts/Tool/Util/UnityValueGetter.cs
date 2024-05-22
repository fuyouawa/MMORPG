using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [Serializable]
    public class UnityValueGetter<T>
    {
        [HideLabel]
        [HorizontalGroup(0.4f)]
        [SerializeField]
        private GameObject _targetGameObject;
        [HideLabel]
        [HorizontalGroup(0.6f)]
        [ValueDropdown("GetTargetGameObjectValueGetterDropdown")]
        [SerializeField]
        private string _memberName;

#if UNITY_EDITOR
        public IEnumerable GetTargetGameObjectValueGetterDropdown()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (_targetGameObject == null)
                return total;
            foreach (var component in _targetGameObject.GetComponents<Component>())
            {
                var compName = component.GetType().Name;
                foreach (var field in component.GetType().GetFields(bindingFlags)
                             .Where(x => x.FieldType == typeof(T)))
                {
                    total.Add($"{compName}/{field.Name}", $"{compName}/{field.Name}");
                }
                foreach (var property in component.GetType().GetProperties(bindingFlags)
                             .Where(x => x.PropertyType == typeof(T)))
                {
                    total.Add($"{compName}/{property.Name}{{ get; }}", $"{compName}/{property.Name}");
                }
                foreach (var method in component.GetType().GetMethods(bindingFlags)
                             .Where(x => x.ReturnType == typeof(T) && !x.GetParameters().Any()))
                {
                    total.Add($"{compName}/{method.Name}()", $"{compName}/{method.Name}");
                }
            }
            return total;
        }
#endif
    }
}
