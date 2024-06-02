using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct ColorByNumberSettings
    {
        public ColorByNumberSettings(float customValue)
        {
            colorGradient = new Gradient();
            colorGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.8f, 0.5f), 0f), new GradientColorKey(new Color(1f, 0f, 0f), 1f) }, new GradientAlphaKey[] { new GradientAlphaKey(1, 0) });

            fromNumber = 10;
            toNumber = 100;
        }

        public Gradient colorGradient;
        public float fromNumber;
        public float toNumber;
    }
}