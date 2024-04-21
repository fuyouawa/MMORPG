using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:2847c972-1bd1-4bff-af1c-cae92fce44c2
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
