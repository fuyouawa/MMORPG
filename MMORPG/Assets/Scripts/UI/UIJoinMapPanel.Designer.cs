using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:eeaae5f1-c051-46a3-b763-0fd6acba29e5
	public partial class UIJoinMapPanel
	{
		public const string Name = "UIJoinMapPanel";
		
		[SerializeField]
		public Michsky.MUIP.ButtonManager BtnEnter;
		
		private UIJoinMapPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnEnter = null;
			
			mData = null;
		}
		
		public UIJoinMapPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIJoinMapPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIJoinMapPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
