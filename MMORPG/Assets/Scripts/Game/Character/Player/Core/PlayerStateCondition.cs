using Sirenix.OdinInspector;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class PlayerStateCondition
{
    [InfoBox("Invalid method!", InfoMessageType.Error, "CheckMethodNameInvalid")]
    [ValueDropdown("GetAllStateConditions")]
    [VerticalGroup("Method")]
    [HideLabel]
    public string FullMethodName = string.Empty;
    [TableColumnWidth(30, false)]
    public bool Not = false;

    public PlayerTransition OwnerTransition { get; set; }

    public string MethodName =>
        FullMethodName == string.Empty ? string.Empty : FullMethodName.Split('/')[1];

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

        Debug.Assert(FullMethodName != string.Empty, "FullMethodName != string.Empty");

        var split = FullMethodName.Split('/');
        Debug.Assert(split.Length == 2);
        var objectName = split[0];
        var methodName = split[1];

        _methodObject = OwnerTransition.OwnerState.GetAttachAbilities()
            .First(x => x.GetType().Name == objectName);

        _methodInfo = _methodObject.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        Debug.Assert(_methodInfo != null, "_methodInfo != null");
        Debug.Assert(_methodInfo.ReturnType == typeof(bool), "_methodInfo.ReturnType == typeof(bool)");

        _method = () =>
        {
            var res = (bool)_methodInfo.Invoke(_methodObject, null);
            return Not ? !res : res;
        };
    }


#if UNITY_EDITOR
    private IEnumerable GetAllStateConditions()
    {
        var total = new ValueDropdownList<string>() { { "None Condition", string.Empty } };
        if (OwnerTransition?.OwnerState != null)
        {
            total.AddRange(
                from ability in OwnerTransition.OwnerState.GetAttachAbilities()
                from condition in ability.GetStateConditions()
                let name = $"{ability.GetType().Name}/{condition.Name}"
                select new ValueDropdownItem<string>(
                    $"{name}{(condition.ReturnType == typeof(bool) ? "" : " - ERROR")}", name)
            );
        }

        return total;
    }

    private bool CheckMethodNameInvalid()
    {
        if (FullMethodName == string.Empty)
            return true;
        var split = FullMethodName.Split('/');
        if (split.Length != 2)
            return true;
        var objectName = split[0];
        var methodName = split[1];

        var methodObject = OwnerTransition.OwnerState.GetAttachAbilities()
            .FirstOrDefault(x => x.GetType().Name == objectName);
        if (methodObject == null)
            return true;

        var methodInfo = methodObject.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (methodInfo == null)
            return true;
        return false;
    }
#endif
}
