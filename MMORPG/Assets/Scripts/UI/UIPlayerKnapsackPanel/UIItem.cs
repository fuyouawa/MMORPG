
using Common.Inventory;
using DuloGames.UI;
using MMORPG.Command;
using MMORPG.Game;
using MMORPG.Global;
using MMORPG.Tool;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIItem : MonoBehaviour, IController,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector3 _offset;
        private Transform _initialParent;
        private Vector3 _initialPosition;
        private bool _isDragging;
        private UIKnapsackSlot _originSlot;

        public TextMeshProUGUI TextAmount;
        public Image ImageIcon;


        /// <summary>
        /// Item信息
        /// </summary>
        public Item Item { get; protected set; }

        private int _amount;

        /// <summary>
        /// 物品数量
        /// </summary>
        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                // 显示到UI上
                if (TextAmount != null)
                {
                    TextAmount.SetText(value == 0 ? string.Empty : value.ToString());
                }

                // 同步更新Item中的Amount
                if (Item != null)
                {
                    Item.Amount = value;
                }
            }
        }

        private void Start()
        {
            TextAmount.raycastTarget = false;
        }

        public void Assign(Item item)
        {
            Amount = item.Amount;
            // 根据Item中的Icon显示物品图标
            ImageIcon.enabled = true;
            ImageIcon.sprite = Resources.Load<Texture2D>($"{Config.ItemIconPath}/{item.Icon}").ToSprite();
            //TODO 其他可能的处理
            Item = item;
        }

        public void Clear()
        {
            // 隐藏物品图标
            ImageIcon.enabled = false;
            ImageIcon.sprite = null;
            Amount = 0;
            //TODO 其他可能的处理
            Item = null;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            _originSlot = transform.parent.GetComponent<UIKnapsackSlot>();
            _offset = transform.position - Input.mousePosition;
            _initialParent = transform.parent;
            _initialPosition = transform.position;

            transform.SetParent(transform.root);
            //_isDragging = true;

            // 隐藏物品的RaycastTarget，避免干扰鼠标事件
            ImageIcon.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 更新物品位置
            transform.position = Input.mousePosition + _offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ImageIcon.raycastTarget = true;

            Destroy(gameObject);

            if (_originSlot == null) return;
            
            int originSlotId = _originSlot.SlotId;
            int targetSlotId = -1;
            
            UIKnapsackSlot targetSlot = eventData.pointerEnter?.gameObject.GetComponent<UIKnapsackSlot>();
            if (targetSlot == null)
            {
                targetSlot = eventData.pointerEnter?.GetComponentInParent<UIKnapsackSlot>();
            }
            if (targetSlot != null && EventSystem.current.IsPointerOverGameObject())
            {
                //transform.SetParent(_initialParent);
                //transform.position = _initialPosition;
                //return;

                targetSlotId = targetSlot.SlotId;
            }

            //if (targetSlot.UIItem == null)
            //{
            //transform.SetParent(targetSlot.transform);
            //transform.position = targetSlot.transform.position;
            //targetSlot.UIItem = this;
            //_originSlot.UIItem = null;
            //}
            //else
            //{
            //transform.SetParent(targetSlot.transform);
            //transform.position = targetSlot.transform.position;

            //targetSlot.UIItem.transform.SetParent(_originSlot.transform);
            //targetSlot.UIItem.transform.position = _originSlot.transform.position;

            //(_originSlot.UIItem, targetSlot.UIItem) = (targetSlot.UIItem, _originSlot.UIItem);
            //}

            this.SendCommand(new PlacementItemCommand(originSlotId, targetSlotId));
            
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
