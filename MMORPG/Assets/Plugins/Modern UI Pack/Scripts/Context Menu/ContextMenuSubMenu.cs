using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.MUIP
{
    public class ContextMenuSubMenu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public ContextMenuManager cmManager;
        public ContextMenuContent cmContent;
        public Animator subMenuAnimator;
        public Transform itemParent;
        public GameObject trigger;
        [HideInInspector] public int subMenuIndex;

        GameObject selectedItem;
        Image setItemImage;
        TextMeshProUGUI setItemText;
        Sprite imageHelper;
        string textHelper;
        RectTransform listParent;

       [HideInInspector] public bool enableFadeOut = true;

        void OnEnable()
        {
            if (itemParent == null) { Debug.Log("<b>[Context Menu]</b> Item Parent is missing.", this); return; }

            listParent = itemParent.parent.gameObject.GetComponent<RectTransform>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (cmManager.subMenuBehaviour == ContextMenuManager.SubMenuBehaviour.Click)
            {
                if (subMenuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Menu In")) 
                { 
                    subMenuAnimator.Play("Menu Out");
                    if (trigger != null) { trigger.SetActive(false); }
                }

                else 
                { 
                    subMenuAnimator.Play("Menu In"); 
                    if (trigger != null) { trigger.SetActive(true); } 
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (Transform child in itemParent)
                Destroy(child.gameObject);

            for (int i = 0; i < cmContent.contexItems[subMenuIndex].subMenuItems.Count; ++i)
            {
                bool nulLVariable = false;

                if (cmContent.contexItems[subMenuIndex].subMenuItems[i].contextItemType == ContextMenuContent.ContextItemType.Button && cmManager.contextButton != null)
                    selectedItem = cmManager.contextButton;
                else if (cmContent.contexItems[subMenuIndex].subMenuItems[i].contextItemType == ContextMenuContent.ContextItemType.Separator && cmManager.contextSeparator != null)
                    selectedItem = cmManager.contextSeparator;
                else
                {
                    Debug.LogError("<b>[Context Menu]</b> At least one of the item presets is missing. " +
                        "You can assign a new variable in Resources (Context Menu) tab. All default presets can be found in " +
                        "<b>Modern UI Pack > Prefabs > Context Menu</b> folder.", this);
                    nulLVariable = true;
                }

                if (nulLVariable == false)
                {
                    GameObject go = Instantiate(selectedItem, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    go.transform.SetParent(itemParent, false);

                    if (cmContent.contexItems[subMenuIndex].subMenuItems[i].contextItemType == ContextMenuContent.ContextItemType.Button)
                    {
                        setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                        textHelper = cmContent.contexItems[subMenuIndex].subMenuItems[i].itemText;
                        setItemText.text = textHelper;

                        Transform goImage = go.gameObject.transform.Find("Icon");
                        setItemImage = goImage.GetComponent<Image>();
                        imageHelper = cmContent.contexItems[subMenuIndex].subMenuItems[i].itemIcon;
                        setItemImage.sprite = imageHelper;

                        if (imageHelper == null)
                            setItemImage.color = new Color(0, 0, 0, 0);

                        Button itemButton = go.GetComponent<Button>();
                        itemButton.onClick.AddListener(cmContent.contexItems[subMenuIndex].subMenuItems[i].onClick.Invoke);
                        itemButton.onClick.AddListener(CloseOnClick);
                        StartCoroutine(ExecuteAfterTime(0.01f));
                    }
                }
            }

            if (cmManager.autoSubMenuPosition == true)
            {
                if (cmManager.horizontalBound == ContextMenuManager.CursorBoundHorizontal.Left) { listParent.pivot = new Vector2(0f, listParent.pivot.y); }
                else if (cmManager.horizontalBound == ContextMenuManager.CursorBoundHorizontal.Right) { listParent.pivot = new Vector2(1f, listParent.pivot.y); }
               
                if (cmManager.verticalBound == ContextMenuManager.CursorBoundVertical.Top) { listParent.pivot = new Vector2(listParent.pivot.x, 0f); }
                else if (cmManager.verticalBound == ContextMenuManager.CursorBoundVertical.Bottom) { listParent.pivot = new Vector2(listParent.pivot.x, 1f); }
            }

            if (cmManager.subMenuBehaviour == ContextMenuManager.SubMenuBehaviour.Hover)
                subMenuAnimator.Play("Menu In");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if !UNITY_2022_1_OR_NEWER
            if (cmManager.subMenuBehaviour == ContextMenuManager.SubMenuBehaviour.Hover && !subMenuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Start"))
                subMenuAnimator.Play("Menu Out");
#endif
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            itemParent.gameObject.SetActive(false);
            itemParent.gameObject.SetActive(true);
            StopCoroutine(ExecuteAfterTime(0.01f));
            StopCoroutine("ExecuteAfterTime");
        }

        public void CloseOnClick()
        {
            cmManager.contextAnimator.Play("Menu Out");
            cmManager.isOn = false;
            trigger.SetActive(false);
        }
    }
}