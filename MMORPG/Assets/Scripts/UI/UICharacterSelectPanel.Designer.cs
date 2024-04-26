using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:000a6d54-a0e2-4c2b-b32a-7c973c7dbe37
	public partial class UICharacterSelectPanel
	{
		public const string Name = "UICharacterSelectPanel";
		
		
		private UICharacterSelectPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
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
