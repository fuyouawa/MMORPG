using UnityEngine;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{
	public class Test_FadeInOut : MonoBehaviour {
		
		[SerializeField] private float m_Duration = 4f;
		[SerializeField] private TweenEasing m_Easing = TweenEasing.InOutQuint;
		private CanvasGroup m_Group;
		
		// Tween controls
		[System.NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		
		protected Test_FadeInOut()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected void OnEnable()
		{
			this.m_Group = this.GetComponent<CanvasGroup>();
			
			if (this.m_Group == null)
				this.m_Group = this.gameObject.AddComponent<CanvasGroup>();
			
			this.StartAlphaTween(0f, this.m_Duration, true);
		}
		
		/// <summary>
		/// Tweens the canvas group alpha.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
		private void StartAlphaTween(float targetAlpha, float duration, bool ignoreTimeScale)
		{
			if (this.m_Group == null)
				return;
			
			float currentAlpha = this.m_Group.alpha;
			
			if (currentAlpha.Equals(targetAlpha))
				return;
			
			var floatTween = new FloatTween { duration = duration, startFloat = currentAlpha, targetFloat = targetAlpha };
			floatTween.AddOnChangedCallback(SetAlpha);
			floatTween.AddOnFinishCallback(OnTweenFinished);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			floatTween.easing = this.m_Easing;
			this.m_FloatTweenRunner.StartTween(floatTween);
		}
		
		/// <summary>
		/// Sets the list alpha.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		private void SetAlpha(float alpha)
		{
			if (this.m_Group == null)
				return;
			
			// Set the alpha
			this.m_Group.alpha = alpha;
		}
		
		/// <summary>
		/// Raises the list tween finished event.
		/// </summary>
		protected virtual void OnTweenFinished()
		{
			if (this.m_Group == null)
				return;
				
			this.StartAlphaTween((this.m_Group.alpha == 1f ? 0f : 1f), this.m_Duration, true);
		}
	}
}
