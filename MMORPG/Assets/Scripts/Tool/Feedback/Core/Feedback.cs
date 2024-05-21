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

    public abstract class Feedback
    {
        public enum InitializeModes
        {
            Awake,
            Start,
            FirstPlay
        }

        [FoldoutGroup("Feedback Settings")]
        public bool Enable = true;
        [FoldoutGroup("Feedback Settings")]
        public string Label = "Feedback";

        [FoldoutGroup("Feedback Settings")]
        [Title("Time")]
        public float DelayBeforePlay;

        [FoldoutGroup("Feedback Settings")]
        [Title("Loop")]
        public bool LoopPlay = false;

        [FoldoutGroup("Feedback Settings")]
        [ShowIf("LoopPlay")]
        public bool LimitLoopAmount = true;

        [FoldoutGroup("Feedback Settings")]
        [ShowIf("@LoopPlay && LimitLoopAmount")]
        public int AmountOfLoop = 1;

        [FoldoutGroup("Feedback Settings")]
        [ShowIf("LoopPlay")]
        public float DelayBetweenLoop = 0f;

        public FeedbackManager Owner { get; private set; }
        public bool IsPlaying { get; private set; }

        public Transform Transform => Owner.transform;

        protected bool IsInitialized = false;
        protected int CurrentLoopCount;

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
            if (!Enable) return;
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

        public virtual void Setup(FeedbackManager owner)
        {
            Owner = owner;
        }

        public virtual void Initialize()
        {
            if (!Enable) return;

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
            if (!Enable)
            {
                Stop();
                yield break;
            }
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

        protected virtual void OnFeedbackPlay() { }

        protected virtual void OnFeedbackStop() { }

        public virtual void OnDestroy() {}

        public virtual float GetDuration()
        {
            return 0;
        }

        protected void StartCoroutine(IEnumerator routine)
        {
            Owner.CoroutineHelper.StartCoroutine(routine);
        }

        protected void StopCoroutine(IEnumerator routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        protected void StopAllCoroutine()
        {
            Owner.CoroutineHelper.StopAllCoroutines();
        }

#if UNITY_EDITOR
        private string GetLabel()
        {
            var duration = GetDuration();

            var timeDisplay = $"{DelayBeforePlay:0.00}s + {GetDuration():0.00}s";
            var enableDisplay = Enable ? "" : " [Disable]";
            if (LoopPlay)
            {
                var loopCountDisplay = LimitLoopAmount ? AmountOfLoop.ToString() : "\u221e";
                if (DelayBetweenLoop > float.Epsilon)
                {
                    timeDisplay += $" + {DelayBetweenLoop:0.00}s";
                }
                return $"{Label} ({timeDisplay}) x {loopCountDisplay}{enableDisplay}";
            }
            else
            {
                return $"{Label} ({timeDisplay}){enableDisplay}";
            }
        }
#endif
    }
}
