using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:b702a2c7-490f-49e2-850a-72c292e2e89d
	public partial class UICharacterSelectPanel
	{
		public const string Name = "UICharacterSelectPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnPlay;
		
		private UICharacterSelectPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
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
