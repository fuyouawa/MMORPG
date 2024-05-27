using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [Serializable]
    public class FeedbackItem
    {
        [Serializable]
        public class Condition
        {
            public enum Modes
            {
                OnAwake,
                OnStart,
                OnUpdate
            }

            [Tooltip("OnAwake: 在Awake时判定\nOnStart: 在Start时判定\nOnUpdate: 每帧判定")]
            public Modes Mode = Modes.OnStart;
            [Tooltip("是否将判定结果取反")]
            public bool Negative;
            [HideLabel]
            public ValuePicker<bool> Picker;
        }
        [Information("@_help.Message", VisibleIf = "ShowInfoBox")]
        [HideLabel]
        [ValueDropdown("GetFeedbackNamesDropdown")]
        public string FeedbackName = string.Empty;

        [Tooltip("是否有效")]
        [HideIf("@Feedback == null")]
        public bool Enable = true;

        [Tooltip("只在编辑器中有用, 指定Feedback的名称")]
        [HideIf("@Feedback == null")]
        public string Label;

        [Tooltip("是否启动运行时Enable判定")]
        [HideIf("@Feedback == null")]
        public bool ActiveEnablePredicate = false;

        [Tooltip("选择一个变量或者函数, 用于在运行时判定是否要Disable")]
        [ShowIf("ActiveEnablePredicate")]
        [HideReferenceObjectPicker]
        public Condition DisableIf = new();

        [BoxGroup("@FeedbackName"), HideLabel]
        [HideIf("@Feedback == null")]
        [SerializeReference]
        [HideReferenceObjectPicker]
        [EnableIf("Enable")]
        public AbstractFeedback Feedback;

        public FeedbacksManager Owner { get; private set; }
        

        public void Setup(FeedbacksManager owner)
        {
            Owner = owner;
            Feedback?.Setup(owner);
        }

        public void Awake()
        {
            if (!Enable) return;
            Feedback?.Awake();
        }

        public void Start()
        {
            if (!Enable) return;
            Feedback?.Start();
        }

        public void Update()
        {
            if (!Enable) return;
            Feedback?.Update();
        }

        public void Play()
        {
            if (!Enable) return;
            Feedback?.Play();
        }

        public void Stop()
        {
            if (!Enable) return;
            Feedback?.Stop();
        }

        public void OnDestroy()
        {
            Feedback?.OnDestroy();
        }

        public void OnDrawGizmosSelected()
        {
            if (!Enable) return;
            Feedback?.OnDrawGizmosSelected();
        }

        public void OnDrawGizmos()
        {
            if (!Enable) return;
            Feedback?.OnDrawGizmos();
        }

        public void Initialize()
        {
            if (!Enable) return;
            Feedback?.Initialize();
        }


#if UNITY_EDITOR
        private static Dictionary<string, Type> s_allFeedbackTypes;
        private static ValueDropdownList<string> s_allFeedbackDropdownItems = new();

        static FeedbackItem()
        {
            s_allFeedbackTypes = (from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(AbstractFeedback).IsAssignableFrom(t) && t != typeof(AbstractFeedback) && t.HasCustomAttribute<AddFeedbackMenuAttribute>()
                select t).ToDictionary(x => x.FullName, y => y);

            s_allFeedbackDropdownItems.Clear();
            s_allFeedbackDropdownItems.Add("None", string.Empty);
            foreach (var kv in s_allFeedbackTypes)
            {
                var attr = kv.Value.GetCustomAttribute<AddFeedbackMenuAttribute>();
                var comment = string.IsNullOrEmpty(attr.Comment) ? string.Empty : $"\t## {attr.Comment}";
                s_allFeedbackDropdownItems.Add($"{attr.Path}{comment}", kv.Value.FullName);
            }
        }

        private IEnumerable GetFeedbackNamesDropdown()
        {
            return s_allFeedbackDropdownItems;
        }


        private string GetLabel()
        {
            if (Feedback == null || string.IsNullOrEmpty(Label))
                return "TODO";
            var duration = Feedback.GetDuration();

            var timeDisplay = $"{Feedback.DelayBeforePlay:0.00}s + {duration:0.00}s";
            if (Feedback.LoopPlay)
            {
                var loopCountDisplay = Feedback.LimitLoopAmount ? Feedback.AmountOfLoop.ToString() : "\u221e";
                if (Feedback.DelayBetweenLoop > float.Epsilon)
                {
                    timeDisplay += $" + {Feedback.DelayBetweenLoop:0.00}s";
                }
                return $"{Label} ({timeDisplay}) x {loopCountDisplay}";
            }
            else
            {
                return $"{Label} ({timeDisplay})";
            }
        }

        private FeedbackHelpAttribute _help;

        private bool ShowInfoBox()
        {
            return Feedback != null && _help != null;
        }

        public void OnValidate()
        {
            Feedback?.OnValidate();
        }

        public void OnInspectorInit()
        {
            Feedback?.OnInspectorInit();
        }

        public void OnInspectorGUI()
        {
            if (!string.IsNullOrEmpty(FeedbackName))
            {
                if ((Feedback == null || Feedback.GetType().FullName != FeedbackName) && Owner != null)
                {
                    var type = s_allFeedbackTypes[FeedbackName];
                    Feedback = (AbstractFeedback)Activator.CreateInstance(type);
                    Feedback.Setup(Owner);
                    if (string.IsNullOrEmpty(Label))
                    {
                        var attr = type.GetCustomAttribute<AddFeedbackMenuAttribute>();
                        Label = attr.Path.Split('/').Last();
                    }
                }
            }
            else
            {
                Feedback = null;
            }

            _help ??= Feedback?.GetType().GetCustomAttribute<FeedbackHelpAttribute>();

            Feedback?.OnInspectorGUI();
        }

        [ButtonGroup]
        [DisableInEditorMode]
        private void TestPlay()
        {
            Play();
        }

        [ButtonGroup]
        [DisableInEditorMode]
        private void TestStop()
        {
            Stop();
        }
#endif
    }

    public class FeedbacksManager : SerializedMonoBehaviour
    {
        public enum InitializationModes { Script, Awake, Start }
        [FoldoutGroup("Settings")]
        public InitializationModes InitializationMode = InitializationModes.Start;

        [FoldoutGroup("Settings")]
        [Tooltip("确保Play前所有Feedbacks都初始化")]
        public bool AutoInitialization = true;

        [FoldoutGroup("Settings")]
        [Tooltip("在Start时自动Play一次")]
        public bool AutoPlayOnStart;
        [FoldoutGroup("Settings")]
        [Tooltip("在OnEnable时自动Play一次")]
        public bool AutoPlayOnEnable;

        [FoldoutGroup("Settings")]
        [Tooltip("是否可以Play")]
        public bool CanPlay = true;
        [FoldoutGroup("Settings")]
        [Tooltip("在当前Play还没结束时是否可以开始新的Play")]
        public bool CanPlayWhileAlreadyPlaying = false;
        [FoldoutGroup("Settings")]
        [ShowIf("CanPlayWhileAlreadyPlaying")]
        [Tooltip("在当前Play还没结束时, 如果有新的Play, 是否要结束当前Play")]
        public bool StopCurrentPlayIfNewPlay = true;
        [FoldoutGroup("Settings")]
        [Tooltip("当Play结束时, 是否要停止所有在Feedback中开启的Coroutines")]
        public bool StopAllCoroutinesWhenStop = true;

        [LabelText("Feedbacks")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetLabel")]
        public List<FeedbackItem> FeedbackItems = new();

        public GameObject Owner { get; private set; }
        public bool IsInitialized { get; private set; }

        public FeedbacksCoroutineHelper CoroutineHelper { get; private set; }

        public float TimeSinceLastPlay { get; private set; }

        public bool CheckIsPlaying()
        {
            return !FeedbackItems.Any(x => !x.Feedback.IsPlaying);
        }

        public void Setup(GameObject owner)
        {
            Owner = owner;
        }

        protected virtual void Awake()
        {
            if (!TryGetComponent(out FeedbacksCoroutineHelper coroutineHelper))
            {
                coroutineHelper = gameObject.AddComponent<FeedbacksCoroutineHelper>();
            }
            CoroutineHelper = coroutineHelper;
            if (InitializationMode == InitializationModes.Awake)
            {
                Initialize();
            }

            foreach (var item in FeedbackItems)
            {
                item.Awake();
            }
        }

        protected virtual void Start()
        {
            if (InitializationMode == InitializationModes.Start)
            {
                Initialize();
            }


            foreach (var item in FeedbackItems)
            {
                item.Start();
            }

            if (AutoPlayOnStart)
            {
                Play();
            }
        }

        protected virtual void Update()
        {
            foreach (var item in FeedbackItems)
            {
                item.Update();
            }
        }

        protected virtual void OnEnable()
        {
            if (AutoPlayOnEnable)
            {
                Play();
            }
        }

        public virtual void Play()
        {
            if (!IsInitialized && AutoInitialization)
            {
                Initialize();
            }

            if (!CanPlay) return;

            if (CheckIsPlaying())
            {
                if (!CanPlayWhileAlreadyPlaying)
                    return;
                if (StopCurrentPlayIfNewPlay)
                    Stop();
            }

            foreach (var item in FeedbackItems)
            {
                item.Play();
            }
            TimeSinceLastPlay = Time.time;
        }

        public virtual void Stop()
        {
            Debug.Assert(IsInitialized);
            foreach (var item in FeedbackItems)
            {
                item.Stop();
            }
            if (StopAllCoroutinesWhenStop)
                CoroutineHelper.StopAllCoroutines();
        }

        public virtual void ProxyDestroy(GameObject gameObjectToDestroy, float delay)
        {
            Destroy(gameObjectToDestroy, delay);
        }

        protected virtual void OnDestroy()
        {
            foreach (var item in FeedbackItems)
            {
                item.OnDestroy();
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            foreach (var item in FeedbackItems)
            {
                item.OnDrawGizmosSelected();
            }
        }

        protected virtual void OnDrawGizmos()
        {
            foreach (var item in FeedbackItems)
            {
                item.OnDrawGizmos();
            }
        }

        public virtual void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            foreach (var item in FeedbackItems)
            {
                item.Setup(this);
                item.Initialize();
            }
        }


#if UNITY_EDITOR

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            foreach (var item in FeedbackItems)
            {
                item.Setup(this);
                item.OnInspectorInit();
            }
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            foreach (var item in FeedbackItems)
            {
                item.Setup(this);
                item.OnInspectorGUI();
            }
        }

        protected virtual void OnValidate()
        {
            foreach (var item in FeedbackItems)
            {
                item.OnValidate();
            }
        }

        [ButtonGroup]
        [DisableIf("@IsInitialized || !UnityEditor.EditorApplication.isPlaying")]
        private void TestInit()
        {
            Initialize();
        }

        [ButtonGroup]
        [DisableInEditorMode]
        private void TestPlay()
        {
            Play();
        }

        [ButtonGroup]
        [DisableInEditorMode]
        private void TestStop()
        {
            Stop();
        }
#endif
    }
    public class FeedbacksCoroutineHelper : MonoBehaviour { }
}
