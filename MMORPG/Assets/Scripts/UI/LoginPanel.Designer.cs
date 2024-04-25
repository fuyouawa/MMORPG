using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:3e1b443b-68b7-494c-8ea8-1bc7a842c81d
	public partial class LoginPanel
	{
		public const string Name = "LoginPanel";
		
		[SerializeField]
		public UnityEngine.UI.InputField InputUsername;
		[SerializeField]
		public UnityEngine.UI.InputField InputPassword;
		[SerializeField]
		public UnityEngine.UI.Button BtnLogin;
		
		private LoginPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			InputUsername = null;
			InputPassword = null;
			BtnLogin = null;
			
			mData = null;
		}
		
		public LoginPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		LoginPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new LoginPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
