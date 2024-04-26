using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Icon Slots/Item Slot", 12)]
	public class UIItemSlot : UISlotBase, IUIItemSlot
    {
        [Serializable] public class OnRightClickEvent : UnityEvent<UIItemSlot> { }
        [Serializable] public class OnDoubleClickEvent : UnityEvent<UIItemSlot> { }
		[Serializable] public class OnAssignEvent : UnityEvent<UIItemSlot> { }
        [Serializable] public class OnAssignWithSourceEvent : UnityEvent<UIItemSlot, Object> { }
		[Serializable] public class OnUnassignEvent : UnityEvent<UIItemSlot> { }
		
		[SerializeField] private UIItemSlot_Group m_SlotGroup = UIItemSlot_Group.None;
		[SerializeField] private int m_ID = 0;
		
		/// <summary>
		/// Gets or sets the slot group.
		/// </summary>
		/// <value>The slot group.</value>
		public UIItemSlot_Group slotGroup
		{
			get { return this.m_SlotGroup; }
			set { this.m_SlotGroup = value; }
		}
		
		/// <summary>
		/// Gets or sets the slot ID.
		/// </summary>
		/// <value>The I.</value>
		public int ID
		{
			get { return this.m_ID; }
			set { this.m_ID = value; }
		}
		
		/// <summary>
		/// The assigned item info.
		/// </summary>
		private UIItemInfo m_ItemInfo;

        /// <summary>
        /// The right click event delegate.
        /// </summary>
        public OnRightClickEvent onRightClick = new OnRightClickEvent();

        /// <summary>
        /// The double click event delegate.
        /// </summary>
        public OnDoubleClickEvent onDoubleClick = new OnDoubleClickEvent();

        /// <summary>
        /// The assign event delegate.
        /// </summary>
        public OnAssignEvent onAssign = new OnAssignEvent();

        /// <summary>
        /// The assign with source event delegate.
        /// </summary>
        public OnAssignWithSourceEvent onAssignWithSource = new OnAssignWithSourceEvent();

        /// <summary>
        /// The unassign event delegate.
        /// </summary>
        public OnUnassignEvent onUnassign = new OnUnassignEvent();
		
		/// <summary>
		/// Gets the item info of the item assigned to this slot.
		/// </summary>
		/// <returns>The spell info.</returns>
		public UIItemInfo GetItemInfo()
		{
			return this.m_ItemInfo;
		}
        
        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_EDITOR
            if (!this.IsInPrefabStage())
            {
                // Check for duplicate id
                List<UIItemSlot> slots = GetSlotsInGroup(this.m_SlotGroup);
                UIItemSlot duplicate = slots.Find(x => x.ID == this.m_ID && !x.Equals(this));

                if (duplicate != null)
                {
                    int oldId = this.m_ID;
                    this.AutoAssignID();
                    Debug.LogWarning("Item Slot with duplicate ID: " + oldId + " in Group: " + this.m_SlotGroup + ", generating and assigning new ID: " + this.m_ID + ".");
                }
            }
#endif
        }

#if UNITY_EDITOR
        private bool IsInPrefabStage()
        {
#if UNITY_2018_3_OR_NEWER
                return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
#else
                return false;
#endif
        }
