using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro {
    [System.Serializable]
    public struct TextSettings
    {
        public TextSettings(float customDefault)
        {
            horizontal = customDefault;

            customColor = false;
            color = new Color(1, 1, 0f, 1);

            size = 0;
            vertical = 0;
            characterSpacing = 0f;

            alpha = 1;

            mark = false;
            markColor = new Color(0, 0, 0, 0.5f);

            bold = false;
            italic = false;
            underline = false;
            strike = false;
        }

        [Header("Basics:")]
        [Tooltip("Makes the text bold.")]
        public bool bold;
        [Tooltip("Makes the text italic.")]
        public bool italic;
        [Tooltip("Adds an underline to the text.")]
        public bool underline;
        [Tooltip("Strikes through the text with a line.")]
        public bool strike;

        [Header("Alpha:")]
        [Range(0, 1)]
        [Tooltip("Changes the alpha of the text.\nWon't work if Custom Color is used.")]
        public float alpha;

        [Header("Color:")]
        [Tooltip("Changes the color of the text.\nOverrides the alpha option above.")]
        public bool customColor;
        public Color color;

        [Header("Mark:")]
        [Tooltip("Highlights the text with a custom color.")]
        public bool mark;
        public Color markColor;

        [Header("Offset:")]
        [Tooltip("Horizontally moves the text.\nCan be used to offset the prefix or suffix.")]
        public float horizontal;
        [Tooltip("Vertically moves the text.\nCan be used to offset the prefix or suffix.")]
        public float vertical;

        [Header("Extra:")]
        [Tooltip("Changes the character spacing.")]
        public float characterSpacing;
        [Tooltip("Changes the font size.")]
        public float size;
    }
}
