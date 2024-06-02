using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageNumbersPro
{
    [System.Serializable]
    public struct DigitSettings
    {
        public DigitSettings(float customDefault)
        {
            decimals = 0;
            decimalChar = ".";
            hideZeros = false;

            dotSeparation = false;
            dotDistance = 3;
            dotChar = ".";

            suffixShorten = false;
            suffixes = new List<string>() { "K", "M", "B" };
            suffixDigits = new List<int>() { 3, 3, 3 };
            maxDigits = 4;
        }

        [Header("Decimals:")]
        [Range(0,3)]
        [Tooltip("Amount of digits visible after the dot.")]
        public int decimals;
        [Tooltip("The character used for the dot.")]
        public string decimalChar;
        [Tooltip("If true zeros at the end of the number will be hidden.")]
        public bool hideZeros;

        [Header("Dots:")]
        [Tooltip("Separates the number with dots.")]
        public bool dotSeparation;
        [Tooltip("Amount of digits between each dot.")]
        public int dotDistance;
        [Tooltip("The character used for the dot.")]
        public string dotChar;

        [Header("Suffix Shorten:")]
        [Tooltip("Shortens a number like 10000 to 10K.")]
        public bool suffixShorten;
        [Tooltip("List of suffixes.")]
        public List<string> suffixes;
        [Tooltip("Corresponding list of how many digits a suffix shortens.  Keep both lists at the same size.")]
        public List<int> suffixDigits;
        [Tooltip("Maximum of visible digits.  If number has more digits than this it will be shortened.")]
        public int maxDigits;
    }
}