#endif

        /// <summary>
        /// Determines whether this slot is assigned.
        /// </summary>
        /// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
        public override bool IsAssigned()
		{
			return (this.m_ItemInfo != null);
		}

        /// <summary>
        /// Assign the slot by new item info while refering to the source.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="source">The source slot (Could be null).</param>
        /// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
        public bool Assign(UIItemInfo itemInfo, Object source)
        {
            if (itemInfo == null)
                return false;

            // Make sure we unassign first, so the event is called before new assignment
            this.Unassign();

            // Use the base class assign to set the icon
            this.Assign(itemInfo.Icon);

            // Set the spell info
            this.m_ItemInfo = itemInfo;

            // Invoke the on assign event
            if (this.onAssign != null)
                this.onAssign.Invoke(this);

            // Invoke the on assign event
            if (this.onAssignWithSource != null)
                this.onAssignWithSource.Invoke(this, source);

            // Success
            return true;
        }

		/// <summary>
		/// Assign the slot by item info.
		/// </summary>
		/// <param name="itemInfo">The item info.</param>
		public bool Assign(UIItemInfo itemInfo)
		{
            return this.Assign(itemInfo, null);
		}
		
		/// <summary>
		/// Assign the slot by the passed source slot.
		/// </summary>
		/// <param name="source">Source.</param>
		public override bool Assign(Object source)
		{
			if (source is IUIItemSlot)
			{
                IUIItemSlot sourceSlot = source as IUIItemSlot;
				
				if (sourceSlot != null)
					return this.Assign(sourceSlot.GetItemInfo(), source);
			}
			
			// Default
			return false;
		}
		
		/// <summary>
		/// Unassign this slot.
		/// </summary>
		public override void Unassign()
		{
			// Remove the icon
			base.Unassign();
			
			// Clear the spell info
			this.m_ItemInfo = null;
			
			// Invoke the on unassign event
			if (this.onUnassign != null)
				this.onUnassign.Invoke(this);
		}
		
		/// <summary>
		/// Determines whether this slot can swap with the specified target slot.
		/// </summary>
		/// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
		/// <param name="target">Target.</param>
		public override bool CanSwapWith(Object target)
		{
			if (target is IUIItemSlot)
			{
				// Check if the equip slot accpets this item
				if (target is UIEquipSlot)
				{
					return (target as UIEquipSlot).CheckEquipType(this.GetItemInfo());
				}
				
				// It's an item slot
				return true;
			}
			
			// Default
			return false;
		}
		
		// <summary>
		/// Performs a slot swap.
		/// </summary>
		/// <returns><c>true</c>, if slot swap was performed, <c>false</c> otherwise.</returns>
		/// <param name="sourceSlot">Source slot.</param>
		public override bool PerformSlotSwap(Object sourceObject)
		{
            // Get the source slot
            IUIItemSlot sourceSlot = (sourceObject as IUIItemSlot);
			
			// Get the source item info
			UIItemInfo sourceItemInfo = sourceSlot.GetItemInfo();

            // Assign the source slot by this slot
			bool assign1 = sourceSlot.Assign(this.GetItemInfo(), this);

			// Assign this slot by the source slot
			bool assign2 = this.Assign(sourceItemInfo, sourceObject);
			
			// Return the status
			return (assign1 && assign2);
		}
		
		/// <summary>
		/// Raises the tooltip event.
		/// </summary>
		/// <param name="show">If set to <c>true</c> show.</param>
		public override void OnTooltip(bool show)
		{
			// Make sure we have spell info, otherwise game might crash
			if (this.m_ItemInfo == null)
				return;
			
			// If we are showing the tooltip
			if (show)
			{
                UITooltip.InstantiateIfNecessary(this.gameObject);

                // Prepare the tooltip
                UIItemSlot.PrepareTooltip(this.m_ItemInfo);
				
				// Anchor to this slot
				UITooltip.AnchorToRect(this.transform as RectTransform);
				
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
		/// Raises the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            // Make sure the slot is assigned
            if (!this.IsAssigned())
                return;

            // Check for left double click
            if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
            {
                // Reset the click count
                eventData.clickCount = 0;

                // Invoke the double click event
                if (this.onDoubleClick != null)
                    this.onDoubleClick.Invoke(this);
            }

            // Check for right click
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Invoke the double click event
                if (this.onRightClick != null)
                    this.onRightClick.Invoke(this);
            }
        }

        /// <summary>
		/// This method is raised when the slot is denied to be thrown away and returned to it's source.
		/// </summary>
        protected override void OnThrowAwayDenied()
        {
            if (!this.IsAssigned())
                return;

            if (UIModalBoxManager.Instance == null)
            {
                Debug.LogWarning("Could not load the modal box manager while creating a modal box.");
                return;
            }

            UIModalBox box = UIModalBoxManager.Instance.Create(this.gameObject);
            if (box != null)
            {
                box.SetText1("Do you really want to destroy \"" + this.m_ItemInfo.Name + "\"?");
                box.SetText2("You wont be able to reverse this operation and your item will be permamently removed.");
                box.SetConfirmButtonText("DESTROY");
                box.onConfirm.AddListener(Unassign);
                box.Show();
            }
        }

        /// <summary>
        /// Automatically generate and assign slot ID.
        /// </summary>
        [ContextMenu("Auto Assign ID")]
        public void AutoAssignID()
        {
            // Get the active slots in the slot's group
            List<UIItemSlot> slots = GetSlotsInGroup(this.m_SlotGroup);

            if (slots.Count > 0)
            {
                slots.Reverse();
                this.m_ID = slots[0].ID + 1;
            }
            else
            {
                // If we have no slots
                this.m_ID = 1;
            }

            slots.Clear();
        }

#if UNITY_EDITOR
        [ContextMenu("Auto Assign ID By Sibling Index")]
        public void AutoAssignIDBySiblingIndex()
        {
			this.m_ID = this.transform.GetSiblingIndex() + 1;
        }
