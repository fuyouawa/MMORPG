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

        public AddFeedbackMenuAttribute(string path)
        {
            Path = path;
        }
    }

    public abstract class AbstractFeedback
    {
        public enum InitializeModes
        {
            Awake,
            Start,
            FirstPlay
        }

        [Serializable]
        public class Condition
        {
            public enum CheckModes
            {
                OnAwake,
                OnStart,
                OnUpdate
            }

            public CheckModes CheckMode = CheckModes.OnStart;
            public bool Negative;
            [HideLabel]
            public UnityValueGetter<bool> Getter = new();
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

        [FoldoutGroup("Feedback Settings")]
        [Title("Enable Condition")]
        [HideReferenceObjectPicker]
        public Condition DisableIf = new();

        public FeedbacksManager Owner { get; private set; }
        public bool IsPlaying { get; private set; }

        public Transform Transform => Owner.transform;

        protected bool IsInitialized = false;
        protected int CurrentLoopCount;

        private List<Coroutine> _feedbackPlayCoroutines;
        private List<Coroutine> _durationCoroutineList;

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
            _feedbackPlayCoroutines.Add(StartCoroutine(FeedbackPlayCo()));
        }

        public virtual void Stop()
        {
            if (_feedbackPlayCoroutines.IsNotNullAndEmpty())
            {
                _feedbackPlayCoroutines.ForEach(StopCoroutine);
                _feedbackPlayCoroutines.Clear();
            }
            if (_durationCoroutineList.IsNotNullAndEmpty())
            {
                _durationCoroutineList.ForEach(StopCoroutine);
                _durationCoroutineList.Clear();
            }
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
            if (!Enable) return;

            if (IsInitialized) return;
            IsInitialized = true;

            _durationCoroutineList ??= new();
            _feedbackPlayCoroutines ??= new();

            OnFeedbackInit();
        }


        protected virtual IEnumerator FeedbackPlayCo()
        {
            yield return new WaitForSeconds(DelayBeforePlay);
            _durationCoroutineList.Add(StartCoroutine(DurationCoroutine()));
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
                _feedbackPlayCoroutines.Add(StartCoroutine(FeedbackPlayCo()));
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
#endif
    }
}
