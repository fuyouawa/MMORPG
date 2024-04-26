using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Icon Slots/Base Slot"), ExecuteInEditMode, DisallowMultipleComponent]
	public class UISlotBase : UIBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
		
		public enum Transition
		{
			None,
			ColorTint,
			SpriteSwap,
			Animation
		}
		
		public enum DragKeyModifier
		{
			None,
			Control,
			Alt,
			Shift
		}
		
		/// <summary>
		/// The current dragged object.
		/// </summary>
		protected GameObject m_CurrentDraggedObject;
		
		/// <summary>
		/// The current dragging plane.
		/// </summary>
		protected RectTransform m_CurrentDraggingPlane;
		
		/// <summary>
		/// The target icon graphic.
		/// </summary>
		public Graphic iconGraphic;

        #pragma warning disable 0649
        [SerializeField, Tooltip("The game object that should be cloned on drag.")]
        private GameObject m_CloneTarget;

		[SerializeField, Tooltip("Should the drag and drop functionallty be enabled.")]
		private bool m_DragAndDropEnabled = true;

		[SerializeField, Tooltip("If set to static the slot won't be unassigned when drag and drop is preformed.")]
		private bool m_IsStatic = false;
		
		[SerializeField, Tooltip("Should the icon assigned to the slot be throwable.")]
		private bool m_AllowThrowAway = true;
		
		[SerializeField, Tooltip("The key which should be held down in order to begin the drag.")]
		private DragKeyModifier m_DragKeyModifier = DragKeyModifier.None;
		
		[SerializeField, Tooltip("Should the tooltip functionallty be enabled.")]
		private bool m_TooltipEnabled = true;
		
		[SerializeField, Tooltip("How long of a delay to expect before showing the tooltip.")]
		private float m_TooltipDelay = 1f;
        #pragma warning restore 0649

		public Transition hoverTransition = Transition.None;
		public Graphic hoverTargetGraphic;
		public Color hoverNormalColor = Color.white;
		public Color hoverHighlightColor = Color.white;
		public float hoverTransitionDuration = 0.15f;
		public Sprite hoverOverrideSprite;
		public string hoverNormalTrigger = "Normal";
		public string hoverHighlightTrigger = "Highlighted";
		
		public Transition pressTransition = Transition.None;
		public Graphic pressTargetGraphic;
		public Color pressNormalColor = Color.white;
		public Color pressPressColor = new Color(0.6117f, 0.6117f, 0.6117f, 1f);
		public float pressTransitionDuration = 0.15f;
		public Sprite pressOverrideSprite;
		public string pressNormalTrigger = "Normal";
		public string pressPressTrigger = "Pressed";
		
		[SerializeField, Tooltip("Should the pressed state transition to normal state instantly.")]
		private bool m_PressTransitionInstaOut = true;
		
		[SerializeField, Tooltip("Should the pressed state force normal state transition on the hover target.")]
		private bool m_PressForceHoverNormal = true;
		
		private bool isPointerDown = false;
		private bool isPointerInside = false;
		private bool m_DragHasBegan = false;
		private bool m_DropPreformed = false;
		private bool m_IsTooltipShown = false;
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UISlotBase"/> drag and drop is enabled.
		/// </summary>
		/// <value><c>true</c> if drag and drop enabled; otherwise, <c>false</c>.</value>
		public bool dragAndDropEnabled
		{
			get
			{
				return this.m_DragAndDropEnabled;
			}
			set
			{
				this.m_DragAndDropEnabled = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UISlotBase"/> is static.
		/// </summary>
		/// <value><c>true</c> if is static; otherwise, <c>false</c>.</value>
		public bool isStatic
		{
			get
			{
				return this.m_IsStatic;
			}
			set
			{
				this.m_IsStatic = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UISlotBase"/> can be throw away.
		/// </summary>
		/// <value><c>true</c> if allow throw away; otherwise, <c>false</c>.</value>
		public bool allowThrowAway
		{
			get
			{
				return this.m_AllowThrowAway;
			}
			set
			{
				this.m_AllowThrowAway = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the drag key modifier.
		/// </summary>
		/// <value>The drag key modifier.</value>
		public DragKeyModifier dragKeyModifier
		{
			get
			{
				return this.m_DragKeyModifier;
			}
			set
			{
				this.m_DragKeyModifier = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UISlotBase"/> tooltip should be enabled.
		/// </summary>
		/// <value><c>true</c> if tooltip enabled; otherwise, <c>false</c>.</value>
		public bool tooltipEnabled
		{
			get
			{
				return this.m_TooltipEnabled;
			}
			set
			{
				this.m_TooltipEnabled = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the tooltip delay.
		/// </summary>
		/// <value>The tooltip delay.</value>
		public float tooltipDelay
		{
			get
			{
				return this.m_TooltipDelay;
			}
			set
			{
				this.m_TooltipDelay = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UISlotBase"/> pressed state should transition out instantly.
		/// </summary>
		/// <value><c>true</c> if press transition insta out; otherwise, <c>false</c>.</value>
		public bool pressTransitionInstaOut
		{
			get
			{
				return this.m_PressTransitionInstaOut;
			}
			set
			{
				this.m_PressTransitionInstaOut = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UISlotBase"/> pressed state should force normal state transition on the hover target.
		/// </summary>
		/// <value><c>true</c> if press force hover normal; otherwise, <c>false</c>.</value>
		public bool pressForceHoverNormal
		{
			get
			{
				return this.m_PressForceHoverNormal;
			}
			set
			{
				this.m_PressForceHoverNormal = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UISlotBase"/> drop was preformed.
		/// </summary>
		/// <value><c>true</c> if drop preformed; otherwise, <c>false</c>.</value>
		public bool dropPreformed
		{
			get
			{
				return this.m_DropPreformed;
			}
			set
			{
				this.m_DropPreformed = value;
			}
		}
		
		protected override void Start()
		{
			// Check if the slot is not assigned but the icon graphic is active
			if (!this.IsAssigned() && this.iconGraphic != null && this.iconGraphic.gameObject.activeSelf)
			{
				// Disable the icon graphic object
				this.iconGraphic.gameObject.SetActive(false);
			}
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();

			// Instant transition
			this.EvaluateAndTransitionHoveredState(true);
			this.EvaluateAndTransitionPressedState(true);
		}
		
		protected override void OnDisable()
		{
			base.OnDisable();
			
			this.isPointerInside = false;
			this.isPointerDown = false;
			
			// Instant transition
			this.EvaluateAndTransitionHoveredState(true);
			this.EvaluateAndTransitionPressedState(true);
		}
		
#if UNITY_EDITOR
		protected override void OnValidate()
		{
			this.hoverTransitionDuration = Mathf.Max(this.hoverTransitionDuration, 0f);
			this.pressTransitionDuration = Mathf.Max(this.pressTransitionDuration, 0f);
			
			if (this.isActiveAndEnabled)
			{
				this.DoSpriteSwap(this.hoverTargetGraphic, null);
				this.DoSpriteSwap(this.pressTargetGraphic, null);
				
				if (!EditorApplication.isPlayingOrWillChangePlaymode)
				{
					// Instant transition
					this.EvaluateAndTransitionHoveredState(true);
					this.EvaluateAndTransitionPressedState(true);
				}
				else
				{
					// Regular transition
					this.EvaluateAndTransitionHoveredState(false);
					this.EvaluateAndTransitionPressedState(false);
				}
			}
		}
#endif
		
		/// <summary>
		/// Raises the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			this.isPointerInside = true;
			this.EvaluateAndTransitionHoveredState(false);
			
			// Check if tooltip is enabled
			if (this.enabled && this.IsActive() && this.m_TooltipEnabled)
			{
				// Start the tooltip delayed show coroutine
				// If delay is set at all
				if (this.m_TooltipDelay > 0f)
				{
					this.StartCoroutine("TooltipDelayedShow");
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
			this.isPointerInside = false;
			this.EvaluateAndTransitionHoveredState(false);
			this.InternalHideTooltip();
		}
		
		/// <summary>
		/// Raises the tooltip event.
		/// </summary>
		/// <param name="show">If set to <c>true</c> show.</param>
		public virtual void OnTooltip(bool show)
		{
		}
		
		/// <summary>
		/// Raises the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			this.isPointerDown = true;
			this.EvaluateAndTransitionPressedState(false);
			
			// Hide the tooltip
			this.InternalHideTooltip();
		}
		
		/// <summary>
		/// Raises the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			this.isPointerDown = false;
			this.EvaluateAndTransitionPressedState(this.m_PressTransitionInstaOut);
		}
		
		/// <summary>
		/// Raises the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData) { }
		
		/// <summary>
		/// Determines whether this slot is highlighted based on the specified eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance is highlighted the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		protected bool IsHighlighted(BaseEventData eventData)
		{
			if (!this.IsActive())
				return false;

			if (eventData is PointerEventData)
			{
				PointerEventData pointerEventData = eventData as PointerEventData;
				return ((this.isPointerDown && !this.isPointerInside && pointerEventData.pointerPress == base.gameObject) || (!this.isPointerDown && this.isPointerInside && pointerEventData.pointerPress == base.gameObject) || (!this.isPointerDown && this.isPointerInside && pointerEventData.pointerPress == null));
			}
			
			return false;
		}
		
		/// <summary>
		/// Determines whether this slot is pressed based on the specified eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance is pressed the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		protected bool IsPressed(BaseEventData eventData)
		{
			return this.IsActive() && this.isPointerInside && this.isPointerDown;
		}
		
		/// <summary>
		/// Evaluates and transitions the hovered state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void EvaluateAndTransitionHoveredState(bool instant)
		{
			if (!this.IsActive() || this.hoverTargetGraphic == null || !this.hoverTargetGraphic.gameObject.activeInHierarchy)
				return;
			
			// Determine what should the state of the hover target be
			bool highlighted = (this.m_PressForceHoverNormal ? (this.isPointerInside && !this.isPointerDown) : this.isPointerInside);
			
			// Do the transition
			switch (this.hoverTransition)
			{
				case Transition.ColorTint:
				{
					this.StartColorTween(this.hoverTargetGraphic, (highlighted ? this.hoverHighlightColor : this.hoverNormalColor), (instant ? 0f : this.hoverTransitionDuration));
					break;
				}
				case Transition.SpriteSwap:
				{
					this.DoSpriteSwap(this.hoverTargetGraphic, (highlighted ? this.hoverOverrideSprite : null));
					break;
				}
				case Transition.Animation:
				{
					this.TriggerHoverStateAnimation(highlighted ? this.hoverHighlightTrigger : this.hoverNormalTrigger);
					break;
				}
			}
		}
		
		/// <summary>
		/// Evaluates and transitions the pressed state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void EvaluateAndTransitionPressedState(bool instant)
		{
			if (!this.IsActive() || this.pressTargetGraphic == null || !this.pressTargetGraphic.gameObject.activeInHierarchy)
				return;
			
			// Do the transition
			switch (this.pressTransition)
			{
				case Transition.ColorTint:
				{
					this.StartColorTween(this.pressTargetGraphic, (this.isPointerDown ? this.pressPressColor : this.pressNormalColor), (instant ? 0f : this.pressTransitionDuration));
					break;
				}
				case Transition.SpriteSwap:
				{
					this.DoSpriteSwap(this.pressTargetGraphic, (this.isPointerDown ? this.pressOverrideSprite : null));
					break;
				}
				case Transition.Animation:
				{
					this.TriggerPressStateAnimation(this.isPointerDown ? this.pressPressTrigger : this.pressNormalTrigger);
					break;
				}
			}
			
			// If we should force normal state transition on the hover target
			if (this.m_PressForceHoverNormal)
				this.EvaluateAndTransitionHoveredState(false);
		}
		
		/// <summary>
		/// Starts a color tween.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="targetColor">Target color.</param>
		/// <param name="duration">Duration.</param>
		protected virtual void StartColorTween(Graphic target, Color targetColor, float duration)
		{
			if (target == null)
				return;
			
			target.CrossFadeColor(targetColor, duration, true, true);
		}
		
		/// <summary>
		/// Does a sprite swap.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="newSprite">New sprite.</param>
		protected virtual void DoSpriteSwap(Graphic target, Sprite newSprite)
		{
			if (target == null)
				return;
			
			Image image = target as Image;
			
			if (image == null)
				return;
			
			image.overrideSprite = newSprite;
		}
		
		/// <summary>
		/// Triggers the hover state animation.
		/// </summary>
		/// <param name="triggername">Triggername.</param>
		private void TriggerHoverStateAnimation(string triggername)
		{
			if (this.hoverTargetGraphic == null)
				return;
			
			// Get the animator on the target game object
			Animator animator = this.hoverTargetGraphic.gameObject.GetComponent<Animator>();
			
			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
				return;
			
			animator.ResetTrigger(this.hoverNormalTrigger);
			animator.ResetTrigger(this.hoverHighlightTrigger);
			animator.SetTrigger(triggername);
		}
		
		/// <summary>
		/// Triggers the pressed state animation.
		/// </summary>
		/// <param name="triggername">Triggername.</param>
		private void TriggerPressStateAnimation(string triggername)
		{
			if (this.pressTargetGraphic == null)
				return;
			
			// Get the animator on the target game object
			Animator animator = this.pressTargetGraphic.gameObject.GetComponent<Animator>();
			
			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
				return;
			
			animator.ResetTrigger(this.pressNormalTrigger);
			animator.ResetTrigger(this.pressPressTrigger);
			animator.SetTrigger(triggername);
		}
		
		/// <summary>
		/// Determines whether this slot is assigned.
		/// </summary>
		/// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
		public virtual bool IsAssigned()
		{
			return (this.GetIconSprite() != null || this.GetIconTexture() != null);
		}
		
		/// <summary>
		/// Assign the specified slot by icon sprite.
		/// </summary>
		/// <param name="icon">Icon.</param>
		public bool Assign(Sprite icon)
		{
			if (icon == null)
				return false;
			
			// Set the icon
			this.SetIcon(icon);

			return true;
		}
		
		/// <summary>
		/// Assign the specified slot by icon texture.
		/// </summary>
		/// <param name="icon">Icon.</param>
		public bool Assign(Texture icon)
		{
			if (icon == null)
				return false;
			
			// Set the icon
			this.SetIcon(icon);

			return true;
		}
		
		/// <summary>
		/// Assign the specified slot by object.
		/// </summary>
		/// <param name="source">Source.</param>
		public virtual bool Assign(Object source)
		{
			if (source is UISlotBase)
			{
				UISlotBase sourceSlot = source as UISlotBase;
				
				if (sourceSlot != null)
				{
					// Assign by sprite or texture
					if (sourceSlot.GetIconSprite() != null)
					{
						return this.Assign(sourceSlot.GetIconSprite());
					}
					else if (sourceSlot.GetIconTexture() != null)
					{
						return this.Assign(sourceSlot.GetIconTexture());
					}
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Unassign this slot.
		/// </summary>
		public virtual void Unassign()
		{
			// Remove the icon
			this.ClearIcon();
		}
		
		/// <summary>
		/// Gets the icon sprite of this slot if it's set and the icon graphic is <see cref="UnityEngine.UI.Image"/>.
		/// </summary>
		/// <returns>The icon.</returns>
		public Sprite GetIconSprite()
		{	
			// Check if the icon graphic valid image
			if (this.iconGraphic == null || !(this.iconGraphic is Image))
				return null;

			return (this.iconGraphic as Image).sprite;
		}
		
		/// <summary>
		/// Gets the icon texture of this slot if it's set and the icon graphic is <see cref="UnityEngine.UI.RawImage"/>.
		/// </summary>
		/// <returns>The icon.</returns>
		public Texture GetIconTexture()
		{
			// Check if the icon graphic valid image
			if (this.iconGraphic == null || !(this.iconGraphic is RawImage))
				return null;
				
			return (this.iconGraphic as RawImage).texture;
		}
		
		/// <summary>
		/// Gets the icon as object.
		/// </summary>
		/// <returns>The icon as object.</returns>
		public Object GetIconAsObject()
		{
			if (this.iconGraphic == null)
				return null;
			
			if (this.iconGraphic is Image)
			{
				return this.GetIconSprite();
			}
			else if (this.iconGraphic is RawImage)
			{
				return this.GetIconTexture();
			}
			
			// Default
			return null;
		}
		
		/// <summary>
		/// Sets the icon of this slot.
		/// </summary>
		/// <param name="iconSprite">The icon sprite.</param>
		public void SetIcon(Sprite iconSprite)
		{
			// Check if the icon graphic valid image
			if (this.iconGraphic == null || !(this.iconGraphic is Image))
				return;
			
			// Set the sprite
			(this.iconGraphic as Image).sprite = iconSprite;
			
			// Enable or disabled the icon graphic game object
			if (iconSprite != null && !this.iconGraphic.gameObject.activeSelf) this.iconGraphic.gameObject.SetActive(true);
			if (iconSprite == null && this.iconGraphic.gameObject.activeSelf) this.iconGraphic.gameObject.SetActive(false);
		}
		
		/// <summary>
		/// Sets the icon of this slot.
		/// </summary>
		/// <param name="iconTex">The icon texture.</param>
		public void SetIcon(Texture iconTex)
		{
			// Check if the icon graphic valid raw image
			if (this.iconGraphic == null || !(this.iconGraphic is RawImage))
				return;
			
			// Set the sprite
			(this.iconGraphic as RawImage).texture = iconTex;
			
			// Enable or disabled the icon graphic game object
			if (iconTex != null && !this.iconGraphic.gameObject.activeSelf) this.iconGraphic.gameObject.SetActive(true);
			if (iconTex == null && this.iconGraphic.gameObject.activeSelf) this.iconGraphic.gameObject.SetActive(false);
		}
		
		/// <summary>
		/// Clears the icon of this slot.
		/// </summary>
		public void ClearIcon()
		{
			// Check if the icon graphic valid
			if (this.iconGraphic == null)
				return;
			
			// In case of image
			if (this.iconGraphic is Image)
				(this.iconGraphic as Image).sprite = null;
			
			// In case of raw image
			if (this.iconGraphic is RawImage)
				(this.iconGraphic as RawImage).texture = null;
			
			// Disable the game object
			this.iconGraphic.gameObject.SetActive(false);
		}
		
		/// <summary>
		/// Raises the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!this.enabled || !this.IsAssigned() || !this.m_DragAndDropEnabled)
			{
				eventData.Reset();
				return;
			}
			
			// Check if we have a key modifier and if it's held down
			if (!this.DragKeyModifierIsDown())
			{
				eventData.Reset();
				return;
			}
			
			// Start the drag
			this.m_DragHasBegan = true;

			// Create the temporary icon for dragging
			this.CreateTemporaryIcon(eventData);
				
			// Prevent event propagation
			eventData.Use();
		}
		
		/// <summary>
		/// Is the drag key modifier down.
		/// </summary>
		/// <returns><c>true</c>, if key modifier is down, <c>false</c> otherwise.</returns>
		public virtual bool DragKeyModifierIsDown()
		{
			switch (this.m_DragKeyModifier)
			{
				case DragKeyModifier.Control:
					return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
				case DragKeyModifier.Alt:
					return (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
				case DragKeyModifier.Shift:
					return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			}
			
			// Default should be true
			return true;
		}
		
		/// <summary>
		/// Raises the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			// Check if the dragging has been started
			if (this.m_DragHasBegan)
			{
				// Update the dragged object's position
				if (this.m_CurrentDraggedObject != null)
					this.UpdateDraggedPosition(eventData);
			}
		}
		
		/// <summary>
		/// Raises the drop event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrop(PointerEventData eventData)
		{
			// Get the source slot
			UISlotBase source = (eventData.pointerPress != null) ? eventData.pointerPress.GetComponent<UISlotBase>() : null;
			
			// Make sure we have the source slot
			if (source == null || !source.IsAssigned() || !source.dragAndDropEnabled)
				return;
			
			// Notify the source that a drop was performed so it does not unassign
			source.dropPreformed = true;
			
			// Check if this slot is enabled and it's drag and drop feature is enabled
			if (!this.enabled || !this.m_DragAndDropEnabled)
				return;
			
			// Prepare a variable indicating whether the assign process was successful
			bool assignSuccess = false;
			
			// Normal empty slot assignment
			if (!this.IsAssigned())
			{
				// Assign the target slot with the info from the source
				assignSuccess = this.Assign(source);
				
				// Unassign the source on successful assignment and the source is not static
				if (assignSuccess && !source.isStatic)
					source.Unassign();
			}
			// The target slot is assigned
			else
			{
				// If the target slot is not static
				// and we have a source slot that is not static
				if (!this.isStatic && !source.isStatic)
				{
					// Check if we can swap
					if (this.CanSwapWith(source) && source.CanSwapWith(this))
					{
						// Swap the slots
						assignSuccess = source.PerformSlotSwap(this);
					}
				}
				// If the target slot is not static
				// and the source slot is a static one
				else if (!this.isStatic && source.isStatic)
				{
					assignSuccess = this.Assign(source);
				}
			}
			
			// If this slot failed to be assigned
			if (!assignSuccess)
			{
				this.OnAssignBySlotFailed(source);
			}
		}
		
		/// <summary>
		/// Raises the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			// Check if a drag was initialized at all
			if (!this.m_DragHasBegan)
				return;
			
			// Reset the drag begin bool
			this.m_DragHasBegan = false;
			
			// Destroy the dragged icon object
			if (this.m_CurrentDraggedObject != null)
			{
				Destroy(this.m_CurrentDraggedObject);
			}
			
			// Reset the variables
			this.m_CurrentDraggedObject = null;
			this.m_CurrentDraggingPlane = null;
			
			// Check if we are returning the icon to the same slot
			// By checking if the slot is highlighted
			if (this.IsHighlighted(eventData))
				return;
			
			// Check if no drop was preformed
			if (!this.m_DropPreformed)
			{
				// Try to throw away the assigned content
				this.OnThrowAway();
			}
			else
			{
				// Reset the drop preformed variable
				this.m_DropPreformed = false;
			}
		}
		
		/// <summary>
		/// Determines whether this slot can swap with the specified target slot.
		/// </summary>
		/// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
		/// <param name="target">Target.</param>
		public virtual bool CanSwapWith(Object target)
		{
			return (target is UISlotBase);
		}
		
		/// <summary>
		/// Performs a slot swap.
		/// </summary>
		/// <param name="targetObject">Target slot.</param>
		public virtual bool PerformSlotSwap(Object targetObject)
		{
			// Get the source slot
			UISlotBase targetSlot = (targetObject as UISlotBase);
			
			// Get the target slot icon
			Object targetIcon = targetSlot.GetIconAsObject();
			
			// Assign the target slot with this one
			bool assign1 = targetSlot.Assign(this);
			
			// Assign this slot by the target slot icon
			bool assign2 = this.Assign(targetIcon);
			
			// Return the status
			return (assign1 && assign2);
		}
		
		/// <summary>
		/// Called when the slot fails to assign by another slot.
		/// </summary>
		protected virtual void OnAssignBySlotFailed(Object source)
		{
			Debug.Log("UISlotBase (" + this.gameObject.name + ") failed to get assigned by (" + (source as UISlotBase).gameObject.name + ").");
		}
		
		/// <summary>
		/// This method is raised to confirm throwing away the slot.
		/// </summary>
		protected virtual void OnThrowAway()
		{
			// Check if throwing away is allowed
			if (this.m_AllowThrowAway)
			{
				// Throw away successful, unassign the slot
				this.Unassign();
			}
			else
			{
				// Throw away was denied
				this.OnThrowAwayDenied();
			}
		}
		
		/// <summary>
		/// This method is raised when the slot is denied to be thrown away and returned to it's source.
		/// </summary>
		protected virtual void OnThrowAwayDenied() { }

		/// <summary>
		/// Creates the temporary icon.
		/// </summary>
		/// <returns>The temporary icon.</returns>
		protected virtual void CreateTemporaryIcon(PointerEventData eventData)
		{
			Canvas canvas = UIUtility.FindInParents<Canvas>(this.gameObject);
			
			if (canvas == null || this.iconGraphic == null)
				return;
			
			// Create temporary panel
			GameObject iconObj = (GameObject)Instantiate((this.m_CloneTarget == null) ? this.iconGraphic.gameObject : this.m_CloneTarget);

            iconObj.transform.localScale = new Vector3(1f, 1f, 1f);
			iconObj.transform.SetParent(canvas.transform, false);
			iconObj.transform.SetAsLastSibling();
			(iconObj.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
			
			// The icon will be under the cursor.
			// We want it to be ignored by the event system.
			iconObj.AddComponent<UIIgnoreRaycast>();
			
			// Save the dragging plane
			this.m_CurrentDraggingPlane = canvas.transform as RectTransform;
			
			// Set as the current dragging object
			this.m_CurrentDraggedObject = iconObj;
			
			// Update the icon position
			this.UpdateDraggedPosition(eventData);
		}
		
		/// <summary>
		/// Updates the dragged icon position.
		/// </summary>
		/// <param name="data">Data.</param>
		private void UpdateDraggedPosition(PointerEventData data)
		{
			var rt = this.m_CurrentDraggedObject.GetComponent<RectTransform>();
			Vector3 globalMousePos;
			
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_CurrentDraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
			{
				rt.position = globalMousePos;
				rt.rotation = this.m_CurrentDraggingPlane.rotation;
			}
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
			this.StopCoroutine("TooltipDelayedShow");
			
			// Call the on tooltip only if it's currently shown
			if (this.m_IsTooltipShown)
			{
				this.m_IsTooltipShown = false;
				this.OnTooltip(false);
			}
		}
		
		protected IEnumerator TooltipDelayedShow()
		{
			yield return new WaitForSeconds(this.m_TooltipDelay);
			this.InternalShowTooltip();
		}
	}
}
