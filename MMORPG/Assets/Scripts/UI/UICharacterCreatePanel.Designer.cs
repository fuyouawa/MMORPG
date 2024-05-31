using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:5e176908-c65d-408b-bb3b-6d99086959a4
	public partial class UICharacterCreatePanel
	{
		public const string Name = "UICharacterCreatePanel";
		
		[SerializeField]
		public TMPro.TMP_InputField InputCharacterName;
		
		private UICharacterCreatePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			InputCharacterName = null;
			
			mData = null;
		}
		
		public UICharacterCreatePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UICharacterCreatePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UICharacterCreatePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
