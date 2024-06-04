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
        [ListDrawerSettings(ShowIndexLabels = true)]
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
                InnerInitialize();
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
                InnerInitialize();
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
                InnerInitialize();
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

        /// <summary>
        /// 添加Feedback
        /// </summary>
        /// <example>
        /// <para>以下这个示例展示了如何使用该Api添加一个伤害区域的反馈</para>
        /// <code>
        /// // 创建一块伤害区域
        /// var damageAreaFeedback = new FeedbackDamageArea()
        /// {
        ///     DelayBeforePlay = 0.3f,
        ///     ActiveDuration = 0.5f,
        ///     AreaOffset = new Vector3(1.1f, 4.5f, 1.4f),
        ///     AreaSize = new Vector3(1.9f, 1.9f, 8.10f),
        ///     CustomLayer = true,
        ///     LayerName = "Weapon",
        ///     TargetLayerMask = LayerMask.NameToLayer("Monster"),
        ///     MinDamageCaused = 114514f,
        ///     MaxDamageCaused = 1919810f
        /// };
        /// // 监听OnHitDamageable, 以便在击打到对象的时候播放击打音效等
        /// damageAreaFeedback.OnHitDamageable.AddListener(HandleWeapon.OnHitDamageable);
        /// // 将伤害区域添加到Feedbacks中
        /// WeaponStartFeedbacks.AddFeedback(
        ///     damageAreaFeedback,
        ///     // 在IsMine的时候激活当前添加的Feedback, 也就是伤害区域
        ///     enableIf: () => Brain.IsMine,
        ///     // 在Feedback的OnStart时判定传入的enableIf是否成立, 如果不成立就Disable
        ///     conditionPredicateModes: ConditionPredicateModes.OnStart);
        /// </code>
        /// </example>
        /// <param name="feedback">要添加的Api</param>
        /// <param name="enable">激活状态</param>
        /// <param name="enableIf">激活条件, 如果填null就是没有激活条件</param>
        /// <param name="conditionPredicateModes">激活条件的判定模式</param>
        public virtual void AddFeedback(
            AbstractFeedback feedback,
            bool enable = true,
            Func<bool> enableIf = null,
            ConditionPredicateModes conditionPredicateModes = ConditionPredicateModes.OnStart)
        {
            var item = new FeedbackItem
            {
                Feedback = feedback,
                Enable = enable,
                FeedbackName = feedback.GetType().FullName,
                Label = feedback.GetType().Name
            };
            if (enableIf != null)
            {
                item.ActiveEnablePredicate = true;
                item.EnableIf ??= new();
                item.EnableIf.Mode = conditionPredicateModes;
                item.EnableIf.AlternativeGetter = enableIf;
            }

            FeedbackItems.Add(item);
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
            if (InitializationMode == InitializationModes.Script)
            {
                InnerInitialize();
            }
        }

        protected virtual void InnerInitialize()
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
