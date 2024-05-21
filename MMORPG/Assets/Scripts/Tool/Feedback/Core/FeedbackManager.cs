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
    public class FeedbackCoroutineHelper : MonoBehaviour { }

    public class FeedbackManager : SerializedMonoBehaviour
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
        public bool CanPlayWhileAlreadyPlaying = true;

        [Required]
        [LabelText("$GetFeedbacksLabel")]
        [LabelWidth(250)]
        [ValueDropdown("GetFeedbacksDropdown")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetLabel")]
        public Feedback[] Feedbacks = Array.Empty<Feedback>();

        public GameObject Owner { get; private set; }
        public bool IsInitialized { get; private set; }

        public FeedbackCoroutineHelper CoroutineHelper { get; private set; }

        public bool CheckIsPlaying()
        {
            return Feedbacks.Any(x => x.IsPlaying);
        }

        public void Setup(GameObject owner)
        {
            Owner = owner;
        }

        private void Awake()
        {
            if (!TryGetComponent(out FeedbackCoroutineHelper coroutineHelper))
            {
                coroutineHelper = gameObject.AddComponent<FeedbackCoroutineHelper>();
            }
            CoroutineHelper = coroutineHelper;
            if (InitializationMode == InitializationModes.Awake)
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (InitializationMode == InitializationModes.Start)
            {
                Initialize();
            }

            if (AutoPlayOnStart)
            {
                Play();
            }
        }

        private void OnEnable()
        {
            if (AutoPlayOnEnable)
            {
                Play();
            }
        }

        public void Play()
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

        public void Stop()
        {
            Feedbacks?.ForEach(x => x.Stop());
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

        protected virtual void Initialize()
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
        [Button]
        private void TestPlay()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "只能在运行时测试!", "确定");
                return;
            }
            Play();
        }

        [HorizontalGroup]
        [Button]
        private void TestStop()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "只能在运行时测试!", "确定");
                return;
            }
            Stop();
        }

        private static Type[] s_allFeedbackTypes;

        static FeedbackManager()
        {
            s_allFeedbackTypes = (from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(Feedback).IsAssignableFrom(t) && t != typeof(Feedback) && t.HasAttribute<AddFeedbackMenuAttribute>()
                select t).ToArray();
        }

        private IEnumerable GetFeedbacksDropdown()
        {
            var total = new ValueDropdownList<Feedback>();
            foreach (var type in s_allFeedbackTypes)
            {
                var attr = type.GetAttribute<AddFeedbackMenuAttribute>();
                Debug.Assert(attr != null);
                var inst = (Feedback)Activator.CreateInstance(type);
                inst.Setup(this);
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
}