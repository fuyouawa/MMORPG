
using Common.Inventory;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector3 _offset;
        private Transform _initialParent;
        private Vector3 _initialPosition;
        private bool _isDragging;
        private UIKnapsackSlot _originSlot;
        private ResLoader _resLoader = ResLoader.Allocate();

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
            ImageIcon.sprite = _resLoader.LoadSync<Sprite>(item.Icon);
            //TODO 其他可能的处理
            Item = item;
        }

        /// <summary>
        /// 将此槽置空
        /// </summary>
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
            _offset = transform.position - Input.mousePosition;
            _initialParent = transform.parent;
            _initialPosition = transform.position;

            transform.SetParent(transform.root);
            _isDragging = true;

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
            UIKnapsackSlot targetSlot = null;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (targetSlot != null)
                {
                    transform.SetParent(targetSlot.transform);
                    transform.position = targetSlot.transform.position;
                }
                else
                {
                    transform.SetParent(_initialParent);
                    transform.position = _initialPosition;
                }
            }
        }

    }
}
