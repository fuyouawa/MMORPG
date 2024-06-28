// Generate Id:7a33944b-c6dc-4307-a831-98aed5893cdb
using UnityEngine;

namespace MMORPG.UI
{
	public partial class ChatPanel : QFramework.IController
	{

		public UnityEngine.UI.ToggleGroup GroupTabsMenu;

		public MMORPG.UI.UIChatTabContent TabContentCompositeChat;

		public MMORPG.UI.UIChatTabContent TabContentWorldChat;

		public MMORPG.UI.UIChatTabContent TabContentMapChat;

		public MMORPG.UI.UIChatTabContent TabContentGroupChat;

		public TMPro.TextMeshProUGUI TextChannel;

		public TMPro.TMP_InputField InputMessage;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
