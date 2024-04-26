using System;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{
	public class Test_UIProgressBar : MonoBehaviour {
		
		public enum TextVariant
		{
			Percent,
			Value,
			ValueMax
		}
		
		public UIProgressBar bar;
		public float Duration = 5f;
		public TweenEasing Easing = TweenEasing.InOutQuint;
		public Text m_Text;
		public TextVariant m_TextVariant = TextVariant.Percent;
		public int m_TextValue = 100;
        public string m_TextValueFormat = "0";

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		
		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected Test_UIProgressBar()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected void OnEnable()
		{
			if (this.bar == null)
				return;
			
			this.StartTween(0f, (this.bar.fillAmount * this.Duration));
		}
		
		protected void SetFillAmount(float amount)
		{
			if (this.bar == null)
				return;
			
			this.bar.fillAmount = amount;
			
			if (this.m_Text != null)
			{
				if (this.m_TextVariant == TextVariant.Percent)
				{
					this.m_Text.text = Mathf.RoundToInt(amount * 100f).ToString() + "%";
				}
				else if (this.m_TextVariant == TextVariant.Value)
				{
					this.m_Text.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat);
				}
				else if (this.m_TextVariant == TextVariant.ValueMax)
				{
					this.m_Text.text =  ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat) + "/" + this.m_TextValue;
				}
			}
		}
		
		protected void OnTweenFinished()
		{
			if (this.bar == null)
				return;
			
			this.StartTween((this.bar.fillAmount == 0f ? 1f : 0f), this.Duration);
		}
		
		protected void StartTween(float targetFloat, float duration)
		{
			if (this.bar == null)
				return;
			
			var floatTween = new FloatTween { duration = duration, startFloat = this.bar.fillAmount, targetFloat = targetFloat };
			floatTween.AddOnChangedCallback(SetFillAmount);
			floatTween.AddOnFinishCallback(OnTweenFinished);
			floatTween.ignoreTimeScale = true;
			floatTween.easing = this.Easing;
			this.m_FloatTweenRunner.StartTween(floatTween);
		}
	}
}
