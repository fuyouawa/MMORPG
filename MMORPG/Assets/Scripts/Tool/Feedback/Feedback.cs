using System;
using System.Collections;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    public class AddFeedbackMenuAttribute : Attribute
    {
        public string Path;

        public AddFeedbackMenuAttribute(string path)
        {
            Path = path;
        }
    }

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

        public FeedbackManager Owner { get; private set; }

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

        public virtual void Setup(FeedbackManager owner)
        {
            Owner = owner;
        }

        public void StartCoroutine(IEnumerator routine)
        {
            Owner.StartCoroutine(routine);
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

        public virtual void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            FSM ??= new();

            FSM.State(FeedbackStates.Idle);

            FSM.State(FeedbackStates.Start).OnEnter(() =>
            {
                FSM.ChangeState(FeedbackStates.DelayBeforePlay);
            });

            FSM.State(FeedbackStates.DelayBeforePlay).OnEnter(() => StartCoroutine(FeedbackDelayBeforePlayCo()));

            FSM.State(FeedbackStates.Playing).OnUpdate(CaseFeedbackPlaying);

            FSM.State(FeedbackStates.Stop).OnEnter(() =>
            {
                if (FSM.CurrentStateId is FeedbackStates.Idle)
                    return;
                OnFeedbackStop();
                FSM.ChangeState(FeedbackStates.Idle);
            });

            FSM.StartState(FeedbackStates.Idle);

            OnFeedbackInit();
        }

        protected virtual void CaseFeedbackPlaying()
        {
            OnFeedbackPlaying();
        }


        protected virtual IEnumerator FeedbackDelayBeforePlayCo()
        {
            yield return new WaitForSeconds(DelayBeforePlay);
            TimeSinceFeedbackStart = Time.time;
            OnFeedbackStart();
            StartCoroutine(DurationCoroutine());
            FSM.ChangeState(FeedbackStates.Playing);
        }

        protected virtual IEnumerator DurationCoroutine()
        {
            yield return new WaitForSeconds(GetDuration());
            if (FSM.CurrentStateId is FeedbackStates.Idle or FeedbackStates.Stop)
                yield break;
            FSM.ChangeState(FeedbackStates.Stop);
        }

        protected virtual void OnFeedbackInit() { }

        protected virtual void OnFeedbackStart() { }

        protected virtual void OnFeedbackPlaying() { }

        protected virtual void OnFeedbackStop() { }

        public virtual void OnDestroy()
        {
            FSM?.Clear();
        }

        protected virtual float GetDuration()
        {
            return 0;
        }

#if UNITY_EDITOR
        private string GetLabel()
        {
            return $"{Label} ({DelayBeforePlay:0.00}s + {GetDuration():0.00}s)";
        }
#endif
    }
}
