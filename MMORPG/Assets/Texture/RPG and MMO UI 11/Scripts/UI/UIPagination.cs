using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Pagination", 82)]
	public class UIPagination : MonoBehaviour {
		
        #pragma warning disable 0649
		[SerializeField] private Transform m_PagesContainer;
		[SerializeField] private Button m_ButtonPrev;
		[SerializeField] private Button m_ButtonNext;
		[SerializeField] private Text m_LabelText;
		[SerializeField] private Color m_LabelActiveColor = Color.white;
		#pragma warning restore 0649

		private int activePage = 0;
		
		void Start()
		{
			if (this.m_ButtonPrev != null)
				this.m_ButtonPrev.onClick.AddListener(OnPrevClick);
			
			if (this.m_ButtonNext != null)
				this.m_ButtonNext.onClick.AddListener(OnNextClick);
			
			// Detect active page
			if (this.m_PagesContainer != null && this.m_PagesContainer.childCount > 0)
			{
				for (int i = 0; i < this.m_PagesContainer.childCount; i++)
				{
					if (this.m_PagesContainer.GetChild(i).gameObject.activeSelf)
					{
						this.activePage = i;
						break;
					}
				}
			}
			
			// Prepare the pages visibility
			this.UpdatePagesVisibility();
		}
		
		private void UpdatePagesVisibility()
		{
			if (this.m_PagesContainer == null)
				return;
			
			if (this.m_PagesContainer.childCount > 0)
			{
				for (int i = 0; i < this.m_PagesContainer.childCount; i++)
					this.m_PagesContainer.GetChild(i).gameObject.SetActive((i == activePage) ? true : false);
			}
				
			// Format and update the label text
			if (this.m_LabelText != null)
			{
				this.m_LabelText.text = "<color=#" + CommonColorBuffer.ColorToString(this.m_LabelActiveColor) + ">" + (this.activePage + 1).ToString() + "</color> / " 
										+ this.m_PagesContainer.childCount.ToString();
			}
		}
		
		private void OnPrevClick()
		{
			if (!this.isActiveAndEnabled || this.m_PagesContainer == null)
				return;
			
			// If we are on the first page, jump to the last one
			if (this.activePage == 0)
				this.activePage = this.m_PagesContainer.childCount - 1;
			else
				this.activePage -= 1;
			
			// Activate
			this.UpdatePagesVisibility();
		}
		
		private void OnNextClick()
		{
			if (!this.isActiveAndEnabled || this.m_PagesContainer == null)
				return;
			
			// If we are on the last page, jump to the first one
			if (this.activePage == (this.m_PagesContainer.childCount - 1))
				this.activePage = 0;
			else
				this.activePage += 1;
			
			// Activate
			this.UpdatePagesVisibility();
		}
	}
}
