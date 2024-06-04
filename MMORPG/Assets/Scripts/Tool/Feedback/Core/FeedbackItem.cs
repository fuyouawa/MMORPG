using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace MMORPG.Tool
{
    /// <summary>
    /// 激活条件的判定模式
    /// </summary>
    public enum ConditionPredicateModes
    {
        /// <summary>
        /// 在Awake时判定
        /// </summary>
        OnAwake,
        /// <summary>
        /// 在Start时判定
        /// </summary>
        OnStart,
        /// <summary>
        /// 每帧判定
        /// </summary>
        OnUpdate
    }

    [Serializable]
    public class FeedbackItem
    {
        [Serializable]
        public class Condition
        {
            [Tooltip("OnAwake: 在Awake时判定\nOnStart: 在Start时判定\nOnUpdate: 每帧判定")]
            public ConditionPredicateModes Mode = ConditionPredicateModes.OnStart;
            [Tooltip("是否将判定结果取反")]
            public bool Negative;
            [HideLabel]
            public ValueGetter<bool> Getter = new();

            public Func<bool> AlternativeGetter;

            public bool GetPredicate()
            {
                if (Getter is not { IsValid: true })
                {
                    return AlternativeGetter == null || AlternativeGetter();
                }
                var val = Getter.GetRawValue();
                return Negative ? !val : val;
            }
        }
        [ShowIf("@Feedback == null")]
        [HideLabel]
        [ValueDropdown("GetFeedbackNamesDropdown")]
        [Information("请选择一个Feedback", InfoMessageType.Error, VisibleIf = "NoneFeedbackName")]
        public string FeedbackName = string.Empty;

        [Information("@_help.Message", VisibleIf = "ShowInfoBox")]
        [Tooltip("是否有效")]
        public bool Enable = true;

        [Tooltip("只在编辑器中有用, 指定Feedback的名称")]
        public string Label;

        [Tooltip("是否启动运行时Enable判定")]
        public bool ActiveEnablePredicate = false;

        [Tooltip("选择一个变量或者函数, 用于在运行时判定是否要Enable")]
        [ShowIf("ActiveEnablePredicate")]
        [HideReferenceObjectPicker]
        public Condition EnableIf = new();

        [BoxGroup("@FeedbackName"), HideLabel]
        [SerializeReference]
        [HideReferenceObjectPicker]
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
            if (ActiveEnablePredicate && EnableIf is { Mode: ConditionPredicateModes.OnAwake })
            {
                Enable = EnableIf.GetPredicate();
            }
            Feedback?.Awake();
        }

        public void Start()
        {
            if (!Enable) return;
            if (ActiveEnablePredicate && EnableIf is { Mode: ConditionPredicateModes.OnStart })
            {
                Enable = EnableIf.GetPredicate();
            }
            Feedback?.Start();
        }

        public void Update()
        {
            if (!Enable) return;
            if (ActiveEnablePredicate && EnableIf is { Mode: ConditionPredicateModes.OnUpdate })
            {
                Enable = EnableIf.GetPredicate();
            }
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

        public void OnEnable()
        {
            if (!Enable) return;
            Feedback?.OnEnable();
        }

        public void OnDisable()
        {
            if (!Enable) return;
            Feedback?.OnDisable();
        }

        public void Initialize()
        {
            if (!Enable) return;
            Feedback?.Initialize();
        }


#if UNITY_EDITOR
        private static Dictionary<string, Type> s_allFeedbackTypes;
        private static ValueDropdownList<string> s_allFeedbackDropdownItems = new();

        private bool NoneFeedbackName => string.IsNullOrEmpty(FeedbackName);

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

        private FeedbackHelpAttribute _help;

        private bool ShowInfoBox()
        {
            return Feedback != null && _help != null;
        }
        

        internal void OnSceneGUI()
        {
            Feedback?.OnSceneGUI();
        }

        internal void OnValidate()
        {
            Feedback?.OnValidate();
        }

        internal void OnInspectorInit()
        {
            Feedback?.OnInspectorInit();
        }

        internal void OnInspectorGUI()
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
}
