using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
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
        
        [LabelText("$GetFeedbacksLabel")]
        [LabelWidth(250)]
        [ValueDropdown("GetFeedbacksDropdown")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetLabel")]
        [SerializeReference]
        public AbstractFeedback[] Feedbacks = Array.Empty<AbstractFeedback>();

        public GameObject Owner { get; private set; }
        public bool IsInitialized { get; private set; }

        public FeedbacksCoroutineHelper CoroutineHelper { get; private set; }

        public bool CheckIsPlaying()
        {
            return Feedbacks.Any(x => x.IsPlaying);
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

            Feedbacks.ForEach(x => x.Awake());
        }

        protected virtual void Start()
        {
            if (InitializationMode == InitializationModes.Start)
            {
                Initialize();
            }

            Feedbacks.ForEach(x => x.Start());

            if (AutoPlayOnStart)
            {
                Play();
            }
        }

        protected virtual void Update()
        {
            Feedbacks.ForEach(x => x.Update());
        }

        protected virtual void OnEnable()
        {
            if (AutoPlayOnEnable)
            {
                Play();
            }
        }

        protected virtual void OnValidate()
        {
            Feedbacks.ForEach(x => x.OnValidate());
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
            Feedbacks?.ForEach(x => x.Play());
        }

        public virtual void Stop()
        {
            Debug.Assert(IsInitialized);
            Feedbacks?.ForEach(x => x.Stop());
            if (StopAllCoroutinesWhenStop)
                CoroutineHelper.StopAllCoroutines();
        }

        public virtual void ProxyDestroy(GameObject gameObjectToDestroy, float delay)
        {
            Destroy(gameObjectToDestroy, delay);
        }

        protected virtual void OnDestroy()
        {
            Feedbacks?.ForEach(x => x.OnDestroy());
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Feedbacks?.ForEach(x => x.OnDrawGizmosSelected());
        }

        protected virtual void OnDrawGizmos()
        {
            Feedbacks?.ForEach(x => x.OnDrawGizmos());
        }

        public virtual void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            Feedbacks?.ForEach(x =>
            {
                x.Setup(this);
                x.Initialize();
            });
        }


#if UNITY_EDITOR
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            Feedbacks.ForEach(x => x.Setup(this));
        }

        [HorizontalGroup(Title = "Test (Only in Playing)")]
        [Button("Init")]
        [DisableIf("@IsInitialized || !UnityEditor.EditorApplication.isPlaying")]
        private void TestInit()
        {
            Initialize();
        }

        [HorizontalGroup]
        [Button("Play")]
        [DisableInEditorMode]
        private void TestPlay()
        {
            Play();
        }

        [HorizontalGroup]
        [Button("Stop")]
        [DisableInEditorMode]
        private void TestStop()
        {
            Stop();
        }

        private static Type[] s_allFeedbackTypes;

        static FeedbacksManager()
        {
            s_allFeedbackTypes = (from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(AbstractFeedback).IsAssignableFrom(t) && t != typeof(AbstractFeedback) && t.HasAttribute<AddFeedbackMenuAttribute>()
                select t).ToArray();
        }

        private IEnumerable GetFeedbacksDropdown()
        {
            var total = new ValueDropdownList<AbstractFeedback>();
            foreach (var type in s_allFeedbackTypes)
            {
                var attr = type.GetAttribute<AddFeedbackMenuAttribute>();
                Debug.Assert(attr != null);
                var inst = (AbstractFeedback)Activator.CreateInstance(type);
                inst.Setup(this);
                inst.Label = type.Name.StartsWith("Feedback") ? type.Name[8..] : type.Name;
                total.Add(attr.Path, inst);
            }

            return total;
        }

        private string GetFeedbacksLabel()
        {
            return $"Feedbacks {(CanPlay ? "" : "[Can't Play]")}";
        }
#endif
    }
    public class FeedbacksCoroutineHelper : MonoBehaviour { }
}
