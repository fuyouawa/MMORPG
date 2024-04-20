using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:0e742eee-2909-4fef-bf6d-b01de2e1bb09
	public partial class UIRegisterPanel
	{
		public const string Name = "UIRegisterPanel";
		
		[SerializeField]
		public Michsky.MUIP.CustomInputField InputUsername;
		[SerializeField]
		public Michsky.MUIP.CustomInputField InputPassword;
		[SerializeField]
		public Michsky.MUIP.CustomInputField InputVerifyPassword;
		[SerializeField]
		public Michsky.MUIP.ButtonManager BtnConfirm;
		[SerializeField]
		public Michsky.MUIP.ButtonManager BtnCancel;
		
		private UIRegisterPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			InputUsername = null;
			InputPassword = null;
			InputVerifyPassword = null;
			BtnConfirm = null;
			BtnCancel = null;
			
			mData = null;
		}
		
		public UIRegisterPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIRegisterPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIRegisterPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
