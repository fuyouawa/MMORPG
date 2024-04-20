using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:679821bb-dff4-49f8-8c92-a4dd126de386
	public partial class UIEnterOperationPanel
	{
		public const string Name = "UIEnterOperationPanel";
		
		[SerializeField]
		public Michsky.MUIP.ButtonManager BtnEnter;
		
		private UIEnterOperationPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnEnter = null;
			
			mData = null;
		}
		
		public UIEnterOperationPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIEnterOperationPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIEnterOperationPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
