using System;
using MMORPG.Tool;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:94699374-1a21-432e-a5aa-4a7e80ede975
	public partial class UIToolPanel
	{
		public const string Name = "UIToolPanel";
		
		[SerializeField]
		public NotificationBoxManager NotificationBoxManager;
		[SerializeField]
		public MessageBoxManager MessageBoxManager;
		[SerializeField]
		public SpinnerBoxManager SpinnerBoxManager;
		
		private UIToolPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			NotificationBoxManager = null;
			MessageBoxManager = null;
			SpinnerBoxManager = null;
			
			mData = null;
		}
		
		public UIToolPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIToolPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIToolPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
