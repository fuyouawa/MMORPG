using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:90cac016-9416-406b-ad08-43ca172e4112
	public partial class UIChatPanel
	{
		public const string Name = "UIChatPanel";
		
		[SerializeField]
		public UnityEngine.UI.ToggleGroup GroupTabsMenu;
		[SerializeField]
		public MMORPG.UI.UIChatTabContent TabContentCompositeChat;
		[SerializeField]
		public MMORPG.UI.UIChatTabContent TabContentWorldChat;
		[SerializeField]
		public MMORPG.UI.UIChatTabContent TabContentMapChat;
		[SerializeField]
		public MMORPG.UI.UIChatTabContent TabContentGroupChat;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextChannel;
		[SerializeField]
		public TMPro.TMP_InputField InputMessage;
		
		private UIChatPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			GroupTabsMenu = null;
			TabContentCompositeChat = null;
			TabContentWorldChat = null;
			TabContentMapChat = null;
			TabContentGroupChat = null;
			TextChannel = null;
			InputMessage = null;
			
			mData = null;
		}
		
		public UIChatPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIChatPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIChatPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
