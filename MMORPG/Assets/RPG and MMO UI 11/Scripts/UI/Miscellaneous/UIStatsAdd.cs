using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
	public class UIStatsAdd : MonoBehaviour {
		
        #pragma warning disable 0649
		[SerializeField] private Text m_ValueText;
		#pragma warning restore 0649

		public void OnButtonPress()
		{
			if (this.m_ValueText == null)
				return;
			
			this.m_ValueText.text = (int.Parse(this.m_ValueText.text) + 1).ToString();
		}
	}
}
