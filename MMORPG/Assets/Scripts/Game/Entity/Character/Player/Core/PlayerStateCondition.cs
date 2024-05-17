using Sirenix.OdinInspector;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    [Serializable]
    public class PlayerStateCondition
    {
        [InfoBox("Invalid method!", InfoMessageType.Error, "CheckMethodNameInvalid")]
        [ValueDropdown("GetStateConditionsDropdown")]
        [VerticalGroup("Method")]
        [HideLabel]
        public string FullMethodName = string.Empty;
        [TableColumnWidth(30, false)]
        public bool Not = false;

        public PlayerTransition OwnerTransition { get; set; }

        private object _methodObject;
        private MethodInfo _methodInfo;

        private Func<bool> _method;
        public bool Invoke()
        {
            return _method.Invoke();
        }

        public void Initialize(PlayerTransition transition)
        {
            OwnerTransition = transition;

            Debug.Assert(!FullMethodName.IsNullOrEmpty());

            var split = FullMethodName.Split('/');
            Debug.Assert(split.Length == 2);
            var objectName = split[0];
            var methodName = split[1];

            _methodObject = OwnerTransition.OwnerState.Brain.GetAttachLocalAbilities()
                .First(x => x.GetType().Name == objectName);

            _methodInfo = _methodObject.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Debug.Assert(_methodInfo != null);
            Debug.Assert(_methodInfo.ReturnType == typeof(bool));
            Debug.Assert(_methodInfo.GetParameters().Length == 0);

            _method = () =>
            {
                var res = (bool)_methodInfo.Invoke(_methodObject, null);
                return Not ? !res : res;
            };
        }


#if UNITY_EDITOR
        private IEnumerable GetStateConditionsDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None Condition", string.Empty } };
            if (OwnerTransition?.OwnerState != null)
            {
                var abilities = OwnerTransition.OwnerState.Brain.GetAttachLocalAbilities();
                if (abilities == null)
                    return total;
                foreach (var ability in abilities)
                {
                    foreach (var condition in ability.GetStateConditions())
                    {
                        var attr = condition.GetAttribute<StateConditionAttribute>();
                        var methodPath = $"{ability.GetType().Name}/{condition.Name}";
                        var displayName = $"{ability.GetType().Name}/{condition.Name}";
                        total.Add(displayName, methodPath);
                    }
                }
            }

            return total;
        }

        private bool CheckMethodNameInvalid()
        {
            if (OwnerTransition?.OwnerState?.Brain == null)
                return false;

            if (FullMethodName == string.Empty)
                return true;
            var split = FullMethodName.Split('/');
            if (split.Length != 2)
                return true;
            var objectName = split[0];
            var methodName = split[1];

            var methodObject = OwnerTransition.OwnerState.Brain.GetAttachLocalAbilities()?
                .FirstOrDefault(x => x.GetType().Name == objectName);
            if (methodObject == null)
                return true;

            var methodInfo = methodObject.GetType().GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (methodInfo == null)
                return true;
            return false;
        }

        public bool HasError()
        {
            return CheckMethodNameInvalid();
        }
#endif
    }

}
