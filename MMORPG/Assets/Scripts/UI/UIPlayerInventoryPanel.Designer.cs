using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:bab79817-b697-4c7d-b39e-4e89f96d0529
	public partial class UIPlayerInventoryPanel
	{
		public const string Name = "UIPlayerInventoryPanel";
		
		[SerializeField]
		public UnityEngine.RectTransform SlotsGrid;
		
		private UIPlayerInventoryPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SlotsGrid = null;
			
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
