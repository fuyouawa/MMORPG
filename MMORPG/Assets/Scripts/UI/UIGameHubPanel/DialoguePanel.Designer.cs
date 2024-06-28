// Generate Id:b00b866c-4e5d-4b4c-83b8-6e47d80d0457
using UnityEngine;

namespace MMORPG.UI
{
	public partial class DialoguePanel : QFramework.IController
	{

		public TMPro.TextMeshProUGUI TextName;

		public TMPro.TextMeshProUGUI Content;

		public RectTransform GroupOptionBox;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
