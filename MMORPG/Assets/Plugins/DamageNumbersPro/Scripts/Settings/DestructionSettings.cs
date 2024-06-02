using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct DestructionSettings
    {
        public DestructionSettings(float customDefault)
        {
            maxDistance = 2f;
            spawnDelay = 0.2f;

            duration = 0.3f;
            scaleCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0.5f));
            alphaCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        }

        [Header("Main:")]
        [Tooltip("The maximum distance at which damage numbers will be destroyed.")]
        public float maxDistance;
        [Tooltip("The delay after spawning that numbers will be destroyed.")]
        public float spawnDelay;

        [Header("Animation:")]
        public float duration;
        [Tooltip("The scale over the destruction duration.")]
        public AnimationCurve scaleCurve;
        [Tooltip("The alpha over the destruction duration.")]
        public AnimationCurve alphaCurve;
    }
}

