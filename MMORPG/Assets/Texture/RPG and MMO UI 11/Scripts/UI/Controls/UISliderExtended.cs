using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DuloGames.UI
{
    [AddComponentMenu("UI/Slider Extended", 58)]
    public class UISliderExtended : Slider
    {
        public enum TextEffectType
        {
            None,
            Shadow,
            Outline
        }

        public enum OptionTransition
        {
            None,
            ColorTint
        }

        public enum TransitionTarget
        {
            Image,
            Text
        }

        #pragma warning disable 0649
        [SerializeField] private List<string> m_Options = new List<string>();
        [SerializeField] private List<GameObject> m_OptionGameObjects = new List<GameObject>();
        [SerializeField] private GameObject m_OptionsContGameObject;
        [SerializeField] private RectTransform m_OptionsContRect;
        [SerializeField] private GridLayoutGroup m_OptionsContGrid;
        
        [SerializeField] private RectOffset m_OptionsPadding = new RectOffset();
        [SerializeField] private Sprite m_OptionSprite;
        [SerializeField] private Font m_OptionTextFont;
        [SerializeField] private FontStyle m_OptionTextStyle = FontData.defaultFontData.fontStyle;
        [SerializeField] private int m_OptionTextSize = FontData.defaultFontData.fontSize;
        [SerializeField] private Color m_OptionTextColor = Color.white;
        [SerializeField] private TextEffectType m_OptionTextEffect = TextEffectType.None;
        [SerializeField] private Color m_OptionTextEffectColor = new Color(0f, 0f, 0f, 128f);
        [SerializeField] private Vector2 m_OptionTextEffectDistance = new Vector2(1f, -1f);
        [SerializeField] private bool m_OptionTextEffectUseGraphicAlpha = true;
        [SerializeField] private Vector2 m_OptionTextOffset = Vector2.zero;
        [SerializeField] private OptionTransition m_OptionTransition = OptionTransition.None;
        [SerializeField] private TransitionTarget m_OptionTransitionTarget = TransitionTarget.Text;
        [SerializeField] private Color m_OptionTransitionColorNormal = ColorBlock.defaultColorBlock.normalColor;
        [SerializeField] private Color m_OptionTransitionColorActive = ColorBlock.defaultColorBlock.highlightedColor;
        [SerializeField, Range(1f, 6f)] private float m_OptionTransitionMultiplier = 1f;
        [SerializeField] private float m_OptionTransitionDuration = 0.1f;
        #pragma warning restore 0649

        private GameObject m_CurrentOptionGameObject;

        /// <summary>
        /// Gets or sets the options list (Rebuilds the options on set).
        /// </summary>
        /// <value>The options.</value>
        public List<string> options
        {
            get { return this.m_Options; }
            set
            {
                this.m_Options = value;
                this.RebuildOptions();
                this.ValidateOptions();
            }
        }

        /// <summary>
        /// Gets the selected option game object.
        /// </summary>
        /// <value>The selected option game object.</value>
        public GameObject selectedOptionGameObject
        {
            get { return this.m_CurrentOptionGameObject; }
        }

        /// <summary>
        /// Gets the index of the selected option.
        /// </summary>
        /// <value>The index of the selected option.</value>
        public int selectedOptionIndex
        {
            get
            {
                int optionIndex = Mathf.RoundToInt(this.value);

                // Validate the index
                if (optionIndex < 0 || optionIndex >= this.m_Options.Count)
                    return 0;

                return optionIndex;
            }
        }

        /// <summary>
        /// Gets or sets the options grid padding.
        /// </summary>
        /// <value>The options padding.</value>
        public RectOffset optionsPadding
        {
            get { return this.m_OptionsPadding; }
            set { this.m_OptionsPadding = value; }
        }

        /// <summary>
        /// Determines whether this slider has options.
        /// </summary>
        /// <returns><c>true</c> if this instance has options; otherwise, <c>false</c>.</returns>
        public bool HasOptions()
        {
            return (this.m_Options != null && this.m_Options.Count > 0);
        }

        /// <summary>
        /// Raises the enable event.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            this.ValidateOptions();

            // Add the listener for the value change
            this.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
		/// Raises the disable event.
		/// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // Remove the listener for the value change
            this.onValueChanged.RemoveListener(OnValueChanged);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Raises the validate event.
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            this.ValidateOptions();

            if (this.m_OptionTextFont == null)
                this.m_OptionTextFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        }
#endif

        /// <summary>
        /// Raises the rect transform dimensions change event.
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            if (!this.IsActive())
                return;

            this.UpdateGridProperties();
        }

        /// <summary>
        /// Raises the value changed event.
        /// </summary>
        /// <param name="value">Value.</param>
        public void OnValueChanged(float value)
        {
            if (!this.IsActive() || !this.HasOptions())
                return;

            // Transition
            if (this.m_OptionTransition == OptionTransition.ColorTint)
            {
                // Transition out the current selected option
                Graphic currentTarget =
                    (this.m_OptionTransitionTarget == TransitionTarget.Text) ?
                        (this.m_CurrentOptionGameObject.GetComponentInChildren<Text>() as Graphic) :
                        (this.m_CurrentOptionGameObject.GetComponent<Image>() as Graphic);

                // Transition the current target to normal state
                this.StartColorTween(currentTarget, (this.m_OptionTransitionColorNormal * this.m_OptionTransitionMultiplier), this.m_OptionTransitionDuration);

                // Get the new value option index
                int newOptionIndex = Mathf.RoundToInt(value);

                // Validate the index
                if (newOptionIndex < 0 || newOptionIndex >= this.m_Options.Count)
                    newOptionIndex = 0;

                // Get the new selected option game object
                GameObject newOptionGameObject = this.m_OptionGameObjects[newOptionIndex];

                if (newOptionGameObject != null)
                {
                    Graphic newTarget =
                        (this.m_OptionTransitionTarget == TransitionTarget.Text) ?
                            (newOptionGameObject.GetComponentInChildren<Text>() as Graphic) :
                            (newOptionGameObject.GetComponent<Image>() as Graphic);

                    // Transition the new target to active state
                    this.StartColorTween(newTarget, (this.m_OptionTransitionColorActive * this.m_OptionTransitionMultiplier), this.m_OptionTransitionDuration);
                }

                // Save the new option game object
                this.m_CurrentOptionGameObject = newOptionGameObject;
            }
        }

        /// <summary>
        /// Starts a color tween.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="targetColor">Target color.</param>
        /// <param name="duration">Duration.</param>
        private void StartColorTween(Graphic target, Color targetColor, float duration)
        {
            if (target == null)
                return;

            if (!Application.isPlaying || duration == 0f)
            {
                target.canvasRenderer.SetColor(targetColor);
            }
            else
            {
                target.CrossFadeColor(targetColor, duration, true, true);
            }
        }

        /// <summary>
        /// Validates the options.
        /// </summary>
        protected void ValidateOptions()
        {
            if (!this.IsActive())
                return;

            if (!this.HasOptions())
            {
                // Destroy the options container if we have one
                if (this.m_OptionsContGameObject != null)
                {
                    if (Application.isPlaying)
                        Destroy(this.m_OptionsContGameObject);
                    else
                        DestroyImmediate(this.m_OptionsContGameObject);
                }
                return;
            }

            // Make sure we have the options container
            if (this.m_OptionsContGameObject == null)
                this.CreateOptionsContainer();

            // Make sure we use whole numbers when using options
            if (!this.wholeNumbers)
                this.wholeNumbers = true;

            // Make sure the max value is the options count, when using options
            this.minValue = 0f;
            this.maxValue = ((float)this.m_Options.Count - 1f);

            // Update the grid properties
            this.UpdateGridProperties();

            // Update the options properties
            this.UpdateOptionsProperties();
        }

        /// <summary>
        /// Updates the grid properties.
        /// </summary>
        public void UpdateGridProperties()
        {
            if (this.m_OptionsContGrid == null)
                return;

            // Grid Padding
            if (!this.m_OptionsContGrid.padding.Equals(this.m_OptionsPadding))
                this.m_OptionsContGrid.padding = this.m_OptionsPadding;

            // Grid Cell Size
            Vector2 cellSize = (this.m_OptionSprite != null) ? new Vector2(this.m_OptionSprite.rect.width, this.m_OptionSprite.rect.height) : Vector2.zero;

            if (!this.m_OptionsContGrid.cellSize.Equals(cellSize))
                this.m_OptionsContGrid.cellSize = cellSize;

            // Grid spacing
            float spacingX = (this.m_OptionsContRect.rect.width - ((float)this.m_OptionsPadding.left + (float)this.m_OptionsPadding.right) - ((float)this.m_Options.Count * cellSize.x)) / ((float)this.m_Options.Count - 1f);

            if (this.m_OptionsContGrid.spacing.x != spacingX)
                this.m_OptionsContGrid.spacing = new Vector2(spacingX, 0f);
        }

        /// <summary>
        /// Updates the options properties.
        /// </summary>
        public void UpdateOptionsProperties()
        {
            if (!this.HasOptions())
                return;

            // Loop through the options
            int i = 0;
            foreach (GameObject optionObject in this.m_OptionGameObjects)
            {
                bool selected = Mathf.RoundToInt(this.value) == i;

                // Save as current
                if (selected)
                    this.m_CurrentOptionGameObject = optionObject;

                // Image
                Image image = optionObject.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = this.m_OptionSprite;
                    image.rectTransform.pivot = new Vector2(0f, 1f);

                    if (this.m_OptionTransition == OptionTransition.ColorTint && this.m_OptionTransitionTarget == TransitionTarget.Image)
                        image.canvasRenderer.SetColor((selected) ? this.m_OptionTransitionColorActive : this.m_OptionTransitionColorNormal);
                    else
                        image.canvasRenderer.SetColor(Color.white);
                }

                // Text
                Text text = optionObject.GetComponentInChildren<Text>();
                if (text != null)
                {
                    // Update the text
                    text.font = this.m_OptionTextFont;
                    text.fontStyle = this.m_OptionTextStyle;
                    text.fontSize = this.m_OptionTextSize;
                    text.color = this.m_OptionTextColor;

                    if (this.m_OptionTransition == OptionTransition.ColorTint && this.m_OptionTransitionTarget == TransitionTarget.Text)
                        text.canvasRenderer.SetColor((selected) ? this.m_OptionTransitionColorActive : this.m_OptionTransitionColorNormal);
                    else
                        text.canvasRenderer.SetColor(Color.white);

                    // Update the text offset
                    (text.transform as RectTransform).anchoredPosition = this.m_OptionTextOffset;

                    // Update the text effects
                    this.UpdateTextEffect(text.gameObject);
                }

                // Increase the indexer
                i++;
            }
        }

        /// <summary>
        /// Rebuilds the options.
        /// </summary>
        protected void RebuildOptions()
        {
            if (!this.HasOptions())
                return;

            // Make sure we have the options container
            if (this.m_OptionsContGameObject == null)
                this.CreateOptionsContainer();

            // Clear out the current options
            this.DestroyOptions();

            // Loop through the options	
            int i = 0;
            foreach (string option in this.m_Options)
            {
                GameObject optionObject = new GameObject("Option " + i.ToString(), typeof(RectTransform), typeof(Image));
                optionObject.layer = this.gameObject.layer;
                optionObject.transform.SetParent(this.m_OptionsContGameObject.transform, false);
                optionObject.transform.localScale = Vector3.one;
                optionObject.transform.localPosition = Vector3.zero;

                // Create the text game object
                GameObject textObject = new GameObject("Text", typeof(RectTransform));
                textObject.layer = this.gameObject.layer;
                textObject.transform.SetParent(optionObject.transform, false);
                textObject.transform.localScale = Vector3.one;
                textObject.transform.localPosition = Vector3.zero;

                // Add the text component and set the text
                Text text = textObject.AddComponent<Text>();
                text.text = option;

                // Add content size fitter
                ContentSizeFitter fitter = textObject.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                // Add to the game objects list
                this.m_OptionGameObjects.Add(optionObject);

                // Increase the indexer
                i++;
            }

            // Update the option properties
            this.UpdateOptionsProperties();
        }

        /// <summary>
        /// Adds the option text effect.
        /// </summary>
        /// <param name="gObject">G object.</param>
        private void AddTextEffect(GameObject gObject)
        {
            // Add new text effect
            if (this.m_OptionTextEffect == TextEffectType.Shadow)
            {
                Shadow s = gObject.AddComponent<Shadow>();
                s.effectColor = this.m_OptionTextEffectColor;
                s.effectDistance = this.m_OptionTextEffectDistance;
                s.useGraphicAlpha = this.m_OptionTextEffectUseGraphicAlpha;
            }
            else if (this.m_OptionTextEffect == TextEffectType.Outline)
            {
                Outline o = gObject.AddComponent<Outline>();
                o.effectColor = this.m_OptionTextEffectColor;
                o.effectDistance = this.m_OptionTextEffectDistance;
                o.useGraphicAlpha = this.m_OptionTextEffectUseGraphicAlpha;
            }
        }

        /// <summary>
        /// Updates the option text effect.
        /// </summary>
        /// <param name="gObject">G object.</param>
        private void UpdateTextEffect(GameObject gObject)
        {
            // Update text text effect
            if (this.m_OptionTextEffect == TextEffectType.Shadow)
            {
                Shadow s = gObject.GetComponent<Shadow>();
                if (s == null) s = gObject.AddComponent<Shadow>();
                s.effectColor = this.m_OptionTextEffectColor;
                s.effectDistance = this.m_OptionTextEffectDistance;
                s.useGraphicAlpha = this.m_OptionTextEffectUseGraphicAlpha;
            }
            else if (this.m_OptionTextEffect == TextEffectType.Outline)
            {
                Outline o = gObject.GetComponent<Outline>();
                if (o == null) o = gObject.AddComponent<Outline>();
                o.effectColor = this.m_OptionTextEffectColor;
                o.effectDistance = this.m_OptionTextEffectDistance;
                o.useGraphicAlpha = this.m_OptionTextEffectUseGraphicAlpha;
            }
        }

        /// <summary>
        /// Rebuilds the options text effects.
        /// </summary>
        public void RebuildTextEffects()
        {
            // Loop through the options
            foreach (GameObject optionObject in this.m_OptionGameObjects)
            {
                Text text = optionObject.GetComponentInChildren<Text>();

                if (text != null)
                {
                    Shadow s = text.gameObject.GetComponent<Shadow>();
                    Outline o = text.gameObject.GetComponent<Outline>();

                    // Destroy any effect we find
                    if (Application.isPlaying)
                    {
                        if (s != null) Destroy(s);
                        if (o != null) Destroy(o);
                    }
                    else
                    {
                        if (s != null) DestroyImmediate(s);
                        if (o != null) DestroyImmediate(o);
                    }

                    // Re-add the effect
                    this.AddTextEffect(text.gameObject);
                }
            }
        }

        /// <summary>
        /// Destroies the current options.
        /// </summary>
        protected void DestroyOptions()
        {
            // Clear out the optins
            foreach (GameObject g in this.m_OptionGameObjects)
            {
                if (Application.isPlaying) Destroy(g);
                else DestroyImmediate(g);
            }

            // Clear the list
            this.m_OptionGameObjects.Clear();
        }

        /// <summary>
        /// Creates the options container.
        /// </summary>
        protected void CreateOptionsContainer()
        {
            // Create new game object
            this.m_OptionsContGameObject = new GameObject("Options Grid", typeof(RectTransform), typeof(GridLayoutGroup));
            this.m_OptionsContGameObject.layer = this.gameObject.layer;
            this.m_OptionsContGameObject.transform.SetParent(this.transform, false);
            this.m_OptionsContGameObject.transform.SetAsFirstSibling();
            this.m_OptionsContGameObject.transform.localScale = Vector3.one;
            this.m_OptionsContGameObject.transform.localPosition = Vector3.zero;

            // Get the rect transform
            this.m_OptionsContRect = this.m_OptionsContGameObject.GetComponent<RectTransform>();
            this.m_OptionsContRect.sizeDelta = new Vector2(0f, 0f);
            this.m_OptionsContRect.anchorMin = new Vector2(0f, 0f);
            this.m_OptionsContRect.anchorMax = new Vector2(1f, 1f);
            this.m_OptionsContRect.pivot = new Vector2(0f, 1f);
            this.m_OptionsContRect.anchoredPosition = new Vector2(0f, 0f);

            // Get the grid layout group
            this.m_OptionsContGrid = this.m_OptionsContGameObject.GetComponent<GridLayoutGroup>();
        }
    }
}
