using System;
using System.Collections;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Tool
{
    public enum FeedbackStates
    {
        Idle,
        Start,
        DelayBeforePlay,
        Playing,
        Stop
    }

    public abstract class Feedback
    {
        public enum InitializeModes
        {
            Awake,
            Start,
            FirstPlay
        }

        [FoldoutGroup("Feedback Settings")]
        public string Label = "Feedback";

        [FoldoutGroup("Feedback Settings")]
        public InitializeModes InitializeMode = InitializeModes.Start;

        [Title("Time")]
        [FoldoutGroup("Feedback Settings")]
        public float DelayBeforePlay;
        [FoldoutGroup("Feedback Settings")]
        public float Duration = 1f;

        [Title("Local Transform")]
        [FoldoutGroup("Feedback Settings")]
        public Vector3 LocalPosition;
        [FoldoutGroup("Feedback Settings")]
        public Vector3 LocalRotation;
        [FoldoutGroup("Feedback Settings")]
        public Vector3 LocalScale = Vector3.one;

        public GameObject Owner { get; private set; }
        public FeedbackManager OwnerManager { get; private set; }

        public FSM<FeedbackStates> FSM { get; set; } = new();

        protected bool IsInitialized = false;

        protected float TimeSinceFeedbackStart = 0;

        public virtual void Awake()
        {
            if (InitializeMode == InitializeModes.Awake)
            {
                Initialize();
            }
        }

        public virtual void Start()
        {
            if (InitializeMode == InitializeModes.Start)
            {
                Initialize();
            }
        }

        public virtual void Update()
        {
            FSM.Update();
        }

        public virtual void Setup(GameObject owner, FeedbackManager ownerManager)
        {
            Owner = owner;
            OwnerManager = ownerManager;
        }

        public void StartCoroutine(IEnumerator routine)
        {
            OwnerManager.StartCoroutine(routine);
        }

        public virtual void Play()
        {
            if (!IsInitialized)
            {
                if (InitializeMode == InitializeModes.FirstPlay)
                {
                    Initialize();
                }
                else
                {
                    throw new Exception("Feedback还未初始化!");
                }
            }
            FSM.ChangeState(FeedbackStates.Start);
        }

        public virtual void Stop()
        {
            FSM.ChangeState(FeedbackStates.Stop);
        }

        protected virtual void Initialize()
        {
            FSM.State(FeedbackStates.Idle);

            FSM.State(FeedbackStates.Start).OnEnter(() =>
            {
                OnFeedbackStart();
                FSM.ChangeState(FeedbackStates.DelayBeforePlay);
            });

            FSM.State(FeedbackStates.DelayBeforePlay).OnEnter(() => StartCoroutine(FeedbackDelayBeforePlayCo()));

            FSM.State(FeedbackStates.Playing).OnUpdate(CaseFeedbackPlaying);

            FSM.State(FeedbackStates.Stop).OnEnter(() =>
            {
                if (FSM.CurrentStateId is FeedbackStates.Idle or FeedbackStates.Stop)
                    return;
                OnFeedbackStop();
                FSM.ChangeState(FeedbackStates.Idle);
            });
            FSM.StartState(FeedbackStates.Idle);

            OnFeedbackInit();
        }

        protected virtual void CaseFeedbackPlaying()
        {
            if (Time.time - TimeSinceFeedbackStart < Duration)
            {
                OnFeedbackPlaying();
            }
            else
            {
                FSM.ChangeState(FeedbackStates.Stop);
            }
        }


        protected virtual IEnumerator FeedbackDelayBeforePlayCo()
        {
            yield return new WaitForSeconds(DelayBeforePlay);
            TimeSinceFeedbackStart = Time.time;
            OnFeedbackStart();
            FSM.ChangeState(FeedbackStates.Playing);
        }

        protected virtual void ApplyTransform(Transform transform)
        {
            transform.SetParent(OwnerManager.transform);
            transform.SetLocalPositionAndRotation(LocalPosition, Quaternion.Euler(LocalRotation));
            transform.localScale = LocalScale;
        }

        protected virtual void OnFeedbackInit() { }

        protected virtual void OnFeedbackStart() { }

        protected virtual void OnFeedbackPlaying() { }

        protected virtual void OnFeedbackStop() { }

        public virtual void OnDestroy()
        {
            FSM.Clear();
        }

#if UNITY_EDITOR
        private string GetLabel()
        {
            return $"{Label} ({Duration}s)";
        }
#endif
    }
}
