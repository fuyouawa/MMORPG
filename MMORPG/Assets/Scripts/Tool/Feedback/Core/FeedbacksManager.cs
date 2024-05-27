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
            public enum CheckModes
            {
                OnAwake,
                OnStart,
                OnUpdate
            }

            public CheckModes CheckMode = CheckModes.OnStart;
            public bool Negative;
            [HideLabel]
            public ValuePicker<bool> Picker;
        }
        [DetailedInfoBox("@_help.Message", "@_help.Details", VisibleIf = "ShowDetailedInfoBox")]
        [HideLabel]
        [ValueDropdown("GetFeedbackNamesDropdown")]
        public string FeedbackName = string.Empty;

        [HideIf("@Feedback == null")]
        public bool Enable = true;

        [HideIf("@Feedback == null")]
        public string Label = "Feedback";

        [HideIf("@Feedback == null")]
        public bool ActiveEnableCheck = false;

        [ShowIf("ActiveEnableCheck")]
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
                s_allFeedbackDropdownItems.Add(attr.Path, kv.Value.FullName);
            }
        }

        private IEnumerable GetFeedbackNamesDropdown()
        {
            return s_allFeedbackDropdownItems;
        }


        private string GetLabel()
        {
            if (Feedback == null)
                return "Feedback";
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

        private bool ShowDetailedInfoBox()
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
                    Feedback = (AbstractFeedback)Activator.CreateInstance(s_allFeedbackTypes[FeedbackName]);
                    Feedback.Setup(Owner);
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
        public bool AutoPlayOnStart;
        [FoldoutGroup("Settings")]
        public bool AutoPlayOnEnable;

        [FoldoutGroup("Settings")]
        public bool CanPlay = true;
        [FoldoutGroup("Settings")]
        public bool CanPlayWhileAlreadyPlaying = false;
        [FoldoutGroup("Settings")]
        public bool StopAllCoroutinesWhenStop = true;

        [LabelText("Feedbacks")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetLabel")]
        public List<FeedbackItem> FeedbackItems = new();

        public GameObject Owner { get; private set; }
        public bool IsInitialized { get; private set; }

        public FeedbacksCoroutineHelper CoroutineHelper { get; private set; }

        public bool CheckIsPlaying()
        {
            return false;
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

            if (!CanPlayWhileAlreadyPlaying)
            {
                if (CheckIsPlaying())
                    return;
            }
            foreach (var item in FeedbackItems)
            {
                item.Play();
            }
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
