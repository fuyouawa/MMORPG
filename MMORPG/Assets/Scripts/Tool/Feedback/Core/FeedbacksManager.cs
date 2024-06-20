using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    public class FeedbacksManager : SerializedMonoBehaviour
    {
        public enum InitializationModes { Awake, Start }
        [FoldoutGroup("Settings")]
        public InitializationModes InitializationMode = InitializationModes.Awake;

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

        [LabelText("Feedbacks")]
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<FeedbackItem> FeedbackItems = new();

        public GameObject Owner { get; private set; }
        public bool IsInitialized { get; private set; }

        public FeedbacksCoroutineHelper CoroutineHelper { get; private set; }

        public float TimeSinceLastPlay { get; private set; }

        public bool HasFeedbackPlaying()
        {
            foreach (var item in FeedbackItems)
            {
                if (item.Enable && item.Feedback?.IsPlaying == true)
                {
                    return true;
                }
            }
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
        }

        protected virtual void Start()
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

        protected virtual void Update()
        {
            foreach (var item in FeedbackItems)
            {
                item.Update();
            }
        }

        protected virtual void OnEnable()
        {
            foreach (var item in FeedbackItems)
            {
                item.OnEnable();
            }

            if (AutoPlayOnEnable)
            {
                Play();
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var item in FeedbackItems)
            {
                item.OnDisable();
            }
        }

        public virtual void Play()
        {
            if (!IsInitialized && AutoInitialization)
            {
                Initialize();
            }

            if (!CanPlay) return;

            if (HasFeedbackPlaying())
            {
                if (!CanPlayWhileAlreadyPlaying)
                    return;
                if (StopCurrentPlayIfNewPlay)
                {
                    Stop();
                    Debug.Assert(!HasFeedbackPlaying());
                }
            }

            foreach (var item in FeedbackItems)
            {
                item.Play();
            }
            TimeSinceLastPlay = Time.time;
        }

        public virtual void Stop()
        {
            if (!IsInitialized)
            {
                return;
            }
            foreach (var item in FeedbackItems)
            {
                item.Stop();
            }
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

        protected virtual void Initialize()
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

        public void OnSceneGUI()
        {
            foreach (var item in FeedbackItems)
            {
                item.OnSceneGUI();
            }
        }
#endif
    }
    public class FeedbacksCoroutineHelper : MonoBehaviour { }
}
