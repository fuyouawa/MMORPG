using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

[AttributeUsage(AttributeTargets.Field)]
public class AnimatorParamAttribute : Attribute
{
    public string ParamName = string.Empty;
}

public struct AnimatorTrigger
{
    public bool IsTriggered { get; private set; }

    public void Trigger() => IsTriggered = true;
}

public class AnimatorMachine
{
    private static Dictionary<Type, FieldInfo[]> s_paramsInfoCache = new();

    public Animator Animator { get; }

    private object _target;
    private FieldInfo[] _paramsInfo;


    public AnimatorMachine(object target, Animator animator)
    {
        _target = target;
        Animator = animator;

        InitParamsInfo();
    }

    private void InitParamsInfo()
    {
        var type = _target.GetType();
        if (s_paramsInfoCache.TryGetValue(type, out _paramsInfo))
            return;
        _paramsInfo = (from field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                         where field.HasAttribute<AnimatorParamAttribute>()
                         select field).ToArray();
        _paramsInfo.ForEach(param =>
        {
            Debug.Assert(
                param.FieldType == typeof(bool) ||
                param.FieldType == typeof(float) ||
                param.FieldType == typeof(int) ||
                param.FieldType == typeof(AnimatorTrigger),
                "AnimatorParam的类型必须是bool || float || int || AnimatorTrigger!");
        });
        s_paramsInfoCache[type] = _paramsInfo;
    }

    public void UpdateAnimator()
    {
        foreach (var paramInfo in _paramsInfo)
        {
            var attr = paramInfo.GetAttribute<AnimatorParamAttribute>();
            var paramName = attr.ParamName == string.Empty ? paramInfo.Name : attr.ParamName;
            var paramId = Animator.StringToHash(paramName);
            var value = paramInfo.GetValue(_target);

            if (value is float f)
            {
                Animator.SetFloat(paramId, f);
            }
            else if (value is bool b)
            {
                Animator.SetBool(paramId, b);
            }
            else if (value is int i)
            {
                Animator.SetInteger(paramId, i);
            }
            else if (value is AnimatorTrigger trigger)
            {
                if (trigger.IsTriggered)
                    Animator.SetTrigger(paramId);
            }
        }
    }
}
