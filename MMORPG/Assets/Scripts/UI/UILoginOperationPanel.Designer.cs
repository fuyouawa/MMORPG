using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:6ffcb4a3-e07a-4af6-9390-47093605d1fe
	public partial class UILoginOperationPanel
	{
		public const string Name = "UILoginOperationPanel";
		
		[SerializeField]
		public Michsky.MUIP.ButtonManager BtnLogin;
		[SerializeField]
		public Michsky.MUIP.ButtonManager BtnRegister;
		
		private UILoginOperationPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnLogin = null;
			BtnRegister = null;
			
			mData = null;
		}
		
		public UILoginOperationPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILoginOperationPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILoginOperationPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
