using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct PushSettings
    {
        public PushSettings(float customDefault)
        {
            radius = 4f;
            pushOffset = 0.8f;
        }

        [Header("Main:")]
        public float radius;
        public float pushOffset;
    }
}
