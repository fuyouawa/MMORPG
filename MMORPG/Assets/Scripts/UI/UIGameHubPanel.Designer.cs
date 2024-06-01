using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:2a8c5eb0-ecd8-4ac5-bcec-5728d2322259
	public partial class UIGameHubPanel
	{
		public const string Name = "UIGameHubPanel";
		
		
		private UIGameHubPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIGameHubPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGameHubPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGameHubPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
