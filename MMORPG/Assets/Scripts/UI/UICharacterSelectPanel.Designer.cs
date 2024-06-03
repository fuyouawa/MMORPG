using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:934b51c0-e786-496f-ba71-ea1cf79665c1
	public partial class UICharacterSelectPanel
	{
		public const string Name = "UICharacterSelectPanel";
		
		[SerializeField]
		public RectTransform GroupCharacters;
		[SerializeField]
		public UnityEngine.UI.Button BtnCreateCharacter;
		[SerializeField]
		public UnityEngine.UI.Button BtnPlay;
		
		private UICharacterSelectPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			GroupCharacters = null;
			BtnCreateCharacter = null;
			BtnPlay = null;
			
			mData = null;
		}
		
		public UICharacterSelectPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UICharacterSelectPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UICharacterSelectPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
