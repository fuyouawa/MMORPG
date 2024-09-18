// Generate Id:cbe7a309-8d2a-4e01-a52b-e0711c0ae705
using UnityEngine;

namespace MMORPG.Game
{
	public partial class UIPlayerCanvas : QFramework.IController
	{

		public RectTransform GroupHub;

		public TMPro.TextMeshProUGUI TextName;

		public TMPro.TextMeshProUGUI TextLevel;

		public MMORPG.UI.UIHpBar UIHpBar;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