#endif

        #region Static Methods
        /// <summary>
        /// Gets all the item slots.
        /// </summary>
        /// <returns>The slots.</returns>
        public static List<UIItemSlot> GetSlots()
		{
			List<UIItemSlot> slots = new List<UIItemSlot>();
			UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();
			
			foreach (UIItemSlot s in sl)
			{
				// Check if the slow is active in the hierarchy
				if (s.gameObject.activeInHierarchy)
					slots.Add(s);
			}
			
			return slots;
		}
		
		/// <summary>
		/// Gets all the item slots with the specified ID.
		/// </summary>
		/// <returns>The slots.</returns>
		/// <param name="ID">The slot ID.</param>
		public static List<UIItemSlot> GetSlotsWithID(int ID)
		{
			List<UIItemSlot> slots = new List<UIItemSlot>();
			UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();
			
			foreach (UIItemSlot s in sl)
			{
				// Check if the slow is active in the hierarchy
				if (s.gameObject.activeInHierarchy && s.ID == ID)
					slots.Add(s);
			}
			
			return slots;
		}
		
		/// <summary>
		/// Gets all the item slots in the specified group.
		/// </summary>
		/// <returns>The slots.</returns>
		/// <param name="group">The item slot group.</param>
		public static List<UIItemSlot> GetSlotsInGroup(UIItemSlot_Group group)
		{
			List<UIItemSlot> slots = new List<UIItemSlot>();
			UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();
			
			foreach (UIItemSlot s in sl)
			{
				// Check if the slow is active in the hierarchy
				if (s.gameObject.activeInHierarchy && s.slotGroup == group)
					slots.Add(s);
			}

            // Sort the slots by id
            slots.Sort(delegate (UIItemSlot a, UIItemSlot b)
            {
                return a.ID.CompareTo(b.ID);
            });

			return slots;
		}
		
		/// <summary>
		/// Gets the slot with the specified ID and Group.
		/// </summary>
		/// <returns>The slot.</returns>
		/// <param name="ID">The slot ID.</param>
		/// <param name="group">The slot Group.</param>
		public static UIItemSlot GetSlot(int ID, UIItemSlot_Group group)
		{
			UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();
			
			foreach (UIItemSlot s in sl)
			{
				// Check if the slow is active in the hierarchy
				if (s.gameObject.activeInHierarchy && s.ID == ID && s.slotGroup == group)
					return s;
			}
			
			return null;
		}

        /// <summary>
		/// Prepares the tooltip with the specified item info.
		/// </summary>
		/// <param name="itemInfo">Item info.</param>
		public static void PrepareTooltip(UIItemInfo itemInfo)
        {
            if (itemInfo == null)
                return;

            // Set the tooltip width
            if (UITooltipManager.Instance != null)
                UITooltip.SetWidth(UITooltipManager.Instance.itemTooltipWidth);

            // Set the title and description
            UITooltip.AddTitle("<color=#" + UIItemQualityColor.GetHexColor(itemInfo.Quality) + ">" + itemInfo.Name + "</color>");

            // Spacer
            UITooltip.AddSpacer();

            // Item types
            UITooltip.AddLineColumn(itemInfo.Type, "ItemAttribute");
            UITooltip.AddLineColumn(itemInfo.Subtype, "ItemAttribute");

            if (itemInfo.ItemType == 1)
            {
                UITooltip.AddLineColumn(itemInfo.Damage.ToString() + " Damage", "ItemAttribute");
                UITooltip.AddLineColumn(itemInfo.AttackSpeed.ToString("0.0") + " Attack speed", "ItemAttribute");

                UITooltip.AddLine("(" + ((float)itemInfo.Damage / itemInfo.AttackSpeed).ToString("0.0") + " damage per second)", "ItemAttribute");
            }
            else
            {
                UITooltip.AddLineColumn(itemInfo.Armor.ToString() + " Armor", "ItemAttribute");
                UITooltip.AddLineColumn(itemInfo.Block.ToString() + " Block", "ItemAttribute");
            }

            UITooltip.AddSpacer();

            UITooltip.AddLine("+" + itemInfo.Stamina.ToString() + " Stamina", "ItemStat");
            UITooltip.AddLine("+" + itemInfo.Strength.ToString() + " Strength", "ItemStat");

            UITooltip.AddSpacer();

            UITooltip.AddLine("Durability " + itemInfo.Durability + "/" + itemInfo.Durability, "ItemAttribute");

            if (itemInfo.RequiredLevel > 0)
                UITooltip.AddLine("Requires Level " + itemInfo.RequiredLevel, "ItemAttribute");

            // Set the item description if not empty
            if (!string.IsNullOrEmpty(itemInfo.Description))
            {
                UITooltip.AddSpacer();
                UITooltip.AddLine(itemInfo.Description, "ItemDescription");
            }
        }
        #endregion
    }
}
