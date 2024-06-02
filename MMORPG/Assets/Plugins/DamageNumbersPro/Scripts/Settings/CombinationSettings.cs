using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct CombinationSettings
    {
        public CombinationSettings(float customDefault)
        {
            method = CombinationMethod.ABSORB_NEW;
            maxDistance = 10f;

            bonusLifetime = 1f;
            spawnDelay = 0.2f;

            absorbDuration = 0.4f;
            scaleCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.7f, 1), new Keyframe(1f, 0f));
            alphaCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 1), new Keyframe(1f, 0));

            moveToAbsorber = true;
            teleportToAbsorber = false;
            instantGain = false;

            absorberScaleFactor = 1.5f;
            absorberScaleFade = 15;
        }

        [Header("Main:")]
        [Tooltip("ABSORB_NEW: Oldest damage number absorbs newer damage numbers.\n\nREPLACE_OLD: New damage numbers absorb all existing damage numbers.\n\nIS_ALWAYS_ABSORBER: Will absorb all IS_ALWAYS_VICTIM damage numbers.\n\nIS_ALWAYS_VICTIM: Will be absorbed by the closest IS_ALWAYS_ABSORBER damage number.")]
        public CombinationMethod method;
        [Tooltip("The maximum distance at which numbers will combine.")]
        public float maxDistance;
        [Tooltip("The absorbtion delay after spawning.")]
        public float spawnDelay;

        [Header("Animation:")]
        [Tooltip("The length of the absorb animation.")]
        public float absorbDuration;
        [Tooltip("The scale over the absorb duration.")]
        public AnimationCurve scaleCurve;
        [Tooltip("The alpha over the absorb duration.")]
        public AnimationCurve alphaCurve;
        [Tooltip("If enabled the damage number will move towards it's absorber.")]
        public bool moveToAbsorber;
        [Tooltip("If enabled the damage number will teleport (spawn) inside it's absorber.")]
        public bool teleportToAbsorber;
        [Tooltip("How much the absorber is scaled up when it absorbs a damage number.")]
        public float absorberScaleFactor;
        [Tooltip("How quickly the absorber scales back to it's original size after being scaled up.")]
        public float absorberScaleFade;

        [Header("Other:")]
        [Tooltip("If true, the absorber will instantly gain the numbers of the target.  Should be used when combination is very fast.")]
        public bool instantGain;
        [Tooltip("The lifetime of the absorber is reset but also increased by this bonus lifetime.")]
        public float bonusLifetime;
    }

    [System.Serializable]
    public enum CombinationMethod
    {
        ABSORB_NEW
        ,
        REPLACE_OLD
        ,
        IS_ALWAYS_ABSORBER
        ,
        IS_ALWAYS_VICTIM
    }
}