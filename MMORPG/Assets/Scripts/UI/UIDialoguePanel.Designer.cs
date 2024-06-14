using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:7b2eda6f-4a6c-423f-b370-e29ef0822369
	public partial class UIDialoguePanel
	{
		public const string Name = "UIDialoguePanel";
		
		
		private UIDialoguePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIDialoguePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIDialoguePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIDialoguePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
