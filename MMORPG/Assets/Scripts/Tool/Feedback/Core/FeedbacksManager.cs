using System.Collections.Generic;
using System.Linq;
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
