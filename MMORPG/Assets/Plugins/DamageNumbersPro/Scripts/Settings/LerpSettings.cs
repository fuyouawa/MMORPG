using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct LerpSettings
    {
        public LerpSettings (int customDefault)
        {
            minX = -0.4f;
            maxX = 0.4f;
            minY = 0.5f;
            maxY = 1f;

            speed = 5f;

            randomFlip = false;
        }

        [Header("Speed:")]
        [Tooltip("Speed at which it moves to the offset position.")]
        public float speed;

        [Header("Offset:")]
        [Tooltip("Minimum of horizontal offset.")]
        public float minX;
        [Tooltip("Maximum of horizontal offset.")]
        public float maxX;
        [Tooltip("Minimum of vertical offset.")]
        public float minY;
        [Tooltip("Maximum of vertical offset.")]
        public float maxY;

        [Header("Horizontal Flip:")]
        [Tooltip("Randomly flips the X Offset.\nUseful for avoiding small movements.\nSet Min X and Max X to a positive value.")]
        public bool randomFlip;
    }
}