using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct FollowSettings
    {
        public FollowSettings(float customDefault)
        {
            speed = 10;
            drag = 0f;
        }

        [Tooltip("Speed at which target is followed.")]
        public float speed;
        [Tooltip("Decreases follow speed over time.")]
        public float drag;
    }
}