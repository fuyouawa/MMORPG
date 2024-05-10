using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace MMORPG.Tool
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AnimatorParamAttribute : Attribute
    {
        public bool AutoName { get; }
        public string Name;

        public AnimatorParamAttribute()
        {
            Name = string.Empty;
            AutoName = true;
        }

        public AnimatorParamAttribute(string name)
        {
            Name = name;
            AutoName = false;
        }
    }

    public struct AnimatorTrigger
    {
        public bool Triggered { get; set; }
        public void Trigger() => Triggered = true;
    }

    public class AnimatorParamsAutoUpdater : MonoBehaviour
    {
        private static Dictionary<Type, PropertyInfo[]> s_paramsInfoCache = new();

        public Animator CurrentAnimator { get; private set; }
        public object Target { get; private set; }

        private PropertyInfo[] _paramsInfo;

        public bool Running { get; private set; }
        private bool _prepareStop = false;

        public void Setup(object target, Animator animator)
        {
            Target = target;
            CurrentAnimator = animator;
            InitParamsInfo();
        }

        private void InitParamsInfo()
        {
            var type = Target.GetType();
            if (s_paramsInfoCache.TryGetValue(type, out _paramsInfo))
                return;
            _paramsInfo =
                (from field in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
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

        public void Run()
        {
            Running = true;
        }

        public void Stop()
        {
            Running = false;
        }

        public void StopInNextFrame()
        {
            _prepareStop = true;
        }

        public void UpdateAnimator()
        {
            if (!Running) return;

            foreach (var paramInfo in _paramsInfo)
            {
                var attr = paramInfo.GetAttribute<AnimatorParamAttribute>();
                var paramName = attr.AutoName ? paramInfo.Name : attr.Name;
                var paramId = Animator.StringToHash(paramName);
                var value = paramInfo.GetValue(Target);

                if (value is float f)
                {
                    CurrentAnimator.SetFloat(paramId, f);
                }
                else if (value is bool b)
                {
                    CurrentAnimator.SetBool(paramId, b);
                }
                else if (value is int i)
                {
                    CurrentAnimator.SetInteger(paramId, i);
                }
                else if (value is AnimatorTrigger trigger)
                {
                    if (trigger.Triggered)
                    {
                        CurrentAnimator.SetTrigger(paramId);
                        trigger.Triggered = false;
                    }
                }
                else
                    throw new Exception("AnimatorParam的类型必须是bool || float || int || AnimatorTrigger!");
            }

            if (_prepareStop)
            {
                Running = false;
                _prepareStop = false;
            }
        }

        private void OnApplicationQuit()
        {
            s_paramsInfoCache.Clear();
        }
    }

    public class AnimatorMachine
    {
        private readonly object _target;
        private readonly GameObject _targetObj;
        private readonly Animator _animator;

        private AnimatorParamsAutoUpdater _updater;

        public AnimatorMachine(object target, GameObject targetObj, Animator animator)
        {
            _target = target;
            _targetObj = targetObj;
            _animator = animator;
        }

        public void Run()
        {
            if (_updater == null)
            {
                if (!_targetObj.TryGetComponent(out _updater))
                {
                    _updater = _targetObj.AddComponent<AnimatorParamsAutoUpdater>();
                }
            }

            //TODO Debug.Assert
            Debug.Assert(!_updater.Running);

            if (_updater.CurrentAnimator != _animator || _updater.Target != _target)
            {
                _updater.Setup(_target, _animator);
            }

            _updater.Run();
        }

        public void Stop()
        {
            _updater.Stop();
        }

        public void StopInNextFrame()
        {
            _updater.StopInNextFrame();
        }
    }
}
