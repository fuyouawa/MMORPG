using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

[AttributeUsage(AttributeTargets.Property)]
public class AnimatorParamAttribute : Attribute
{
    public string Name = string.Empty;
}

public struct AnimatorTrigger
{
    public bool Triggered { get; set; }
    public void Trigger() => Triggered = true;
}

public class AnimatorParamsAutoUpdater : MonoBehaviour
{
    private static Dictionary<Type, PropertyInfo[]> s_paramsInfoCache = new();

    private Animator _animator;
    private object _owner;
    private PropertyInfo[] _paramsInfo;

    public void Setup(object owner, Animator animator)
    {
        _owner = owner;
        _animator = animator;
        InitParamsInfo();
    }

    private void InitParamsInfo()
    {
        var type = _owner.GetType();
        if (s_paramsInfoCache.TryGetValue(type, out _paramsInfo))
            return;
        _paramsInfo = (from field in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                         where field.HasAttribute<AnimatorParamAttribute>()
                         select field).ToArray();
        _paramsInfo.ForEach(param =>
        {
            Debug.Assert(
                param.PropertyType == typeof(bool) ||
                param.PropertyType == typeof(float) ||
                param.PropertyType == typeof(int) ||
                param.PropertyType == typeof(AnimatorTrigger),
                "AnimatorParam的类型必须是bool || float || int || AnimatorTrigger!");
        });
        s_paramsInfoCache[type] = _paramsInfo;
    }

    private void Update()
    {
        UpdateAnimator();
    }

    public void UpdateAnimator()
    {
        foreach (var paramInfo in _paramsInfo)
        {
            var attr = paramInfo.GetAttribute<AnimatorParamAttribute>();
            var paramName = attr.Name == string.Empty ? paramInfo.Name : attr.Name;
            var paramId = Animator.StringToHash(paramName);
            var value = paramInfo.GetValue(_owner);

            if (value is float f)
            {
                _animator.SetFloat(paramId, f);
            }
            else if (value is bool b)
            {
                _animator.SetBool(paramId, b);
            }
            else if (value is int i)
            {
                _animator.SetInteger(paramId, i);
            }
            else if (value is AnimatorTrigger trigger)
            {
                if (trigger.Triggered)
                {
                    _animator.SetTrigger(paramId);
                    trigger.Triggered = false;
                }
            }
            else
                throw new Exception("AnimatorParam的类型必须是bool || float || int || AnimatorTrigger!");
        }
    }
}



public interface IAnimatorAutoUpdateParams { }

public static class AnimatorAutoUpdateParamsExtension
{
    public static void StartAnimatorAutoUpdate(this IAnimatorAutoUpdateParams owner, GameObject target)
    {
        StartAnimatorAutoUpdate(owner, target, target.GetComponent<Animator>());
    }

    public static void StartAnimatorAutoUpdate(this IAnimatorAutoUpdateParams owner, GameObject target, Animator animator)
    {
        var updater = target.AddComponent<AnimatorParamsAutoUpdater>();
        updater.Setup(owner, animator);
    }
}
