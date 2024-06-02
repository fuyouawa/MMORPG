using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct CollisionSettings
    {
        public CollisionSettings(float customDefault)
        {
            radius = 0.5f;
            pushFactor = 1f;

            desiredDirection = new Vector3(0, 0);
        }

        [Header("Main:")]
        public float radius;
        public float pushFactor;

        [Header("Direction:")]
        public Vector3 desiredDirection;
    }
}
