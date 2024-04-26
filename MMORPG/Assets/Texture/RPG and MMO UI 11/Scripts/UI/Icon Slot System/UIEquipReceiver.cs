using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Icon Slots/Equip Receiver", 46), ExecuteInEditMode]
	public class UIEquipReceiver : UIBehaviour, IEventSystemHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
		
		public enum HintState
		{
			Shown,
			Hidden
		}
		
        #pragma warning disable 0649
		[SerializeField] private Transform m_SlotsContainer;
		
		[SerializeField] private Text m_HintText;
		[SerializeField] private bool m_Fading = true;
		[SerializeField] private float m_FadeDuration = 0.15f;
		[SerializeField] private HintState m_HintState = HintState.Hidden;
		#pragma warning restore 0649

		protected override void Start()
		{
			// Make sure we have a container
			if (Application.isPlaying && this.m_SlotsContainer == null)
				this.m_SlotsContainer = this.transform;
			
			// Prepare the hint text starting state
			if (this.m_HintText != null)
				this.m_HintText.canvasRenderer.SetAlpha(this.m_HintState == HintState.Hidden ? 0f : 1f);
		}
		
#if UNITY_EDITOR
		protected override void OnValidate()
		{
			if (this.m_HintText != null)
				this.m_HintText.canvasRenderer.SetAlpha(this.m_HintState == HintState.Hidden ? 0f : 1f);
		}
#endif
		
		/// <summary>
		/// Gets a slot within it's children by the specified type.
		/// </summary>
		/// <returns>The slot by type.</returns>
		/// <param name="type">Type.</param>
		public UIEquipSlot GetSlotByType(UIEquipmentType type)
		{
			UIEquipSlot[] slots = this.m_SlotsContainer.GetComponentsInChildren<UIEquipSlot>();
			
			// Find a suitable slot for the given type
			foreach (UIEquipSlot slot in slots)
			{
				if (slot.enabled && slot.gameObject.activeSelf && slot.equipType == type)
					return slot;
			}
			
			return null;
		}
		
		/// <summary>
		/// Raises the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_HintText == null)
				return;
			
			// Check if we are dragging
			if (eventData.dragging)
			{
				// Try getting slot base component from the selected object
				UISlotBase slotBase = this.ExtractSlot(eventData);
				
				// If we have a slot, we should show the hint
				if (slotBase != null)
				{
					// Show the hint
					if (this.m_Fading)
					{
						this.m_HintText.CrossFadeAlpha(1f, this.m_FadeDuration, true);
					}
					else
					{
						this.m_HintText.canvasRenderer.SetAlpha(1f);
					}
					
					// Set the hint state
					this.m_HintState = HintState.Shown;
				}
			}
		}
		
		/// <summary>
		/// Hides the hint text if it's visible.
		/// </summary>
		private void HideHint()
		{
			if (this.m_HintState == HintState.Hidden)
				return;
			
			// Hide the hint
			if (this.m_Fading)
			{
				this.m_HintText.CrossFadeAlpha(0f, this.m_FadeDuration, true);
			}
			else
			{
				this.m_HintText.canvasRenderer.SetAlpha(0f);
			}
			
			this.m_HintState = HintState.Hidden;
		}
		
		/// <summary>
		/// Raises the pointer exit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.m_HintText == null)
				return;
			
			// Hide the hint
			this.HideHint();
		}
		
		/// <summary>
		/// Raises the drop event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrop(PointerEventData eventData)
		{
			if (eventData.pointerPress == null)
				return;
			
			// Try getting slot base component from the selected object
			UISlotBase slotBase = this.ExtractSlot(eventData);
			
			// Check if we have a slot
			if (slotBase == null)
				return;
			
			// Determine the type of slot we are dropping here
			if (slotBase is UIItemSlot)
			{
				UIItemSlot itemSlot = (slotBase as UIItemSlot);
				
				// Make sure the slot we are dropping is valid and assigned
				if (itemSlot != null && itemSlot.IsAssigned())
				{
					// Try finding a suitable slot to equip
					UIEquipSlot equipSlot = this.GetSlotByType(itemSlot.GetItemInfo().EquipType);
					
					if (equipSlot != null)
					{
						// Use the drop event to handle equip
						equipSlot.OnDrop(eventData);
						
						// Hide the hint
						this.HideHint();
						
						// Break out of the method
						return;
					}
				}
			}
		}
		
		/// <summary>
		/// Extracts a base slot based on the event data.
		/// </summary>
		/// <returns>The slot.</returns>
		/// <param name="eventData">Event data.</param>
		private UISlotBase ExtractSlot(PointerEventData eventData)
		{
			if (eventData.pointerPress == null)
				return null;
				
			// Try getting slot base component from the selected object
			UISlotBase slotBase = eventData.pointerPress.GetComponent<UISlotBase>();
			
			// Check if we failed to get a slot from the pressed game object directly
			if (slotBase == null)
				slotBase = eventData.pointerPress.GetComponentInChildren<UISlotBase>();
			
			return slotBase;
		}
	}
}
