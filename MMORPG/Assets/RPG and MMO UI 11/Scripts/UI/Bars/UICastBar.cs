using System;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;
using System.Collections;

namespace DuloGames.UI
{
	[ExecuteInEditMode, RequireComponent(typeof(CanvasGroup))]
	public class UICastBar : MonoBehaviour {
		
		[Serializable]
		public enum DisplayTime
		{
			Elapsed,
			Remaining
		}
		
		[Serializable]
		public enum Transition
		{
			Instant,
			Fade
		}
		
		[Serializable]
		public class ColorStage
		{
			public Color fillColor = Color.white;
			public Color titleColor = Color.white;
			public Color timeColor = Color.white;
		}
		
        #pragma warning disable 0649
		[SerializeField] private UIProgressBar m_ProgressBar;
		[SerializeField] private Text m_TitleLabel;
		[SerializeField] private Text m_TimeLabel;
		[SerializeField] private DisplayTime m_DisplayTime = DisplayTime.Remaining;
		[SerializeField] private string m_TimeFormat = "0.0 sec";
		[SerializeField] private Text m_FullTimeLabel;
        [SerializeField] private string m_FullTimeFormat = "0.0 sec";

		[SerializeField] private bool m_UseSpellIcon;
		[SerializeField] private GameObject m_IconFrame;
		[SerializeField] private Image m_IconImage;
		
        [SerializeField] private Image m_FillImage;
		[SerializeField] private ColorStage m_NormalColors = new ColorStage();
		[SerializeField] private ColorStage m_OnInterruptColors = new ColorStage();
		[SerializeField] private ColorStage m_OnFinishColors = new ColorStage();

		[SerializeField] private Transition m_InTransition = Transition.Instant;
		[SerializeField] private float m_InTransitionDuration = 0.1f;
		[SerializeField] private bool m_BrindToFront = true;

        [SerializeField] private Transition m_OutTransition = Transition.Fade;
		[SerializeField] private float m_OutTransitionDuration = 0.1f;
		[SerializeField] private float m_HideDelay = 0.3f;
		#pragma warning restore 0649

		private bool m_IsCasting = false;
		
		/// <summary>
		/// Gets a value indicating whether this cast bar is casting.
		/// </summary>
		/// <value><c>true</c> if this instance is casting; otherwise, <c>false</c>.</value>
		public bool IsCasting { 
			get { return this.m_IsCasting; }
		}
		
		private float currentCastDuration = 0f;
		private float currentCastEndTime = 0f;
		
		private CanvasGroup m_CanvasGroup;
		
		/// <summary>
		/// Gets the canvas group.
		/// </summary>
		/// <value>The canvas group.</value>
		public CanvasGroup canvasGroup
		{
			get { return this.m_CanvasGroup; }
		}
		
		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		
		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UICastBar()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected virtual void Awake()
		{
			this.m_CanvasGroup = this.gameObject.GetComponent<CanvasGroup>();
		}
		
		protected virtual void Start()
		{
			if (this.isActiveAndEnabled)
			{
				// Apply the normal color stage
				this.ApplyColorStage(this.m_NormalColors);
				
				// Hide the bar only while the application is playing
				if (Application.isPlaying)
				{
					// Hide the icon frame
					if (this.m_IconFrame != null)
						this.m_IconFrame.SetActive(false);
					
					// Hide the cast bar
					this.Hide(true);
				}
			}
		}
		
#if UNITY_EDITOR
		protected void OnValidate()
		{
			// Apply the normal color stage
			if (this.isActiveAndEnabled)
				this.ApplyColorStage(this.m_NormalColors);
		}
#endif
		
		/// <summary>
		/// Applies the color stages.
		/// </summary>
		/// <param name="stage">Stage.</param>
		public virtual void ApplyColorStage(ColorStage stage)
		{
			if (this.m_FillImage != null)
				this.m_FillImage.canvasRenderer.SetColor(stage.fillColor);
			
			if (this.m_TitleLabel != null)
				this.m_TitleLabel.canvasRenderer.SetColor(stage.titleColor);
			
			if (this.m_TimeLabel != null)
				this.m_TimeLabel.canvasRenderer.SetColor(stage.timeColor);
		}
		
		/// <summary>
		/// Show this cast bar.
		/// </summary>
		public void Show()
		{
			// Call show with a transition
			this.Show(false);
		}
		
		/// <summary>
		/// Show this cast bar.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public virtual void Show(bool instant)
		{
            // Bring to the front
            if (this.m_BrindToFront)
                UIUtility.BringToFront(this.gameObject);

            // Do the transition
            if (instant || this.m_InTransition == Transition.Instant)
			{
				// Set the canvas group alpha
				this.m_CanvasGroup.alpha = 1f;
			}
			else
			{
				// Start a tween
				this.StartAlphaTween(1f, this.m_InTransitionDuration, true);
			}
		}
		
		/// <summary>
		/// Hide this cast bar.
		/// </summary>
		public void Hide()
		{
			// Call hide with a transition
			this.Hide(false);
		}
		
		/// <summary>
		/// Hide this cast bar.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public virtual void Hide(bool instant)
		{
			if (instant || this.m_OutTransition == Transition.Instant)
			{
				// Set the canvas group alpha
				this.m_CanvasGroup.alpha = 0f;
				
				// Raise the on hide tween finished event
				this.OnHideTweenFinished();
			}
			else
			{
				// Start a tween
				this.StartAlphaTween(0f, this.m_OutTransitionDuration, true);
			}
		}
		
