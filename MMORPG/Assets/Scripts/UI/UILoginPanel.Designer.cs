using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:55acbcbd-a31c-4c68-8b93-4e645f55404c
	public partial class UILoginPanel
	{
		public const string Name = "UILoginPanel";
		
		[SerializeField]
		public UnityEngine.UI.InputField InputUsername;
		[SerializeField]
		public UnityEngine.UI.InputField InputPassword;
		[SerializeField]
		public UnityEngine.UI.Button BtnLogin;
		
		private UILoginPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			InputUsername = null;
			InputPassword = null;
			BtnLogin = null;
			
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
