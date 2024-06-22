// Generate Id:410d8231-a7fa-4324-8aaa-01268b1ef98e
using UnityEngine;

namespace MMORPG.UI
{
	public partial class UIMonsterCanvas : QFramework.IController
	{

		public TMPro.TextMeshProUGUI TextName;

		public TMPro.TextMeshProUGUI TextLevel;

		public MMORPG.UI.UIHpBar UIHpBar;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
