using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:895d2293-2906-45b3-8365-747506fcdf4c
	public partial class UILoginPanel
	{
		public const string Name = "UILoginPanel";
		
		[SerializeField]
		public Michsky.MUIP.CustomInputField InputUsername;
		[SerializeField]
		public Michsky.MUIP.CustomInputField InputPassword;
		[SerializeField]
		public Michsky.MUIP.ButtonManager BtnConfirm;
		[SerializeField]
		public Michsky.MUIP.ButtonManager BtnCancel;
		
		private UILoginPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			InputUsername = null;
			InputPassword = null;
			BtnConfirm = null;
			BtnCancel = null;
			
			mData = null;
		}
		
		public UILoginPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILoginPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILoginPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
