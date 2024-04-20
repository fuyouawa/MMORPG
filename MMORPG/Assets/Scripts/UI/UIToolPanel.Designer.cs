using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:0344978f-f97c-4371-a604-54380a8b5b8a
	public partial class UIToolPanel
	{
		public const string Name = "UIToolPanel";
		
		[SerializeField]
		public NotificationBoxManager NotificationBoxManager;
		[SerializeField]
		public BlackFieldManager BlackFieldManager;
		[SerializeField]
		public MessageBoxManager MessageBoxManager;
		[SerializeField]
		public SpinnerBoxManager SpinnerBoxManager;
		
		private UIToolPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			NotificationBoxManager = null;
			BlackFieldManager = null;
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
