using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.MUIP
{
    public class DropdownMultiSelect : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
    {
        // Resources
        public GameObject triggerObject;
        public Transform itemParent;
        public GameObject itemObject;
        public GameObject scrollbar;
        private VerticalLayoutGroup itemList;
        private Transform currentListParent;
        public Transform listParent;
        private Animator dropdownAnimator;
        public TextMeshProUGUI setItemText;
        public CanvasGroup contentCG;

        // Settings
        public bool isInteractable = true;
        public bool initAtStart = true;
        public bool enableIcon = true;
        public bool enableTrigger = true;
        public bool enableScrollbar = true;
        public bool setHighPriority = true;
        public bool outOnPointerExit = false;
        public bool isListItem = false;
        public bool invokeAtStart = false;
        [Range(1, 50)] public int itemPaddingTop = 8;
        [Range(1, 50)] public int itemPaddingBottom = 8;
        [Range(1, 50)] public int itemPaddingLeft = 8;
        [Range(1, 50)] public int itemPaddingRight = 25;
        [Range(1, 50)] public int itemSpacing = 8;

        // Animation
        public AnimationType animationType;
        [Range(1, 25)] public float transitionSmoothness = 10;
        [Range(1, 25)] public float sizeSmoothness = 15;
        public float panelSize = 200;
        public RectTransform listRect;
        public CanvasGroup listCG;
        bool isInTransition = false;
        float closeOn;

        // Items
        [SerializeField]
        public List<Item> items = new List<Item>();

        // Other variables
        bool isInitialized = false;
        int currentIndex;
        Toggle currentToggle;
        string textHelper;
        bool isOn;
        public int siblingIndex = 0;
        EventTrigger triggerEvent;

        [System.Serializable]
        public class ToggleEvent : UnityEvent<bool> { }

        public enum AnimationType { Modular, Stylish }

        [System.Serializable]
        public class Item
        {
            public string itemName = "Dropdown Item";
            public bool isOn;
            [HideInInspector] public int itemIndex;
            [SerializeField] public ToggleEvent onValueChanged = new ToggleEvent();
        }

        void OnEnable()
        {
            if (isInitialized == false) { Initialize(); }
            if (animationType == AnimationType.Modular)
            {
                listCG.alpha = 0;
                listCG.interactable = false;
                listCG.blocksRaycasts = false;
                listRect.sizeDelta = new Vector2(listRect.sizeDelta.x, closeOn);
            }
        }

        void Initialize()
        {
            if (listCG == null) { listCG = gameObject.GetComponentInChildren<CanvasGroup>(); }
            if (listRect == null) { listRect = listCG.GetComponent<RectTransform>(); }
            if (initAtStart == true) { SetupDropdown(); }
            if (animationType == AnimationType.Modular && dropdownAnimator != null) { Destroy(dropdownAnimator); }

            if (enableTrigger == true && triggerObject != null)
            {
                // triggerButton = gameObject.GetComponent<Button>();
                triggerEvent = triggerObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => { Animate(); });
                triggerEvent.GetComponent<EventTrigger>().triggers.Add(entry);
            }

            if (setHighPriority == true)
            {
                if (contentCG == null) { contentCG = transform.Find("Content/Item List").GetComponent<CanvasGroup>(); }
                contentCG.alpha = 1;

                Canvas tempCanvas = contentCG.gameObject.AddComponent<Canvas>();
                tempCanvas.overrideSorting = true;
                tempCanvas.sortingOrder = 30000;
                contentCG.gameObject.AddComponent<GraphicRaycaster>();
            }

            currentListParent = transform.parent;
            closeOn = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            isInitialized = true;
        }

        void Update()
        {
            if (isInTransition == false)
                return;

            ProcessModularAnimation();
        }

        void ProcessModularAnimation()
        {
            if (isOn == true)
            {
                listCG.alpha += Time.unscaledDeltaTime * transitionSmoothness;
                listRect.sizeDelta = Vector2.Lerp(listRect.sizeDelta, new Vector2(listRect.sizeDelta.x, panelSize), Time.unscaledDeltaTime * sizeSmoothness);

                if (listRect.sizeDelta.y >= panelSize - 0.1f && listCG.alpha >= 1) { isInTransition = false; }
            }

            else
            {
                listCG.alpha -= Time.unscaledDeltaTime * transitionSmoothness;
                listRect.sizeDelta = Vector2.Lerp(listRect.sizeDelta, new Vector2(listRect.sizeDelta.x, closeOn), Time.unscaledDeltaTime * sizeSmoothness);

                if (listRect.sizeDelta.y <= closeOn + 0.1f && listCG.alpha <= 0) { isInTransition = false; }
            }
        }

        public void SetupDropdown()
        {
            if (dropdownAnimator == null) { dropdownAnimator = gameObject.GetComponent<Animator>(); }
            if (enableScrollbar == false && scrollbar != null) { Destroy(scrollbar); }
            if (itemList == null) { itemList = itemParent.GetComponent<VerticalLayoutGroup>(); }

            UpdateItemLayout();

            foreach (Transform child in itemParent) { Destroy(child.gameObject); }
            for (int i = 0; i < items.Count; ++i)
            {
                GameObject go = Instantiate(itemObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(itemParent, false);

                setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                textHelper = items[i].itemName;
                setItemText.text = textHelper;

                items[i].itemIndex = i;
                DropdownMultiSelect.Item mainItem = items[i];

                Toggle itemToggle = go.GetComponent<Toggle>();
                itemToggle.onValueChanged.AddListener(delegate { UpdateToggleData(mainItem.itemIndex); });
                itemToggle.onValueChanged.AddListener(UpdateToggle);
                itemToggle.onValueChanged.AddListener(items[i].onValueChanged.Invoke);

                if (items[i].isOn == true) { itemToggle.isOn = true; }
                else { itemToggle.isOn = false; }

                if (invokeAtStart == true)
                {
                    if (items[i].isOn == true) { items[i].onValueChanged.Invoke(true); }
                    else { items[i].onValueChanged.Invoke(false); }
                }
            }

            currentListParent = transform.parent;
        }

        void UpdateToggle(bool value)
        {
            if (value == true) { currentToggle.isOn = true; items[currentIndex].isOn = true; }
            else { currentToggle.isOn = false; items[currentIndex].isOn = false; }
        }

        void UpdateToggleData(int itemIndex)
        {
            currentIndex = itemIndex;
            currentToggle = itemParent.GetChild(currentIndex).GetComponent<Toggle>();
        }

        public void Animate()
        {
            if (isOn == false && animationType == AnimationType.Modular)
            {
                isOn = true;
                isInTransition = true;
                this.enabled = true;
                listCG.blocksRaycasts = true;
                listCG.interactable = true;

                if (isListItem == true)
                {
                    siblingIndex = transform.GetSiblingIndex();
                    gameObject.transform.SetParent(listParent, true);
                }
            }

            else if (isOn == true && animationType == AnimationType.Modular)
            {
                isOn = false;
                isInTransition = true;
                this.enabled = true;
                listCG.blocksRaycasts = false;
                listCG.interactable = false;

                if (isListItem == true)
                {
                    gameObject.transform.SetParent(currentListParent, true);
                    gameObject.transform.SetSiblingIndex(siblingIndex);
                }
            }

            else if (isOn == false && animationType == AnimationType.Stylish)
            {
                dropdownAnimator.Play("Stylish In");
                isOn = true;

                if (isListItem == true)
                {
                    siblingIndex = transform.GetSiblingIndex();
                    gameObject.transform.SetParent(listParent, true);
                }
            }

            else if (isOn == true && animationType == AnimationType.Stylish)
            {
                dropdownAnimator.Play("Stylish Out");
                isOn = false;

                if (isListItem == true)
                {
                    gameObject.transform.SetParent(currentListParent, true);
                    gameObject.transform.SetSiblingIndex(siblingIndex);
                }
            }

            if (enableTrigger == true && isOn == false) { triggerObject.SetActive(false); }
            else if (enableTrigger == true && isOn == true) { triggerObject.SetActive(true); }

            if (enableTrigger == true && outOnPointerExit == true) { triggerObject.SetActive(false); }
        }

        public void CreateNewItem(string title, bool value, bool notify)
        {
            Item item = new Item();
            item.itemName = title;
            item.isOn = value;
            items.Add(item);
            if (notify == true) { SetupDropdown(); }
        }

        public void CreateNewItem(string title, bool value)
        {
            Item item = new Item();
            item.itemName = title;
            item.isOn = value;
            items.Add(item);
            SetupDropdown();     
        }

        public void CreateNewItem(string title)
        {
            Item item = new Item();
            item.itemName = title;
            items.Add(item);
        }

        public void RemoveItem(string itemTitle)
        {
            var item = items.Find(x => x.itemName == itemTitle);
            items.Remove(item);
            SetupDropdown();
        }

        public void UpdateItemLayout()
        {
            if (itemList != null)
            {
                itemList.spacing = itemSpacing;
                itemList.padding.top = itemPaddingTop;
                itemList.padding.bottom = itemPaddingBottom;
                itemList.padding.left = itemPaddingLeft;
                itemList.padding.right = itemPaddingRight;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            Animate();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (outOnPointerExit == true && isOn == true)
            {
                Animate();
                isOn = false;

                if (isListItem == true) { gameObject.transform.SetParent(currentListParent, true); }
            }
        }
    }
}