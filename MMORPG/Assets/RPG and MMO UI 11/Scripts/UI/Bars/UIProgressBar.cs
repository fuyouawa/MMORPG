using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Bars/Progress Bar")]
	public class UIProgressBar : MonoBehaviour, IUIProgressBar
    {
		[Serializable] public class ChangeEvent : UnityEvent<float> { }
		
		public enum Type
		{
			Filled,
			Resize,
            Sprites
		}
		
		public enum FillSizing
		{
			Parent,
			Fixed
		}
		
		[SerializeField] private Type m_Type = Type.Filled;
		[SerializeField] private Image m_TargetImage;
        [SerializeField] private Sprite[] m_Sprites;
		[SerializeField] private RectTransform m_TargetTransform;
		[SerializeField] private FillSizing m_FillSizing = FillSizing.Parent;
		[SerializeField] private float m_MinWidth = 0f;
		[SerializeField] private float m_MaxWidth = 100f;
		[SerializeField][Range(0f, 1f)] private float m_FillAmount = 1f;
		[SerializeField] private int m_Steps = 0;
		public ChangeEvent onChange = new ChangeEvent();
		
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public Type type {
			get { return this.m_Type; }
			set { this.m_Type = value; }
		}
		
		/// <summary>
		/// Gets or sets the target image.
		/// </summary>
		/// <value>The target image.</value>
		public Image targetImage {
			get { return this.m_TargetImage; }
			set { this.m_TargetImage = value; }
		}
		
        /// <summary>
        /// Gets or sets array with the animation sprites.
        /// </summary>
        public Sprite[] sprites
        {
            get { return this.m_Sprites; }
            set { this.m_Sprites = value; }
        }

		/// <summary>
		/// Gets or sets the target transform.
		/// </summary>
		/// <value>The target transform.</value>
		public RectTransform targetTransform {
			get { return this.m_TargetTransform; }
			set { this.m_TargetTransform = value; }
		}
		
		/// <summary>
		/// Gets or sets the minimum width (Used for the resize type bar).
		/// </summary>
		/// <value>The minimum width.</value>
		public float minWidth {
			get { return this.m_MinWidth; }
			set {
				this.m_MinWidth = value;
				this.UpdateBarFill();
			}
		}
		
		/// <summary>
		/// Gets or sets the maximum width (Used for the resize type bar).
		/// </summary>
		/// <value>The maximum width.</value>
		public float maxWidth {
			get { return this.m_MaxWidth; }
			set {
				this.m_MaxWidth = value;
				this.UpdateBarFill();
			}
		}
		
		/// <summary>
		/// Gets or sets the fill amount.
		/// </summary>
		/// <value>The fill amount.</value>
		public float fillAmount {
			get {
				return this.m_FillAmount;
			}
			set {
				if (this.m_FillAmount != Mathf.Clamp01(value))
				{
					this.m_FillAmount = Mathf.Clamp01(value);
					this.UpdateBarFill();
					this.onChange.Invoke(this.m_FillAmount);
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the steps (Zero for no stepping).
		/// </summary>
		/// <value>The steps.</value>
		public int steps {
			get { return this.m_Steps; }
			set { this.m_Steps = value; }
		}
		
		/// <summary>
		/// Gets or sets the current step.
		/// </summary>
		/// <value>The current step.</value>
		public int currentStep {
			get {
				if (this.m_Steps == 0)
					return 0;
				
				float perStep = 1f / (this.m_Steps - 1);
				return Mathf.RoundToInt(this.fillAmount / perStep);
			}
			set {
				if (this.m_Steps > 0)
				{
					float perStep = 1f / (this.m_Steps - 1);
					this.fillAmount = Mathf.Clamp(value, 0, this.m_Steps) * perStep;
				}
			}
		}
		
        protected virtual void Start()
        {
            // Make sure the fill anchor reflects the pivot
            if (this.m_Type == Type.Resize && this.m_FillSizing == FillSizing.Parent && this.m_TargetTransform != null)
            {
                float height = this.m_TargetTransform.rect.height;
                this.m_TargetTransform.anchorMin = this.m_TargetTransform.pivot;
                this.m_TargetTransform.anchorMax = this.m_TargetTransform.pivot;
                this.m_TargetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            // Update the bar fill
            this.UpdateBarFill();
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            // Update the bar fill
            this.UpdateBarFill();
        }

#if UNITY_EDITOR
        protected void OnValidate()
		{
            // Make sure the fill anchor reflects the pivot
            if (this.m_Type == Type.Resize && this.m_FillSizing == FillSizing.Parent && this.m_TargetTransform != null)
            {
                float height = this.m_TargetTransform.rect.height;
                this.m_TargetTransform.anchorMin = this.m_TargetTransform.pivot;
                this.m_TargetTransform.anchorMax = this.m_TargetTransform.pivot;
                this.m_TargetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
		}
		
		protected void Reset()
		{
			this.onChange = new ChangeEvent();
		}
#endif

		/// <summary>
		/// Updates the bar fill.
		/// </summary>
		public void UpdateBarFill()
		{
			if (!this.isActiveAndEnabled)
				return;

            if (this.m_Type == Type.Filled && this.m_TargetImage == null)
                return;

            if (this.m_Type == Type.Resize && this.m_TargetTransform == null)
                return;

            if (this.m_Type == Type.Sprites && this.m_Sprites.Length == 0)
                return;

            // Get the fill amount
            float fill = this.m_FillAmount;
			
			// Check for steps
			if (this.m_Steps > 0)
				fill = Mathf.Round(this.m_FillAmount * (this.m_Steps - 1)) / (this.m_Steps - 1);
			
			if (this.m_Type == Type.Resize)
			{
				// Update the bar fill by changing it's width
				// we are doing it this way because we are using a mask on the bar and have it's fill inside with static width and position
				if (this.m_FillSizing == FillSizing.Fixed)
				{
					this.m_TargetTransform.SetSizeWithCurrentAnchors(
						RectTransform.Axis.Horizontal, 
						(this.m_MinWidth + ((this.m_MaxWidth - this.m_MinWidth) * fill))
					);
				}
				else
				{
                    this.m_TargetTransform.SetSizeWithCurrentAnchors(
						RectTransform.Axis.Horizontal, 
						((this.m_TargetTransform.parent as RectTransform).rect.width * fill)
					);
				}
			}
            else if (this.m_Type == Type.Sprites)
            {
                int spriteIndex = Mathf.RoundToInt(fill * (float)this.m_Sprites.Length) - 1;

                if (spriteIndex > -1)
                {
                    this.targetImage.overrideSprite = this.m_Sprites[spriteIndex];
                    this.targetImage.canvasRenderer.SetAlpha(1f);
                }
                else
                {
                    this.targetImage.overrideSprite = null;
                    this.targetImage.canvasRenderer.SetAlpha(0f);
                }
            }
			else
			{
				// Update the image fill amount
				this.m_TargetImage.fillAmount = fill;
			}
		}
		
		/// <summary>
		/// Adds to the fill (Used for buttons).
		/// </summary>
		public void AddFill()
		{
			if (this.m_Steps > 0)
			{
				this.currentStep = this.currentStep + 1;
			}
			else
			{
				this.fillAmount = this.fillAmount + 0.1f;
			}
		}
		
		/// <summary>
		/// Removes from the fill (Used for buttons).
		/// </summary>
		public void RemoveFill()
		{
			if (this.m_Steps > 0)
			{
				this.currentStep = this.currentStep - 1;
			}
			else
			{
				this.fillAmount = this.fillAmount - 0.1f;
			}
		}
	}
}
