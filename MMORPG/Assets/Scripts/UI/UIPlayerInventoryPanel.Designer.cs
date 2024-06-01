using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:154d787b-07bb-4bd5-adfd-941a0171f7c4
	public partial class UIPlayerInventoryPanel
	{
		public const string Name = "UIPlayerInventoryPanel";
		
		
		private UIPlayerInventoryPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIPlayerInventoryPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIPlayerInventoryPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIPlayerInventoryPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
