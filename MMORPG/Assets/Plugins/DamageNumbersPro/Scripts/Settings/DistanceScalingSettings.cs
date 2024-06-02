using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct DistanceScalingSettings
    {
        public DistanceScalingSettings (float customDefault)
        {
            baseDistance = 15f;
            closeDistance = 5f;
            farDistance = 50f;

            closeScale = 2f;
            farScale = 0.5f;
        }

        [Header("Distances:")]
        [Tooltip("The consistent size of the number is based on this distance.")]
        public float baseDistance;
        [Tooltip("The closest distance the number will be scaling up to.")]
        public float closeDistance;
        [Tooltip("The farthest distance the number will be scaling down to.")]
        public float farDistance;

        [Header("Scales:")]
        [Tooltip("The max scale the number reaches at the closest distance.")]
        public float closeScale;
        [Tooltip("The min scale the number reaches at the farthest distance.")]
        public float farScale;
    }
}