		/// <summary>
		/// Starts alpha tween.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
		public void StartAlphaTween(float targetAlpha, float duration, bool ignoreTimeScale)
		{
			if (this.m_CanvasGroup == null)
				return;
			
			var floatTween = new FloatTween { duration = duration, startFloat = this.m_CanvasGroup.alpha, targetFloat = targetAlpha };
			floatTween.AddOnChangedCallback(SetCanvasAlpha);
			floatTween.AddOnFinishCallback(OnHideTweenFinished);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			this.m_FloatTweenRunner.StartTween(floatTween);
		}
		
		/// <summary>
		/// Sets the canvas alpha.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		protected void SetCanvasAlpha(float alpha)
		{
			if (this.m_CanvasGroup == null)
				return;
			
			// Set the alpha
			this.m_CanvasGroup.alpha = alpha;
		}
		
		/// <summary>
		/// Raises the hide tween finished event.
		/// </summary>
		protected virtual void OnHideTweenFinished()
		{
			// Hide the icon frame
			if (this.m_IconFrame != null)
				this.m_IconFrame.SetActive(false);
			
			// Unset the icon image sprite
			if (this.m_IconImage != null)
				this.m_IconImage.sprite = null;
		}
		
		/// <summary>
		/// Sets the fill amount.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void SetFillAmount(float amount)
		{
			if (this.m_ProgressBar != null)
				this.m_ProgressBar.fillAmount = amount;
		}
		
		IEnumerator AnimateCast()
		{
			// Update the text
			if (this.m_TimeLabel != null)
				this.m_TimeLabel.text = ((this.m_DisplayTime == DisplayTime.Elapsed) ? 0f.ToString(this.m_TimeFormat) : this.currentCastDuration.ToString(this.m_TimeFormat));
			
			// Get the timestamp
			float startTime = (this.currentCastEndTime > 0f) ? (this.currentCastEndTime - this.currentCastDuration) : Time.time;
			
			// Fade In the notification
			while (Time.time < (startTime + this.currentCastDuration))
			{
				float RemainingTime = (startTime + this.currentCastDuration) - Time.time;
				float ElapsedTime = this.currentCastDuration - RemainingTime;
				float ElapsedTimePct = ElapsedTime / this.currentCastDuration;
				
				// Update the elapsed cast time value
				if (this.m_TimeLabel != null)
					this.m_TimeLabel.text = ((this.m_DisplayTime == DisplayTime.Elapsed) ? ElapsedTime.ToString(this.m_TimeFormat) : RemainingTime.ToString(this.m_TimeFormat));
				
				// Update the fill sprite
				this.SetFillAmount(ElapsedTimePct);
				
				yield return 0;
			}

            // Update the fill sprite to full
            this.SetFillAmount(1f);

            // Make sure it's maxed
            if (this.m_TimeLabel != null)
				this.m_TimeLabel.text = ((this.m_DisplayTime == DisplayTime.Elapsed) ? this.currentCastDuration.ToString(this.m_TimeFormat) : 0f.ToString(this.m_TimeFormat));
			
			// Call that we finished
			this.OnFinishedCasting();
			
			// Hide with a delay
			this.StartCoroutine("DelayHide");
		}
		
		IEnumerator DelayHide()
		{
			// Wait for the hide delay
			yield return new WaitForSeconds(this.m_HideDelay);
			
			// Do not show the casting anymore
			this.Hide();
		}
		
		/// <summary>
		/// Starts the casting of the specified spell.
		/// </summary>
		/// <param name="spellInfo">Spell info.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="endTime">End time.</param>
		public virtual void StartCasting(UISpellInfo spellInfo, float duration, float endTime)
		{
			// Make sure we can start casting it
			if (this.m_IsCasting)
				return;
			
			// Stop the coroutine might be still running on the hide delay
			this.StopCoroutine("AnimateCast");
			this.StopCoroutine("DelayHide");
			
			// Apply the normal colors
			this.ApplyColorStage(this.m_NormalColors);
			
			// Change the fill pct
			this.SetFillAmount(0f);
			
			// Set the spell name
			if (this.m_TitleLabel != null)
				this.m_TitleLabel.text = spellInfo.Name;

            // Set the full time cast text
            if (this.m_FullTimeLabel != null)
                this.m_FullTimeLabel.text = spellInfo.CastTime.ToString(this.m_FullTimeFormat);

            // Set the icon if we have enabled icons
            if (this.m_UseSpellIcon)
			{
				// Check if we have a sprite
				if (spellInfo.Icon != null)
				{
					// Check if the icon image is set
					if (this.m_IconImage != null)
						this.m_IconImage.sprite = spellInfo.Icon;
					
					// Enable the frame
					if (this.m_IconFrame != null)
						this.m_IconFrame.SetActive(true);
				}
			}
			
			// Set some info about the cast
			this.currentCastDuration = duration;
			this.currentCastEndTime = endTime;
			
			// Define that we start casting animation
			this.m_IsCasting = true;
			
			// Show the cast bar
			this.Show();
			
			// Start the cast animation
			this.StartCoroutine("AnimateCast");
		}
		
		/// <summary>
		/// Interrupts the current cast if any.
		/// </summary>
		public virtual void Interrupt()
		{
			if (this.m_IsCasting)
			{
				// Stop the coroutine if it's assigned
				this.StopCoroutine("AnimateCast");
				
				// No longer casting
				this.m_IsCasting = false;
				
				// Apply the interrupt colors
				this.ApplyColorStage(this.m_OnInterruptColors);
				
				// Hide with a delay
				this.StartCoroutine("DelayHide");
			}
		}
		
		protected void OnFinishedCasting()
		{
			// Define that we are no longer casting
			this.m_IsCasting = false;

            // Apply the finish colors
            this.ApplyColorStage(this.m_OnFinishColors);
        }
	}
}
