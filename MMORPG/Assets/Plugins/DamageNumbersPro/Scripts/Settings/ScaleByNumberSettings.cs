using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct ScaleByNumberSettings
    {
        public ScaleByNumberSettings(float customDefault)
        {
            fromNumber = 0f;
            fromScale = 1f;

            toNumber = 1000f;
            toScale = 2f;
        }

        [Header("Number Range:")]
        [Tooltip("The number at which scaling starts.")]
        public float fromNumber;
        [Tooltip("The number at which scaling caps.")]
        public float toNumber;

        [Header("Scale Range:")]
        [Tooltip("The scale when the number is smaller of equal 'From Number'.")]
        public float fromScale;
        [Tooltip("The scale when the number is bigger of equal 'To Number'.")]
        public float toScale;
    }
}