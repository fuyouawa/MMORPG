// Generate Id:8a5801af-ce88-4db0-8ed9-02266b16c1da
using UnityEngine;

namespace MMORPG.UI
{
	public partial class UIChatItem : QFramework.IController
	{

		public TMPro.TextMeshProUGUI TextMessage;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
