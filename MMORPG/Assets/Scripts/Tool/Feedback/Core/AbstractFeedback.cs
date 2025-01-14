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
        [FoldoutGroup("Feedback Settings")]
        public bool StopAfterDuration = true;

        public float TotalDuration => DelayBeforePlay + GetDuration();
        public float TimeSincePlay { get; private set; }

        public FeedbacksManager Owner { get; private set; }
        public bool IsPlaying { get; private set; }

        public Transform Transform => Owner.transform;

        protected bool IsInitialized = false;
        protected int CurrentLoopCount;
        private List<Coroutine> _totalFeedbackPlayCoroutine;
        private List<Coroutine> _totalDurationCoroutine;

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
            TimeSincePlay = Time.time;
            _totalFeedbackPlayCoroutine.Add(StartCoroutine(FeedbackPlayCo()));
        }

        public virtual void Stop()
        {
            if (!IsPlaying)
                return;
            IsPlaying = false;

            foreach (var coroutine in _totalFeedbackPlayCoroutine)
            {
                StopCoroutine(coroutine);
            }
            _totalFeedbackPlayCoroutine.Clear();
            foreach (var coroutine in _totalDurationCoroutine)
            {
                StopCoroutine(coroutine);
            }
            _totalDurationCoroutine.Clear();

            _feedbackPlayCoroutineIndex = 0;

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

            _totalFeedbackPlayCoroutine = new();
            _totalDurationCoroutine = new();

            OnFeedbackInit();
        }


        private int _feedbackPlayCoroutineIndex = 0;
        protected virtual IEnumerator FeedbackPlayCo()
        {
            _feedbackPlayCoroutineIndex++;
            yield return new WaitForSeconds(DelayBeforePlay);
            _totalDurationCoroutine.Add(StartCoroutine(DurationCoroutine(_feedbackPlayCoroutineIndex)));
            OnFeedbackPlay();
        }

        protected virtual IEnumerator DurationCoroutine(int index)
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
                Play();
                yield break;
            }

            stop:
            if (StopAfterDuration && index == _feedbackPlayCoroutineIndex)
            {
                Stop();
            }
        }

        public virtual void OnDrawGizmos() {}

        public virtual void OnDrawGizmosSelected() {}

        protected virtual void OnFeedbackInit() { }

        protected virtual void OnFeedbackReset() { }

        protected abstract void OnFeedbackPlay();

        protected abstract void OnFeedbackStop();

        public virtual void OnDestroy() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

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

        // protected T InstantiateTemp<T>(T original, Transform parent) where T : Component
        // {
        //     var target = GameObject.Instantiate(original, parent);
        //     target.transform.SetPositionAndRotation(
        //         Owner.transform.position + original.transform.localPosition,
        //         Owner.transform.rotation * original.transform.rotation);
        //     var lifeTime = target.gameObject.AddComponent<LifeTime>();
        //     lifeTime.EnableLifeTime = StopAfterDuration;
        //     lifeTime.DestroyLifeTime = GetDuration();
        //     return target;
        // }

#if UNITY_EDITOR
        public virtual void OnValidate() {}
        public virtual void OnInspectorInit() {}
        public virtual void OnInspectorGUI() {}
        public virtual void OnSceneGUI() {}
#endif
    }
}
