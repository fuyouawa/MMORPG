using UnityEngine;
using TMPro;

namespace Michsky.MUIP
{
    public class PBFilled : MonoBehaviour
    {
        [Header("Resources")]
        public TextMeshProUGUI minLabel;
        public TextMeshProUGUI maxLabel;

        [Header("Settings")]
        [Range(0, 100)] public int transitionAfter = 50;
        public Color minColor = new Color(0, 0, 0, 255);
        public Color maxColor = new Color(255, 255, 255, 255);

        ProgressBar progressBar;
        Animator barAnimatior;

        void Start()
        {
            progressBar = gameObject.GetComponent<ProgressBar>();
            barAnimatior = gameObject.GetComponent<Animator>();

            minLabel.color = minColor;
            maxLabel.color = maxColor;
        }

        void Update()
        {
            if (progressBar.currentPercent >= transitionAfter)
                barAnimatior.Play("Radial PB Filled");

            if (progressBar.currentPercent <= transitionAfter)
                barAnimatior.Play("Radial PB Empty");

            maxLabel.text = minLabel.text;
        }
    }
}