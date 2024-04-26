using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace DuloGames.UI
{
    public class Demo_XPTooltip : UIBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #pragma warning disable 0649
        [SerializeField] private GameObject m_TooltipObject;
        [SerializeField] private UIProgressBar m_ProgressBar;
        [SerializeField] private Text m_PercentText;
        [SerializeField] private float m_OffsetY = 0f;
        #pragma warning restore 0649

        [SerializeField, Tooltip("How long of a delay to expect before showing the tooltip."), Range(0f, 10f)]
        private float m_Delay = 1f;

        private bool m_IsTooltipShown = false;

        protected override void Awake()
        {
            base.Awake();

            if (this.m_TooltipObject != null)
            {
                RectTransform tooltipRect = (this.m_TooltipObject.transform as RectTransform);
                tooltipRect.anchorMin = new Vector2(0f, 1f);
                tooltipRect.anchorMax = new Vector2(0f, 1f);
                tooltipRect.pivot = new Vector2(0.5f, 0f);
                this.m_TooltipObject.SetActive(false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (this.m_ProgressBar != null)
                this.m_ProgressBar.onChange.AddListener(OnProgressChange);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (this.m_ProgressBar != null)
                this.m_ProgressBar.onChange.RemoveListener(OnProgressChange);
        }

        private void OnProgressChange(float value)
        {
            // Update tooltip position
            this.UpdatePosition();
        }

        /// <summary>
        /// Raises the tooltip event.
        /// </summary>
        /// <param name="show">If set to <c>true</c> show.</param>
        public virtual void OnTooltip(bool show)
        {
            if (this.m_TooltipObject == null)
                return;
            
            if (show)
            {
                // Update tooltip position
                this.UpdatePosition();

                // Enable the tooltip
                this.m_TooltipObject.SetActive(true);
            }
            else
            {
                // Disable the tooltip
                this.m_TooltipObject.SetActive(false);
            }
        }

        /// <summary>
        /// Raises the pointer enter event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            // Check if tooltip is enabled
            if (this.enabled && this.IsActive())
            {
                // Start the tooltip delayed show coroutine
                // If delay is set at all
                if (this.m_Delay > 0f)
                {
                    this.StartCoroutine("DelayedShow");
                }
                else
                {
                    this.InternalShowTooltip();
                }
            }
        }

        /// <summary>
        /// Raises the pointer exit event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            this.InternalHideTooltip();
        }

        /// <summary>
		/// Internal call for show tooltip.
		/// </summary>
		protected void InternalShowTooltip()
        {
            // Call the on tooltip only if it's currently not shown
            if (!this.m_IsTooltipShown)
            {
                this.m_IsTooltipShown = true;
                this.OnTooltip(true);
            }
        }

        /// <summary>
        /// Internal call for hide tooltip.
        /// </summary>
        protected void InternalHideTooltip()
        {
            // Cancel the delayed show coroutine
            this.StopCoroutine("DelayedShow");

            // Call the on tooltip only if it's currently shown
            if (this.m_IsTooltipShown)
            {
                this.m_IsTooltipShown = false;
                this.OnTooltip(false);
            }
        }

        protected IEnumerator DelayedShow()
        {
            yield return new WaitForSeconds(this.m_Delay);
            this.InternalShowTooltip();
        }

        public void UpdatePosition()
        {
            if (this.m_ProgressBar == null || this.m_TooltipObject == null)
                return;
            
            RectTransform tooltipRect = (this.m_TooltipObject.transform as RectTransform);
            RectTransform fillRect = (this.m_ProgressBar.type == UIProgressBar.Type.Filled ? (this.m_ProgressBar.targetImage.transform as RectTransform) : (this.m_ProgressBar.targetTransform.parent as RectTransform));

            Transform parent = tooltipRect.parent;

            // Change the parent so we can calculate the position correctly
            tooltipRect.SetParent(fillRect, true);

            // Change the position based on fill
            tooltipRect.anchoredPosition = new Vector2(fillRect.rect.width * this.m_ProgressBar.fillAmount, this.m_OffsetY);

            // Bring to top
            tooltipRect.SetParent(parent, true);

            // Set the percent text
            if (this.m_PercentText != null)
                this.m_PercentText.text = (this.m_ProgressBar.fillAmount * 100f).ToString("0") + "%";
        } 
    }
}
