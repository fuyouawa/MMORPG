using UnityEngine;

namespace Michsky.MUIP
{
    public class ListViewItem : MonoBehaviour
    {
        [Header("Settings")]
        public ListViewRow row0;
        public ListViewRow row1;
        public ListViewRow row2;

        [Header("References")]
        public ListView.RowCount rowCount;
        public ListView.ListRow row0Ref;
        public ListView.ListRow row1Ref;
        public ListView.ListRow row2Ref;

        public void PassReferences()
        {
            if (rowCount == ListView.RowCount.One) { row0.gameObject.SetActive(true); row1.gameObject.SetActive(false); row2.gameObject.SetActive(false); }
            else if (rowCount == ListView.RowCount.Two) { row0.gameObject.SetActive(true); row1.gameObject.SetActive(true); row2.gameObject.SetActive(false); }
            else if (rowCount == ListView.RowCount.Three) { row0.gameObject.SetActive(true); row1.gameObject.SetActive(true); row2.gameObject.SetActive(true); }

            // Row 1
            if (row0Ref.rowType == ListView.RowType.Icon)
            { 
                row0.iconImage.sprite = row0Ref.rowIcon; 
                row0.iconImage.gameObject.SetActive(true); 
                row0.textObject.gameObject.SetActive(false);
                row0.iconImage.transform.localScale = new Vector3(row0Ref.iconScale, row0Ref.iconScale, row0Ref.iconScale);
            }

            else if (row0Ref.rowType == ListView.RowType.Text) 
            {
                row0.textObject.text = row0Ref.rowText;
                row0.iconImage.gameObject.SetActive(false); 
                row0.textObject.gameObject.SetActive(true); 
            }

            if (row0Ref.usePreferredWidth == true) { row0.layoutElement.preferredWidth = row0Ref.preferredWidth; }
            else { row0.layoutElement.preferredWidth = -1; }

            // Row 2
            if (row1Ref == null)
                return;

            if (row1Ref.rowType == ListView.RowType.Icon) 
            {
                row1.iconImage.sprite = row1Ref.rowIcon; 
                row1.iconImage.gameObject.SetActive(true);
                row1.textObject.gameObject.SetActive(false);
                row1.iconImage.transform.localScale = new Vector3(row1Ref.iconScale, row1Ref.iconScale, row1Ref.iconScale);
            }

            else if (row1Ref.rowType == ListView.RowType.Text) 
            {
                row1.textObject.text = row1Ref.rowText; 
                row1.iconImage.gameObject.SetActive(false); 
                row1.textObject.gameObject.SetActive(true);
            }

            if (row1Ref.usePreferredWidth == true) { row1.layoutElement.preferredWidth = row1Ref.preferredWidth; }
            else { row1.layoutElement.preferredWidth = -1; }

            // Row 3
            if (row2Ref == null)
                return;

            if (row2Ref.rowType == ListView.RowType.Icon) 
            { 
                row2.iconImage.sprite = row2Ref.rowIcon; 
                row2.iconImage.gameObject.SetActive(true);
                row2.textObject.gameObject.SetActive(false);
                row2.iconImage.transform.localScale = new Vector3(row2Ref.iconScale, row2Ref.iconScale, row2Ref.iconScale);
            }

            else if (row2Ref.rowType == ListView.RowType.Text)
            { 
                row2.textObject.text = row2Ref.rowText; 
                row2.iconImage.gameObject.SetActive(false); 
                row2.textObject.gameObject.SetActive(true); 
            }

            if (row2Ref.usePreferredWidth == true) { row2.layoutElement.preferredWidth = row2Ref.preferredWidth; }
            else { row2.layoutElement.preferredWidth = -1; }
        }
    }
}