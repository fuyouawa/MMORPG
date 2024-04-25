using System;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{
    public class Test_Fill : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Image imageComponent;
        [SerializeField] private float Duration = 5f;
        [SerializeField] private TweenEasing Easing = TweenEasing.InOutQuint;
        #pragma warning restore 0649

        // Tween controls
        [NonSerialized]
        private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        // Called by Unity prior to deserialization, 
        // should not be called by users
        protected Test_Fill()
        {
            if (this.m_FloatTweenRunner == null)
                this.m_FloatTweenRunner = new TweenRunner<FloatTween>();

            this.m_FloatTweenRunner.Init(this);
        }

        protected void OnEnable()
        {
            if (this.imageComponent == null)
                return;

            this.StartTween(0f, (this.imageComponent.fillAmount * this.Duration));
        }

        protected void SetFillAmount(float amount)
        {
            if (this.imageComponent == null)
                return;

            this.imageComponent.fillAmount = amount;
        }

        protected void OnTweenFinished()
        {
            if (this.imageComponent == null)
                return;

            this.StartTween((this.imageComponent.fillAmount == 0f ? 1f : 0f), this.Duration);
        }

        protected void StartTween(float targetFloat, float duration)
        {
            if (this.imageComponent == null)
                return;

            var floatTween = new FloatTween { duration = duration, startFloat = this.imageComponent.fillAmount, targetFloat = targetFloat };
            floatTween.AddOnChangedCallback(SetFillAmount);
            floatTween.AddOnFinishCallback(OnTweenFinished);
            floatTween.ignoreTimeScale = true;
            floatTween.easing = this.Easing;
            this.m_FloatTweenRunner.StartTween(floatTween);
        }
    }
}
