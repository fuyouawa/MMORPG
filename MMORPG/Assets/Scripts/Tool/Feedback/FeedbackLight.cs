
 using System;
 using System.Collections;
 using System.Collections.Generic;
 using Sirenix.OdinInspector;
 using UnityEngine;

 namespace MMORPG.Tool
{
    [AddFeedbackMenu("Light")]
    public class FeedbackLight : AbstractFeedback
    {
        public enum Modes { OverTime, Instant }

        [FoldoutGroup("Light")]
        public Light BoundLight;
        [FoldoutGroup("Light")]
        public Modes Mode = Modes.OverTime;
        [FoldoutGroup("Light")]
        [HideIf("Mode", Modes.Instant)]
        public float Duration = 0.2f;
        [FoldoutGroup("Light")]
        public bool DisableOnStop = false;
        [FoldoutGroup("Light")]
        public bool RelativeValues = true;
        [FoldoutGroup("Light")]
        public bool AllowAdditivePlays = false;

        [FoldoutGroup("Transform")]
        [Tooltip("在Play时改变Parent, 在Stop时还原")]
        public bool ChangeParentOnPlay = false;
        [FoldoutGroup("Transform")]
        [ShowIf("ChangeParentOnPlay")]
        [Tooltip("在Play时要改变到的Parent")]
        public Transform ParentOnPlay;
        [FoldoutGroup("Transform")]
        [ShowIf("ChangeParentOnPlay")]
        [Tooltip("在改变Parent时是否保持世界坐标")]
        public bool WorldPositionStays = true;


        [FoldoutGroup("Color")]
        public bool ModifyColor = true;
        [FoldoutGroup("Color")]
        [ShowIf("Mode", Modes.OverTime)]
        public Gradient ColorOverTime;
        [FoldoutGroup("Color")]
        [ShowIf("Mode", Modes.Instant)]
        public Color InstantColor = Color.red;

        [FoldoutGroup("Intensity")]
        public bool ModifyIntensity = true;
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.OverTime)]
        public AnimationCurve IntensityCurve = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapIntensityZero = 0f;
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapIntensityOne = 1f;
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.Instant)]
        public float InstantIntensity = 1f;

        [FoldoutGroup("Range")]
        public bool ModifyRange = true;
        [FoldoutGroup("Range")]
        [ShowIf("Mode", Modes.OverTime)]
        public AnimationCurve RangeCurve = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        [FoldoutGroup("Range")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapRangeZero = 0f;
        [FoldoutGroup("Range")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapRangeOne = 1f;
        [FoldoutGroup("Range")]
        [ShowIf("Mode", Modes.Instant)]
        public float InstantRange = 10f;

        protected float _initialRange;
        protected float _initialShadowStrength;
        protected float _initialIntensity;
        protected Coroutine _coroutine;
        protected Color _initialColor;

        protected TransformLocalStore _transformLocalStore;

        public override float GetDuration()
        {
            return Duration;
        }

        protected override void OnFeedbackInit()
        {
            if (BoundLight == null)
                return;

            _initialRange = BoundLight.range;
            _initialShadowStrength = BoundLight.shadowStrength;
            _initialIntensity = BoundLight.intensity;
            _initialColor = BoundLight.color;
            _transformLocalStore = BoundLight.transform.GetLocalStore();
        }

        protected override void OnFeedbackPlay()
        {
            if (_coroutine != null && !AllowAdditivePlays)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (ChangeParentOnPlay)
            {
                BoundLight.transform.SetParent(ParentOnPlay, WorldPositionStays);
            }

            BoundLight.enabled = true;
            switch (Mode)
            {
                case Modes.OverTime:
                    _coroutine = StartCoroutine(LightSequence());
                    break;
                case Modes.Instant:
                    BoundLight.intensity = InstantIntensity;
                    BoundLight.range = InstantRange;
                    if (ModifyColor)
                        BoundLight.color = InstantColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnFeedbackStop()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (ChangeParentOnPlay)
            {
                BoundLight.transform.FromLocalStore(_transformLocalStore);
            }

            if (DisableOnStop)
            {
                BoundLight.enabled = false;
            }
        }

        protected virtual IEnumerator LightSequence()
        {
            var totalTime = 0f;
            while (totalTime <= Duration)
            {
                SetLightValues(totalTime);
                totalTime += Time.deltaTime;
                yield return null;
            }
        }

        protected virtual void SetLightValues(float time)
        {
            float normalizedTime = MathHelper.Remap(time, 0, Duration, 0, 1);


            var intensity = MathHelper.Remap(
                IntensityCurve.Evaluate(normalizedTime),
                0f, 1f,
                RemapIntensityZero,
                RemapIntensityOne);
            var range = MathHelper.Remap(
                RangeCurve.Evaluate(normalizedTime),
                0f, 1f,
                RemapRangeZero,
                RemapRangeOne);

            var color = ColorOverTime.Evaluate(normalizedTime);

            if (RelativeValues)
            {
                intensity += _initialIntensity;
                range += _initialRange;
            }
            if (ModifyIntensity)
            {
                BoundLight.intensity = intensity;
            }
            if (ModifyRange)
            {
                BoundLight.range = range;
            }
            if (ModifyColor)
            {
                BoundLight.color = color;
            }
        }
    }
}
