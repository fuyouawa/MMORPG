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
    [AttributeUsage(AttributeTargets.Property)]
    public class AnimatorStateSpeedAttribute : Attribute
    {
        public int LayerIndex;
        public string State;

        public AnimatorStateSpeedAttribute(int layerIndex, string state)
        {
            LayerIndex = layerIndex;
            State = state;
        }
        public AnimatorStateSpeedAttribute(string state)
            : this(0, state)
        {
        }
    }

    public struct AnimatorTrigger
    {
        public bool Triggered { get; set; }
        public void Trigger() => Triggered = true;
    }

    public class AnimatorParamsAutoUpdater : MonoBehaviour
    {
        private static Dictionary<Type, PropertyInfo[]> s_paramsPropertiesCache = new();
        private static Dictionary<Type, PropertyInfo[]> s_stateSpeedPropertiesCache = new();

        public Animator CurrentAnimator { get; private set; }
        public object Target { get; private set; }

        private PropertyInfo[] _paramsProperties;
        private PropertyInfo[] _stateSpeedProperties;

        public bool Running { get; private set; }
        private bool _prepareStop = false;

        public void Setup(object target, Animator animator)
        {
            Target = target;
            CurrentAnimator = animator;
            InitParamsProperties();
            InitStateSpeedProperties();
        }

        private void InitStateSpeedProperties()
        {
            var type = Target.GetType();
            if (s_stateSpeedPropertiesCache.TryGetValue(type, out _stateSpeedProperties))
                return;
            _stateSpeedProperties =
                (from field in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                    where field.HasAttribute<AnimatorStateSpeedAttribute>()
                    select field).ToArray();
            _stateSpeedProperties.ForEach(param =>
            {
                Debug.Assert(param.PropertyType == typeof(float), "AnimatorClipSpeed的类型必须是float");
            });
            s_stateSpeedPropertiesCache[type] = _stateSpeedProperties;
        }

        private void InitParamsProperties()
        {
            var type = Target.GetType();
            if (s_paramsPropertiesCache.TryGetValue(type, out _paramsProperties))
                return;
            _paramsProperties =
                (from field in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                    where field.HasAttribute<AnimatorParamAttribute>()
                    select field).ToArray();
            _paramsProperties.ForEach(param =>
            {
                Debug.Assert(
                    param.PropertyType == typeof(bool) ||
                    param.PropertyType == typeof(float) ||
                    param.PropertyType == typeof(int) ||
                    param.PropertyType == typeof(AnimatorTrigger),
                    "AnimatorParam的类型必须是bool || float || int || AnimatorTrigger!");
            });
            s_paramsPropertiesCache[type] = _paramsProperties;
        }

        private void Update()
        {
            if (!Running) return;

            UpdateParams();
            UpdateStateSpeeds();
        }

        private void UpdateStateSpeeds()
        {
            foreach (var prop in _stateSpeedProperties)
            {
                var attr = prop.GetAttribute<AnimatorStateSpeedAttribute>();

                var curState = CurrentAnimator.GetCurrentAnimatorStateInfo(attr.LayerIndex);
                if (curState.IsName(attr.State))
                {
                    CurrentAnimator.speed = (float)prop.GetValue(Target);
                }
            }
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

        public void UpdateParams()
        {

            foreach (var prop in _paramsProperties)
            {
                var attr = prop.GetAttribute<AnimatorParamAttribute>();
                var paramName = attr.AutoName ? prop.Name : attr.Name;
                var paramId = Animator.StringToHash(paramName);
                var value = prop.GetValue(Target);

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
            s_paramsPropertiesCache.Clear();
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
