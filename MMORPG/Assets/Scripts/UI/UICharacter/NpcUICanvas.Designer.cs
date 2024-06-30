// Generate Id:9e2ad5d8-d4da-48de-a8e5-cc15c63aeb67
using UnityEngine;

namespace MMORPG.UI
{
	public partial class NpcUICanvas : QFramework.IController
	{

		public TMPro.TextMeshProUGUI TextName;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
