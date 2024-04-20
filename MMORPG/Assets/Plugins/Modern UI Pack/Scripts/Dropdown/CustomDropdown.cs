using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.MUIP
{
    public class CustomDropdown : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler, ISubmitHandler
    {
        // Resources
        public Animator dropdownAnimator;
        public GameObject triggerObject;
        public TextMeshProUGUI selectedText;
        public Image selectedImage;
        public Transform itemParent;
        public GameObject itemObject;
        public GameObject scrollbar;
        public VerticalLayoutGroup itemList;
        public AudioSource soundSource;
        public RectTransform listRect;
        public CanvasGroup listCG;
        public CanvasGroup contentCG;

        // Settings
        public bool isInteractable = true;
        public bool enableIcon = true;
        public bool enableTrigger = true;
        public bool enableScrollbar = true;
        public bool updateOnEnable = true;
        public bool outOnPointerExit = false;
        public bool setHighPriority = true;
        public bool invokeAtStart = false;
        public bool initAtStart = true;
        public bool enableDropdownSounds = false;
        public bool useHoverSound = true;
        public bool useClickSound = true;
        [Range(1, 50)] public int itemPaddingTop = 8;
        [Range(1, 50)] public int itemPaddingBottom = 8;
        [Range(1, 50)] public int itemPaddingLeft = 8;
        [Range(1, 50)] public int itemPaddingRight = 25;
        [Range(1, 50)] public int itemSpacing = 8;
        public int selectedItemIndex = 0;

        // Animation
        public AnimationType animationType;
        public PanelDirection panelDirection;
        [Range(25, 1000)] public float panelSize = 200;
        [Range(0.5f, 10)] public float curveSpeed = 3;
        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        // Saving
        public bool saveSelected = false;
        public string saveKey = "My Dropdown";

        // Item list
        [SerializeField]
        public List<Item> items = new List<Item>();

        // Events
        [System.Serializable] public class DropdownEvent : UnityEvent<int> { }
        public DropdownEvent onValueChanged = new DropdownEvent();
        [System.Serializable] public class ItemTextChangedEvent : UnityEvent<TMP_Text> { }
        public ItemTextChangedEvent onItemTextChanged = new ItemTextChangedEvent();

        // Audio
        public AudioClip hoverSound;
        public AudioClip clickSound;

        // Helpers
        bool isInitialized = false;
        [HideInInspector] public bool isOn;
        [HideInInspector] public int index = 0;
        [HideInInspector] public int siblingIndex = 0;
        [HideInInspector] public TextMeshProUGUI setItemText;
        [HideInInspector] public Image setItemImage;
        EventTrigger triggerEvent;
        Sprite imageHelper;
        string textHelper;

#if UNITY_EDITOR
        public bool extendEvents = false;
#endif

        public enum AnimationType { Modular, Custom }
        public enum PanelDirection { Bottom, Top }

        [System.Serializable]
        public class Item
        {
            public string itemName = "Dropdown Item";
            public Sprite itemIcon;
            [HideInInspector] public int itemIndex;
            public UnityEvent OnItemSelection = new UnityEvent();
        }

        void OnEnable()
        {
            if (isInitialized == false) { Initialize(); }
            if (updateOnEnable == true && index < items.Count) { SetDropdownIndex(selectedItemIndex); }

            listCG.alpha = 0;
            listCG.interactable = false;
            listCG.blocksRaycasts = false;
            listRect.sizeDelta = new Vector2(listRect.sizeDelta.x, 0);
        }

        void Initialize()
        {
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

            dropdownAnimator = gameObject.GetComponent<Animator>();

            if (listCG == null) { listCG = gameObject.GetComponentInChildren<CanvasGroup>(); }
            if (listRect == null) { listRect = listCG.GetComponent<RectTransform>(); }
            if (initAtStart == true && items.Count != 0) { SetupDropdown(); }
            if (animationType == AnimationType.Modular && dropdownAnimator != null) { Destroy(dropdownAnimator); }

            isInitialized = true;
        }

        public void SetupDropdown()
        {
            if (enableScrollbar == false && scrollbar != null) { Destroy(scrollbar); }
            if (itemList == null) { itemList = itemParent.GetComponent<VerticalLayoutGroup>(); }

            UpdateItemLayout();
            index = 0;

            foreach (Transform child in itemParent) { Destroy(child.gameObject); }
            for (int i = 0; i < items.Count; ++i)
            {
                GameObject go = Instantiate(itemObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(itemParent, false);
                go.name = items[i].itemName;

                setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                textHelper = items[i].itemName;
                setItemText.text = textHelper;

                onItemTextChanged?.Invoke(setItemText);

                Transform goImage = go.gameObject.transform.Find("Icon");
                setItemImage = goImage.GetComponent<Image>();

                if (items[i].itemIcon == null) { setItemImage.gameObject.SetActive(false); }
                else { imageHelper = items[i].itemIcon; setItemImage.sprite = imageHelper; }
              
                items[i].itemIndex = i;
                Item mainItem = items[i];

                Button itemButton = go.GetComponent<Button>();
                itemButton.onClick.AddListener(Animate);
                itemButton.onClick.AddListener(items[i].OnItemSelection.Invoke);
                itemButton.onClick.AddListener(delegate
                {
                    SetDropdownIndex(index = mainItem.itemIndex);
                    onValueChanged.Invoke(index = mainItem.itemIndex);
                    if (saveSelected == true) { PlayerPrefs.SetInt("Dropdown_" + saveKey, mainItem.itemIndex); }
                });
            }

            if (selectedImage != null && enableIcon == false) { selectedImage.gameObject.SetActive(false); }
            else if (selectedImage != null) { selectedImage.sprite = items[selectedItemIndex].itemIcon; }
            if (selectedText != null) { selectedText.text = items[selectedItemIndex].itemName; onItemTextChanged?.Invoke(selectedText); }
          
            if (saveSelected == true)
            {
                if (invokeAtStart == true) { items[PlayerPrefs.GetInt("Dropdown_" + saveKey)].OnItemSelection.Invoke(); }
                else { SetDropdownIndex(PlayerPrefs.GetInt("Dropdown_" + saveKey)); }
            }
            else if (invokeAtStart == true) { items[selectedItemIndex].OnItemSelection.Invoke(); }
        }

        // Obsolete
        public void ChangeDropdownInfo(int itemIndex)
        { 
            SetDropdownIndex(itemIndex); 
        }

        public void SetDropdownIndex(int itemIndex)
        {
            if (selectedImage != null && enableIcon == true && items[itemIndex].itemIcon != null) { selectedImage.gameObject.SetActive(true); selectedImage.sprite = items[itemIndex].itemIcon; }
            else if (selectedImage != null && enableIcon == true && items[itemIndex].itemIcon == null) { selectedImage.gameObject.SetActive(false); }
            if (selectedText != null) { selectedText.text = items[itemIndex].itemName; onItemTextChanged?.Invoke(selectedText); }
            if (enableDropdownSounds == true && useClickSound == true) { soundSource.PlayOneShot(clickSound); }

            selectedItemIndex = itemIndex;
        }

        public void Animate()
        {
            if (isOn == false && animationType == AnimationType.Modular)
            {
                isOn = true;
                listCG.blocksRaycasts = true;
                listCG.interactable = true;
                listCG.gameObject.SetActive(true);

                StopCoroutine("StartMinimize");
                StopCoroutine("StartExpand");
                StartCoroutine("StartExpand");
            }

            else if (isOn == true && animationType == AnimationType.Modular)
            {
                isOn = false;
                listCG.blocksRaycasts = false;
                listCG.interactable = false;

                StopCoroutine("StartMinimize");
                StopCoroutine("StartExpand");
                StartCoroutine("StartMinimize");
            }

            else if (isOn == false && animationType == AnimationType.Custom)
            {
                dropdownAnimator.Play("Stylish In");
                isOn = true;
            }

            else if (isOn == true && animationType == AnimationType.Custom)
            {
                dropdownAnimator.Play("Stylish Out");
                isOn = false;
            }

            if (enableTrigger == true && isOn == false) { triggerObject.SetActive(false); }
            else if (enableTrigger == true && isOn == true) { triggerObject.SetActive(true); }
            if (enableTrigger == true && outOnPointerExit == true) { triggerObject.SetActive(false); }
        }

        public void Interactable(bool value)
        {
            isInteractable = value;
        }

        public void CreateNewItem(string title, Sprite icon, bool notify = false)
        {
            Item item = new Item();
            item.itemName = title;
            item.itemIcon = icon;
            items.Add(item);

            if (selectedItemIndex > items.Count) { selectedItemIndex = 0; }
            if (notify == true) { SetupDropdown(); }
        }

        public void CreateNewItem(string title, bool notify = false)
        {
            Item item = new Item();
            item.itemName = title;
            items.Add(item);

            if (selectedItemIndex > items.Count) { selectedItemIndex = 0; }
            if (notify == true) { SetupDropdown(); }
        }

        public void RemoveItem(string itemTitle, bool notify = false)
        {
            var item = items.Find(x => x.itemName == itemTitle);
            items.Remove(item);

            if (selectedItemIndex > items.Count) { selectedItemIndex = 0; }
            if (notify == true) { SetupDropdown(); }
        }

        public void UpdateItemLayout()
        {
            if (itemList == null)
                return;

            itemList.spacing = itemSpacing;
            itemList.padding.top = itemPaddingTop;
            itemList.padding.bottom = itemPaddingBottom;
            itemList.padding.left = itemPaddingLeft;
            itemList.padding.right = itemPaddingRight;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (enableDropdownSounds == true && useClickSound == true) { soundSource.PlayOneShot(clickSound); }
            
            Animate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (enableDropdownSounds == true && useHoverSound == true) { soundSource.PlayOneShot(hoverSound); }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (outOnPointerExit == true && isOn == true) { Animate(); isOn = false; }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (enableDropdownSounds == true && useClickSound == true) { soundSource.PlayOneShot(clickSound); }

            Animate();
        }

        IEnumerator StartExpand()
        {
            float elapsedTime = 0;
            Vector2 startPos = listRect.sizeDelta;
            Vector2 endPos = new Vector2(listRect.sizeDelta.x, panelSize);

            while (listRect.sizeDelta.y <= panelSize - 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                listCG.alpha += Time.unscaledDeltaTime * (curveSpeed * 2);
                listRect.sizeDelta = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime * curveSpeed));
                yield return null;
            }

            listCG.alpha = 1;
            listRect.sizeDelta = endPos;
        }

        IEnumerator StartMinimize()
        {
            float elapsedTime = 0;
            Vector2 startPos = listRect.sizeDelta;
            Vector2 endPos = new Vector2(listRect.sizeDelta.x, 0);

            while (listRect.sizeDelta.y >= 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                listCG.alpha -= Time.unscaledDeltaTime * (curveSpeed * 2);
                listRect.sizeDelta = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime * curveSpeed));

                yield return null;
            }

            listCG.alpha = 0;
            listRect.sizeDelta = endPos;
            listCG.gameObject.SetActive(false);
        }
    }
}