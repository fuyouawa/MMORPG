using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace MMORPG.Tool
{
    public class AddFeedbackMenuAttribute : Attribute
    {
        public string Path;
        public string Comment;

        public AddFeedbackMenuAttribute(string path, string comment = null)
        {
            Path = path;
            Comment = comment;
        }
    }

    public class FeedbackHelpAttribute : Attribute
    {
        public string Message;

        public FeedbackHelpAttribute(string message)
        {
            Message = message;
        }
    }


    [Serializable]
    public abstract class AbstractFeedback
    {
        public enum InitializeModes
        {
            Awake,
            Start,
            FirstPlay
        }

        [FoldoutGroup("Feedback Settings")]
        [Title("Time")]
        [Tooltip("在正式Play前经过多少时间的延迟(s)")]
        public float DelayBeforePlay;

        [FoldoutGroup("Feedback Settings")]
        [Title("Loop")]
        [Tooltip("是否要循环Play")]
        public bool LoopPlay = false;

        [FoldoutGroup("Feedback Settings")]
        [ShowIf("LoopPlay")]
        [Tooltip("是否限制循环Play的次数")]
        public bool LimitLoopAmount = false;

        [FoldoutGroup("Feedback Settings")]
        [ShowIf("@LoopPlay && LimitLoopAmount")]
        [Tooltip("循环Play的次数")]
        public int AmountOfLoop = 1;

        [FoldoutGroup("Feedback Settings")]
        [ShowIf("LoopPlay")]
        [Tooltip("每次循环Play的间隔")]
        public float DelayBetweenLoop = 0f;

        public FeedbacksManager Owner { get; private set; }
        public bool IsPlaying { get; private set; }

        public Transform Transform => Owner.transform;

        protected bool IsInitialized = false;
        protected int CurrentLoopCount;

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Reset()
        {
            if (LoopPlay && LimitLoopAmount)
            {
                CurrentLoopCount = AmountOfLoop;
            }
            OnFeedbackReset();
        }

        public virtual void Play()
        {
            Reset();
            IsPlaying = true;
            StartCoroutine(FeedbackPlayCo());
        }

        public virtual void Stop()
        {
            if (!IsPlaying)
                return;
            IsPlaying = false;
            OnFeedbackStop();
        }

        public virtual void Setup(FeedbacksManager owner)
        {
            Owner = owner;
        }

        public virtual void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            OnFeedbackInit();
        }


        protected virtual IEnumerator FeedbackPlayCo()
        {
            yield return new WaitForSeconds(DelayBeforePlay);
            StartCoroutine(DurationCoroutine());
            OnFeedbackPlay();
        }

        protected virtual IEnumerator DurationCoroutine()
        {
            yield return new WaitForSeconds(GetDuration());
            if (!IsPlaying)
                yield break;
            if (LoopPlay)
            {
                if (LimitLoopAmount)
                {
                    if (CurrentLoopCount == 0)
                        goto stop;
                    CurrentLoopCount--;
                }
                StartCoroutine(FeedbackPlayCo());
                yield break;
            }
            stop:
            Stop();
        }

        public virtual void OnDrawGizmos() {}

        public virtual void OnDrawGizmosSelected() {}

        protected virtual void OnFeedbackInit() { }

        protected virtual void OnFeedbackReset() { }

        protected abstract void OnFeedbackPlay();

        protected abstract void OnFeedbackStop();

        public virtual void OnDestroy() {}

        public virtual float GetDuration()
        {
            return 0;
        }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            return Owner.CoroutineHelper.StartCoroutine(routine);
        }

        protected void StopCoroutine(IEnumerator routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        protected void StopCoroutine(Coroutine routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        protected void StopAllCoroutine()
        {
            Owner.CoroutineHelper.StopAllCoroutines();
        }

#if UNITY_EDITOR
        public virtual void OnValidate() { }
        public virtual void OnInspectorInit() { }
        public virtual void OnInspectorGUI() {}
#endif
    }
}
