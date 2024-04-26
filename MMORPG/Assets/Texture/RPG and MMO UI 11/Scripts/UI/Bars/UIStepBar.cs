using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Bars/Step Bar")]
	public class UIStepBar : MonoBehaviour {
		
		[System.Serializable]
		public struct StepFillInfo
		{
			public int index;
			public float amount;
		}
		
        #pragma warning disable 0649
		[SerializeField] private List<GameObject> m_StepsGameObjects = new List<GameObject>();
		[SerializeField] private List<StepFillInfo> m_OverrideFillList = new List<StepFillInfo>();
		
		[SerializeField] private GameObject m_StepsGridGameObject;
		[SerializeField] private RectTransform m_StepsGridRect;
		[SerializeField] private GridLayoutGroup m_StepsGrid;
		
		[SerializeField] private int m_CurrentStep = 0;
		[SerializeField] private int m_StepsCount = 1;
		[SerializeField] private RectOffset m_StepsGridPadding = new RectOffset();
		[SerializeField] private Sprite m_SeparatorSprite;
		[SerializeField] private Sprite m_SeparatorSpriteActive;
		[SerializeField] private Color m_SeparatorSpriteColor = Color.white;
		[SerializeField] private bool m_SeparatorAutoSize = true;
		[SerializeField] private Vector2 m_SeparatorSize = Vector2.zero;
		[SerializeField] private Image m_FillImage;
		[SerializeField] private RectTransform m_BubbleRect;
		[SerializeField] private Vector2 m_BubbleOffset = Vector2.zero;
		[SerializeField] private Text m_BubbleText;
		#pragma warning restore 0649

		/// <summary>
		/// Gets or sets the current step.
		/// </summary>
		/// <value>The value.</value>
		public int step
		{
			get { return this.m_CurrentStep; }
			set { this.GoToStep(value); }
		}
		
		/// <summary>
		/// Determines whether this instance is active.
		/// </summary>
		/// <returns><c>true</c> if this instance is active; otherwise, <c>false</c>.</returns>
		public virtual bool IsActive()
		{
			return this.enabled && this.gameObject.activeInHierarchy;
		}
		
		protected virtual void Start()
		{
			this.UpdateBubble();
		}

#if UNITY_EDITOR
        /// <summary>
        /// Editor only!
        /// </summary>
        public void RebuildSteps_Editor()
        {
            // Create the steps grid if required
            this.CreateStepsGrid();

            // Update the grid properties
            this.UpdateGridProperties();

            // Rebuild the steps if required
            this.RebuildSteps();

            // Update the steps properties
            this.UpdateStepsProperties();

            // Update the bubble
            this.UpdateBubble();
        }

		/// <summary>
		/// Raises the validate event.
		/// </summary>
		protected virtual void OnValidate()
		{
			// Validate the step integers
			if (this.m_CurrentStep < 0) this.m_CurrentStep = 0;
			if (this.m_StepsCount < 1) this.m_StepsCount = 1;
			if (this.m_CurrentStep > this.m_StepsCount) this.m_CurrentStep = this.m_StepsCount + 1;
			
			// Create the steps grid if required
			//this.CreateStepsGrid();
			
			// Update the grid properties
			this.UpdateGridProperties();
			
			// Rebuild the steps if required
			//this.RebuildSteps();
			
			// Update the steps properties
			this.UpdateStepsProperties();
			
			// Validate the fill image
			if (this.m_FillImage != null)
			{
				// Validate the image type
				if (this.m_FillImage.type != Image.Type.Filled)
					this.m_FillImage.type = Image.Type.Filled;
				
				// Validate the fill method
				if (this.m_FillImage.fillMethod != Image.FillMethod.Horizontal)
					this.m_FillImage.fillMethod = Image.FillMethod.Horizontal;
					
				// Update the fill amount
				this.UpdateFillImage();
			}
			
			// Update the bubble
			this.UpdateBubble();
		}
#endif

		/// <summary>
		/// Gets the fill override values list.
		/// </summary>
		/// <returns>The override fill list.</returns>
		public List<StepFillInfo> GetOverrideFillList()
		{
			return this.m_OverrideFillList;
		}
		
		/// <summary>
		/// Sets the fill override values list.
		/// </summary>
		/// <param name="list">List.</param>
		public void SetOverrideFillList(List<StepFillInfo> list)
		{
			this.m_OverrideFillList = list;
		}
		
		/// <summary>
		/// Validates the fill override values list.
		/// </summary>
		public void ValidateOverrideFillList()
		{
			// Create a temporary list
			List<StepFillInfo> list = new List<StepFillInfo>();
			
			// Copy the current list to array
			StepFillInfo[] tempArr = this.m_OverrideFillList.ToArray();
			
			// Loop
			foreach (StepFillInfo info in tempArr)
			{
				// Add all the valid step infos to the temporary list
				if (info.index > 1 && info.index <= this.m_StepsCount && info.amount > 0f)
					list.Add(info);
			}
			
			// Set the temporary list as the override list
			this.m_OverrideFillList = list;
		}
		
		/// <summary>
		/// Raises the rect transform dimensions change event.
		/// </summary>
		protected virtual void OnRectTransformDimensionsChange()
		{
			if (!this.IsActive())
				return;
			
			this.UpdateGridProperties();
		}
		
		/// <summary>
		/// Goes to the specified step.
		/// </summary>
		/// <param name="step">Step.</param>
		public void GoToStep(int step)
		{
			// Validate the step
			if (step < 0) step = 0;
			if (step > this.m_StepsCount) step = this.m_StepsCount + 1;
			
			// Set the step
			this.m_CurrentStep = step;
			
			// Update the steps properties
			this.UpdateStepsProperties();
			
			// Update the fill amount
			this.UpdateFillImage();
			
			// Update the bubble
			this.UpdateBubble();
		}
		
		/// <summary>
		/// Updates the fill image.
		/// </summary>
		public void UpdateFillImage()
		{
			if (this.m_FillImage == null)
				return;
			
			int overrideIndex = this.m_OverrideFillList.FindIndex(x => x.index == this.m_CurrentStep);
			
			// Apply fill amount
			this.m_FillImage.fillAmount = (overrideIndex >= 0) ? this.m_OverrideFillList[overrideIndex].amount : this.GetStepFillAmount(this.m_CurrentStep);
		}
		
		/// <summary>
		/// Updates the bubble.
		/// </summary>
		public void UpdateBubble()
		{
			if (this.m_BubbleRect == null)
				return;
			
			// Determine if the bubble should be visible
			if (this.m_CurrentStep > 0 && this.m_CurrentStep <= this.m_StepsCount)
			{
				// Activate if required
				if (!this.m_BubbleRect.gameObject.activeSelf)
					this.m_BubbleRect.gameObject.SetActive(true);
				
				// Get the step game object
				GameObject stepObject = this.m_StepsGameObjects[this.m_CurrentStep];
				
				if (stepObject != null)
				{
					RectTransform stepRect = stepObject.transform as RectTransform;
					
					if (stepRect.anchoredPosition.x != 0f)
					{
						// Update the bubble position based on the location of the step
						this.m_BubbleRect.anchoredPosition = new Vector2(this.m_BubbleOffset.x + (stepRect.anchoredPosition.x + (stepRect.rect.width / 2f)), this.m_BubbleOffset.y);
					}
				}

                // Update the bubble text
                if (this.m_BubbleText != null)
                    this.m_BubbleText.text = this.m_CurrentStep.ToString();
			}
			else
			{
				// Deactivate if required
				if (this.m_BubbleRect.gameObject.activeSelf)
					this.m_BubbleRect.gameObject.SetActive(false);
			}
		}
		
		/// <summary>
		/// Gets the step fill amount.
		/// </summary>
		/// <returns>The step fill amount.</returns>
		/// <param name="step">Step.</param>
		public float GetStepFillAmount(int step)
		{
			return ((1f / (float)(this.m_StepsCount + 1)) * (float)step);
		}
		
		/// <summary>
		/// Creates the steps grid.
		/// </summary>
		protected void CreateStepsGrid()
		{
			if (this.m_StepsGridGameObject != null)
				return;
			
			// Create new game object
			this.m_StepsGridGameObject = new GameObject("Steps Grid", typeof(RectTransform), typeof(GridLayoutGroup));
			this.m_StepsGridGameObject.layer = this.gameObject.layer;
			this.m_StepsGridGameObject.transform.SetParent(this.transform, false);
            this.m_StepsGridGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            this.m_StepsGridGameObject.transform.localPosition = Vector3.zero;
            this.m_StepsGridGameObject.transform.SetAsLastSibling();
			
			// Get the rect transform
			this.m_StepsGridRect = this.m_StepsGridGameObject.GetComponent<RectTransform>();
			this.m_StepsGridRect.sizeDelta = new Vector2(0f, 0f);
			this.m_StepsGridRect.anchorMin = new Vector2(0f, 0f);
			this.m_StepsGridRect.anchorMax = new Vector2(1f, 1f);
			this.m_StepsGridRect.pivot = new Vector2(0f, 1f);
			this.m_StepsGridRect.anchoredPosition = new Vector2(0f, 0f);
			
			// Get the grid layout group
			this.m_StepsGrid = this.m_StepsGridGameObject.GetComponent<GridLayoutGroup>();
			
			// Set the bubble as last sibling
			if (this.m_BubbleRect != null)
				this.m_BubbleRect.SetAsLastSibling();

            // Clear the steps game objects list
            this.m_StepsGameObjects.Clear();
        }
		
		/// <summary>
		/// Updates the grid properties.
		/// </summary>
		public void UpdateGridProperties()
		{
			if (this.m_StepsGrid == null)
				return;

            int seps = this.m_StepsCount + 2;

            // Grid Padding
            if (!this.m_StepsGrid.padding.Equals(this.m_StepsGridPadding))
				this.m_StepsGrid.padding = this.m_StepsGridPadding;
			
			// Auto sizing
			if (this.m_SeparatorAutoSize && this.m_SeparatorSprite != null)
			{
				this.m_SeparatorSize = new Vector2(this.m_SeparatorSprite.rect.width, this.m_SeparatorSprite.rect.height);
			}
			
			if (!this.m_StepsGrid.cellSize.Equals(this.m_SeparatorSize))
				this.m_StepsGrid.cellSize = this.m_SeparatorSize;
			
			// Grid spacing
			float spacingX = Mathf.Floor((this.m_StepsGridRect.rect.width - (float)this.m_StepsGridPadding.horizontal - ((float)seps * this.m_SeparatorSize.x)) / (float)(seps - 1) / 2f) * 2f;
			
			if (this.m_StepsGrid.spacing.x != spacingX)
				this.m_StepsGrid.spacing = new Vector2(spacingX, 0f);
		}
		
		/// <summary>
		/// Rebuilds the steps.
		/// </summary>
		public void RebuildSteps()
		{
			if (this.m_StepsGridGameObject == null)
				return;
            
			// Check if we already have the steps
			if (this.m_StepsGameObjects.Count == (this.m_StepsCount + 2))
				return;
			
			// Destroy the steps
			this.DestroySteps();

            int seps = this.m_StepsCount + 2;

            // Create the steps
            for (int i = 0; i < seps; i++)
			{
				GameObject step = new GameObject("Step " + i.ToString(), typeof(RectTransform));
				step.layer = this.gameObject.layer;
                step.transform.localScale = new Vector3(1f, 1f, 1f);
                step.transform.localPosition = Vector3.zero;
                step.transform.SetParent(this.m_StepsGridGameObject.transform, false);

                if (i > 0 && i < (seps - 1))
                    step.AddComponent<Image>();

				// Add to the list
				this.m_StepsGameObjects.Add(step);
			}
		}
		
		/// <summary>
		/// Updates the steps properties.
		/// </summary>
		protected void UpdateStepsProperties()
		{
			// Loop through the options
			foreach (GameObject stepObject in this.m_StepsGameObjects)
			{
				int index = this.m_StepsGameObjects.IndexOf(stepObject) + 1;
				bool active = index <= this.m_CurrentStep;

				// Image
				Image image = stepObject.GetComponent<Image>();
				
				if (image != null)
				{
					image.sprite = this.m_SeparatorSprite;
					image.overrideSprite = (active) ? this.m_SeparatorSpriteActive : null;
					image.color = this.m_SeparatorSpriteColor;
					image.rectTransform.pivot = new Vector2(0f, 1f);
				}
			}
		}
		
		/// <summary>
		/// Destroies the steps.
		/// </summary>
		protected void DestroySteps()
		{
			if (Application.isPlaying)
			{
				foreach (GameObject g in this.m_StepsGameObjects)
					Destroy(g);
			}
			else
			{
#if UNITY_EDITOR
				GameObject[] objects = this.m_StepsGameObjects.ToArray();
				
				UnityEditor.EditorApplication.delayCall += ()=>
				{
					foreach (GameObject g in objects)
						DestroyImmediate(g);
				};
#endif
			}
			
			// Clear the list
			this.m_StepsGameObjects.Clear();
		}
	}
}
