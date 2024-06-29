using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MMORPG.Tool
{
    [AddFeedbackMenu("Volume/Set Vignette")]

    public class FeedbackVolumeSetVignette : AbstractFeedback
    {
        public enum Modes { OverTime, Instant }

        [FoldoutGroup("Vignette")]
        [Required]
        public VolumeProfile BoundVolumeProfile;
        [FoldoutGroup("Vignette")]
        public Modes Mode = Modes.OverTime;
        [FoldoutGroup("Vignette")]
        public float Duration = 0.2f;
        [FoldoutGroup("Vignette")]
        public bool AllowAdditivePlays;
        [FoldoutGroup("Vignette")]
        public bool DisableOnStop;

        [FoldoutGroup("Color")]
        public bool ModifyColor = true;
        [FoldoutGroup("Color")]
        [ShowIf("Mode", Modes.OverTime)]
        public Gradient ColorOverTime = new();
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
        public float RemapIntensityOne = 0.6f;
        [FoldoutGroup("Intensity")]
        [ShowIf("Mode", Modes.Instant)]
        public float InstantIntensity = 0.47f;

        [FoldoutGroup("Smooth")]
        public bool ModifySmooth = false;
        [FoldoutGroup("Smooth")]
        [ShowIf("Mode", Modes.OverTime)]
        public AnimationCurve SmoothCurve = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        [FoldoutGroup("Smooth")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapSmoothZero = 0f;
        [FoldoutGroup("Smooth")]
        [ShowIf("Mode", Modes.OverTime)]
        public float RemapSmoothOne = 0.3f;
        [FoldoutGroup("Smooth")]
        [ShowIf("Mode", Modes.Instant)]
        public float InstantSmooth = 0.3f;

        private Vignette _vignette;
        private Coroutine _coroutine;

        public override float GetDuration()
        {
            return Duration;
        }

        protected override void OnFeedbackInit()
        {
            if (!BoundVolumeProfile.TryGet(out _vignette))
            {
                _vignette = BoundVolumeProfile.Add<Vignette>();
            }

        }

        protected override void OnFeedbackPlay()
        {
            if (_coroutine != null && !AllowAdditivePlays)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _vignette.active = true;

            switch (Mode)
            {
                case Modes.OverTime:
                    _coroutine = StartCoroutine(VignetteSequence());
                    break;
                case Modes.Instant:
                    _vignette.intensity.max = Mathf.Max(_vignette.intensity.max, InstantIntensity);
                    _vignette.intensity.value = InstantIntensity;
                    _vignette.smoothness.max = Mathf.Max(_vignette.smoothness.max, InstantSmooth);
                    _vignette.smoothness.value = InstantSmooth;
                    if (ModifyColor)
                        _vignette.color.value = InstantColor;
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

            if (DisableOnStop)
            {
                _vignette.active = false;
            }
        }


        protected virtual IEnumerator VignetteSequence()
        {
            var totalTime = 0f;
            while (totalTime <= Duration)
            {
                SetVignetteValues(totalTime);
                totalTime += Time.deltaTime;
                yield return null;
            }
        }

        protected virtual void SetVignetteValues(float time)
        {
            float normalizedTime = MathHelper.Remap(time, 0, Duration, 0, 1);

            var intensity = MathHelper.Remap(
                IntensityCurve.Evaluate(normalizedTime),
                0f, 1f,
                RemapIntensityZero,
                RemapIntensityOne);
            var smooth = MathHelper.Remap(
                SmoothCurve.Evaluate(normalizedTime),
                0f, 1f,
                RemapSmoothZero,
                RemapSmoothOne);

            var color = ColorOverTime.Evaluate(normalizedTime);

            if (ModifyIntensity)
            {
                _vignette.intensity.max = Mathf.Max(_vignette.intensity.max, intensity);
                _vignette.intensity.value = intensity;
            }
            if (ModifySmooth)
            {
                _vignette.smoothness.max = Mathf.Max(_vignette.smoothness.max, smooth);
                _vignette.smoothness.value = smooth;
            }
            if (ModifyColor)
            {
                _vignette.color.value = color;
            }
        }
    }
}
