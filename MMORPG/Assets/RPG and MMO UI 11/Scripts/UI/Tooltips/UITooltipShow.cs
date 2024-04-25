using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DuloGames.UI
{
    [AddComponentMenu("UI/Tooltip Show", 58), DisallowMultipleComponent]
    public class UITooltipShow : UIBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum Position
        {
            Floating,
            Anchored
        }

        public enum WidthMode
        {
            Default,
            Preferred
        }
        
        [SerializeField] private Position m_Position = Position.Floating;
        [SerializeField] private WidthMode m_WidthMode = WidthMode.Default;
        [SerializeField] private bool m_OverrideOffset = false;
        [SerializeField] private Vector2 m_Offset = Vector2.zero;

        [SerializeField, Tooltip("How long of a delay to expect before showing the tooltip."), Range(0f, 10f)]
        private float m_Delay = 1f;
        
        [SerializeField] private UITooltipLineContent[] m_ContentLines = new UITooltipLineContent[0];
        
        private bool m_IsTooltipShown = false;

        /// <summary>
        /// Gets or sets the tooltip content lines.
        /// </summary>
        public UITooltipLineContent[] contentLines
        {
            get { return this.m_ContentLines; }
            set { this.m_ContentLines = value; }
        }

        /// <summary>
        /// Raises the tooltip event.
        /// </summary>
        /// <param name="show">If set to <c>true</c> show.</param>
        public virtual void OnTooltip(bool show)
        {
            if (!this.enabled || !this.IsActive())
                return;
            
            // If we are showing the tooltip
            if (show)
            {
                UITooltip.InstantiateIfNecessary(this.gameObject);

                for (int i = 0; i < this.m_ContentLines.Length; i++)
                {
                    UITooltipLineContent line = this.m_ContentLines[i];

                    if (line.IsSpacer)
                    {
                        UITooltip.AddSpacer();
                    }
                    else
                    {
                        if (line.LineStyle != UITooltipLines.LineStyle.Custom)
                        {
                            UITooltip.AddLine(line.Content, line.LineStyle);
                        }
                        else
                        {
                            UITooltip.AddLine(line.Content, line.CustomLineStyle);
                        }
                    }
                }
                
                if (this.m_WidthMode == WidthMode.Preferred)
                    UITooltip.SetHorizontalFitMode(ContentSizeFitter.FitMode.PreferredSize);

                // Anchor to this slot
                if (this.m_Position == Position.Anchored)
                    UITooltip.AnchorToRect(this.transform as RectTransform);

                // Handle offset override
                if (this.m_OverrideOffset)
                {
                    UITooltip.OverrideOffset(this.m_Offset);
                    UITooltip.OverrideAnchoredOffset(this.m_Offset);
                }

                // Show the tooltip
                UITooltip.Show();
            }
            else
            {
                // Hide the tooltip
                UITooltip.Hide();
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
                // Instantiate the tooltip now
                UITooltip.InstantiateIfNecessary(this.gameObject);

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
        /// Raises the pointer down event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Hide the tooltip
            this.InternalHideTooltip();
        }

        /// <summary>
        /// Raises the pointer up event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
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
    }
}
