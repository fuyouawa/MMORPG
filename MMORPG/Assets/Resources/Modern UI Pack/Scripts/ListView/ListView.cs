using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.MUIP
{
    public class ListView : MonoBehaviour
    {
        // Resources
        public Transform itemParent;
        public GameObject itemPreset;
        public GameObject scrollbar;

        // Settings
        public bool initializeOnAwake = true;
        public bool showScrollbar = true;
        public RowCount rowCount = RowCount.Two;

        // Item list
        [SerializeField]
        public List<ListItem> listItems = new List<ListItem>();

        [System.Serializable]
        public class ListItem
        {
            public string itemTitle = "List Item";
            [HideInInspector] public ListRow row0;
            [HideInInspector] public ListRow row1;
            [HideInInspector] public ListRow row2;
#if UNITY_EDITOR
            [HideInInspector] public bool isExpanded;
#endif
        }

        [System.Serializable]
        public class ListRow
        {
            public RowType rowType = RowType.Text;
            public Sprite rowIcon;
            public string rowText = "Row text";
            public bool usePreferredWidth;
            public int preferredWidth = 50;
            [Range(0.1f, 1)] public float iconScale = 1;
        }

        public enum RowType { Icon, Text }
        public enum RowCount { One, Two, Three }

        void Awake()
        {
            if (itemParent == null) { Debug.LogError("<b>[List View]</b> 'Item Parent' is missing."); return; }
            if (initializeOnAwake == true) { InitializeItems(); }
        }

        public void InitializeItems()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false) { for (int i = itemParent.childCount; i > 0; --i) { DestroyImmediate(itemParent.GetChild(0).gameObject); } }
            else { foreach (Transform child in itemParent) { Destroy(child.gameObject); } }
#else
            foreach (Transform child in itemParent) { Destroy(child.gameObject); }
#endif

            for (int i = 0; i < listItems.Count; ++i)
            {
                GameObject go = Instantiate(itemPreset, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(itemParent, false);
                go.name = listItems[i].itemTitle;

                ListViewItem lvi = go.GetComponent<ListViewItem>();
                lvi.rowCount = rowCount;
                lvi.row0Ref = listItems[i].row0;
                lvi.row1Ref = listItems[i].row1;
                lvi.row2Ref = listItems[i].row2;
                lvi.PassReferences();
            }

            if (showScrollbar == false && scrollbar != null) { scrollbar.transform.localScale = new Vector3(0, 0, 0); }
            else if (showScrollbar == true && scrollbar != null) { scrollbar.transform.localScale = new Vector3(1, 1, 1); }
        }
    }
}