using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections;

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
            public ValueGetter<bool> Getter;
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
}
