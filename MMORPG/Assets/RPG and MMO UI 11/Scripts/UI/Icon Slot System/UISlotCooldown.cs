using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Icon Slots/Slot Cooldown", 28)]
	public class UISlotCooldown : MonoBehaviour {
		
		public class CooldownInfo
		{
			public float duration;
			public float startTime;
			public float endTime;
			
			public CooldownInfo(float duration, float startTime, float endTime)
			{
				this.duration = duration;
				this.startTime = startTime;
				this.endTime = endTime;
			}
		}
		
		private static Dictionary<int, CooldownInfo> spellCooldowns = new Dictionary<int, CooldownInfo>();
		
        #pragma warning disable 0649
		[SerializeField] private UISlotBase m_TargetSlot;
		[SerializeField] private Image m_TargetGraphic;
		[SerializeField] private Text m_TargetText;
		[SerializeField] private Image m_FinishGraphic;
		[SerializeField] private float m_FinishOffsetY = 0f;
		[SerializeField] private float m_FinishFadingPct = 25f;
        #pragma warning restore 0649

        private IUISlotHasCooldown m_CooldownSlot;
		private bool m_IsOnCooldown = false;
		private int m_CurrentSpellId = 0;

        protected void Awake()
        {
            // Set the cooldown component variable on the target slot
            if (this.m_TargetSlot != null && this.m_TargetSlot is IUISlotHasCooldown)
            {
                this.m_CooldownSlot = this.m_TargetSlot as IUISlotHasCooldown;
                this.m_CooldownSlot.SetCooldownComponent(this);
            }
            else
            {
                Debug.LogWarning("The slot cooldown script cannot operate without a target slot with a IUISlotHasCooldown interface, disabling script.");
                this.enabled = false;
                return;
            }
        }

        protected void Start()
		{
			// Disable the image and text if they are shown by any chance
			if (this.m_TargetGraphic != null) this.m_TargetGraphic.enabled = false;
			if (this.m_TargetText != null) this.m_TargetText.enabled = false;
			
			// Prepare the finish
			if (this.m_FinishGraphic != null)
			{
				this.m_FinishGraphic.enabled = false;
				this.m_FinishGraphic.rectTransform.anchorMin = new Vector2(this.m_FinishGraphic.rectTransform.anchorMin.x, 1f);
				this.m_FinishGraphic.rectTransform.anchorMax = new Vector2(this.m_FinishGraphic.rectTransform.anchorMax.x, 1f);
			}
		}

        protected void OnEnable()
        {
            // Check for active cooldown
            this.CheckForActiveCooldown();
        }

        protected void OnDisable()
		{
			// Disable any current cooldown display
			this.InterruptCooldown();
        }
		
		/// <summary>
		/// Gets a value indicating whether the cooldown is active.
		/// </summary>
		/// <value><c>true</c> if this instance is on cooldown; otherwise, <c>false</c>.</value>
		public bool IsOnCooldown
		{
			get { return this.m_IsOnCooldown; }
		}
		
		/// <summary>
		/// Raises the assign spell event.
		/// </summary>
		public void OnAssignSpell()
		{
            this.CheckForActiveCooldown();
        }
		
		/// <summary>
		/// Raises the unassign event.
		/// </summary>
		public void OnUnassignSpell()
		{
			this.InterruptCooldown();
		}
		
        /// <summary>
        /// Checks if the target slot has an active cooldown and resumes it.
        /// </summary>
        public void CheckForActiveCooldown()
        {
            // Return if the slot is not valid
            if (this.m_CooldownSlot == null)
                return;
            
            // Get the spell info
            UISpellInfo spellInfo = this.m_CooldownSlot.GetSpellInfo();

            if (spellInfo == null)
                return;

            // Check if this spell still has cooldown
            if (spellCooldowns.ContainsKey(spellInfo.ID))
            {
                float cooldownTill = spellCooldowns[spellInfo.ID].endTime;

                // Check if the cooldown isnt expired
                if (cooldownTill > Time.time)
                {
                    this.m_IsOnCooldown = true;

                    // Resume the cooldown
                    this.ResumeCooldown(spellInfo.ID);
                }
                else
                {
                    // Cooldown already expired, remove the record
                    spellCooldowns.Remove(spellInfo.ID);
                }
            }
        }

		/// <summary>
		/// Starts a cooldown.
		/// </summary>
		/// <param name="spellId">Spell identifier.</param>
		/// <param name="duration">Duration.</param>
		public void StartCooldown(int spellId, float duration)
		{
			if (!this.enabled || !this.gameObject.activeInHierarchy || this.m_TargetGraphic == null)
				return;
			
			// Save the spell id
			this.m_CurrentSpellId = spellId;
			
			// Enable the image if it's disabled
			if (!this.m_TargetGraphic.enabled)
				this.m_TargetGraphic.enabled = true;
			
			// Reset the fill amount
			this.m_TargetGraphic.fillAmount = 1f;
			
			// Enable the text if it's disabled
			if (this.m_TargetText != null)
			{
				if (!this.m_TargetText.enabled)
					this.m_TargetText.enabled = true;
				
				this.m_TargetText.text = duration.ToString("0");
			}
			
			// Prepare the finish graphic
			if (this.m_FinishGraphic != null)
			{
				this.m_FinishGraphic.canvasRenderer.SetAlpha(0f);
				this.m_FinishGraphic.enabled = true;
				this.m_FinishGraphic.rectTransform.anchoredPosition = new Vector2(
					this.m_FinishGraphic.rectTransform.anchoredPosition.x, 
					this.m_FinishOffsetY
				);
			}
			
			// Set the slot on cooldown
			this.m_IsOnCooldown = true;
			
			// Create new cooldown info
			CooldownInfo cooldownInfo = new CooldownInfo(duration, Time.time, (Time.time + duration));
			
			// Save that this spell is on cooldown
			if (!spellCooldowns.ContainsKey(spellId))
				spellCooldowns.Add(spellId, cooldownInfo);
			
			// Start the coroutine
			this.StartCoroutine("_StartCooldown", cooldownInfo);
		}
		
		/// <summary>
		/// Resumes a cooldown.
		/// </summary>
		/// <param name="spellId">Spell identifier.</param>
		public void ResumeCooldown(int spellId)
		{
			if (!this.enabled || !this.gameObject.activeInHierarchy || this.m_TargetGraphic == null)
				return;
			
			// Check if we have the cooldown info for that spell
			if (!spellCooldowns.ContainsKey(spellId))
				return;
				
			// Get the cooldown info
			CooldownInfo cooldownInfo = spellCooldowns[spellId];
			
			// Get the remaining time
			float remainingTime = (cooldownInfo.endTime - Time.time);
			float remainingTimePct = remainingTime / cooldownInfo.duration;
			
			// Save the spell id
			this.m_CurrentSpellId = spellId;
			
			// Enable the image if it's disabled
			if (!this.m_TargetGraphic.enabled)
				this.m_TargetGraphic.enabled = true;
			
			// Set the fill amount to the remaing percents
			this.m_TargetGraphic.fillAmount = (remainingTime / cooldownInfo.duration);
			
			// Enable the text if it's disabled
			if (this.m_TargetText != null)
			{
				if (!this.m_TargetText.enabled)
					this.m_TargetText.enabled = true;
				
				this.m_TargetText.text = remainingTime.ToString("0");
			}
			
			// Update the finish
			if (this.m_FinishGraphic != null)
			{
				this.m_FinishGraphic.enabled = true;
				this.UpdateFinishPosition(remainingTimePct);
			}
			
			// Start the coroutine
			this.StartCoroutine("_StartCooldown", cooldownInfo);
		}
		
		/// <summary>
		/// Interrupts the current cooldown.
		/// </summary>
		public void InterruptCooldown()
		{
			// Cancel the coroutine
			this.StopCoroutine("_StartCooldown");
			
			// Call the finish
			this.OnCooldownFinished();
		}
		
		IEnumerator _StartCooldown(CooldownInfo cooldownInfo)
		{
			while (Time.time < (cooldownInfo.startTime + cooldownInfo.duration))
			{
				float RemainingTime = (cooldownInfo.startTime + cooldownInfo.duration) - Time.time;
				float RemainingTimePct = RemainingTime / cooldownInfo.duration;
				
				// Update the cooldown image
				if (this.m_TargetGraphic != null)
					this.m_TargetGraphic.fillAmount = RemainingTimePct;
				
				// Update the text
				if (this.m_TargetText != null)
					this.m_TargetText.text = RemainingTime.ToString("0");
				
				// Update the finish position
				this.UpdateFinishPosition(RemainingTimePct);
				
				yield return 0;
			}
			
			// Call the on finish
			this.OnCooldownCompleted();
		}
		
		/// <summary>
		/// Raised when the cooldown completes it's full duration.
		/// </summary>
		private void OnCooldownCompleted()
		{
			// Remove from the cooldowns list
			if (spellCooldowns.ContainsKey(this.m_CurrentSpellId))
				spellCooldowns.Remove(this.m_CurrentSpellId);
			
			// Fire up the cooldown finished
			this.OnCooldownFinished();
		}
		
		/// <summary>
		/// Raised when the cooldown finishes or has been interrupted.
		/// </summary>
		private void OnCooldownFinished()
		{
			// No longer on cooldown
			this.m_IsOnCooldown = false;
			this.m_CurrentSpellId = 0;
			
			// Disable the image
			if (this.m_TargetGraphic != null)
				this.m_TargetGraphic.enabled = false;
			
			// Disable the text
			if (this.m_TargetText != null)
				this.m_TargetText.enabled = false;
				
			// Disable the finish sprite
			if (this.m_FinishGraphic != null)
				this.m_FinishGraphic.enabled = false;
		}
		
		/// <summary>
		/// Updates the finish position based on the remaining time percentage of the cooldown.
		/// </summary>
		/// <param name="RemainingTimePct">Remaining time pct.</param>
		protected void UpdateFinishPosition(float RemainingTimePct)
		{
			// Update the finish position
			if (this.m_FinishGraphic != null && this.m_TargetGraphic != null)
			{
				// Calculate the fill position
				float newY = (0f - this.m_TargetGraphic.rectTransform.rect.height + (this.m_TargetGraphic.rectTransform.rect.height * RemainingTimePct));
				
				// Add the offset
				newY += this.m_FinishOffsetY;
				
				// Update the finish position
				this.m_FinishGraphic.rectTransform.anchoredPosition = new Vector2(
					this.m_FinishGraphic.rectTransform.anchoredPosition.x, 
					newY
					);
				
				float fadingPct = this.m_FinishFadingPct / 100;
				
				// Manage finish fading
				if (RemainingTimePct <= fadingPct)
				{
					this.m_FinishGraphic.canvasRenderer.SetAlpha(RemainingTimePct / fadingPct);
				}
				else if (RemainingTimePct >= (1f - fadingPct))
				{
					this.m_FinishGraphic.canvasRenderer.SetAlpha(1f - (RemainingTimePct - (1f - fadingPct)) / fadingPct);
				}
				else if (RemainingTimePct > fadingPct && RemainingTimePct < (1f - fadingPct))
				{
					this.m_FinishGraphic.canvasRenderer.SetAlpha(1f);
				}
			}
		}
	}
}
