// Generate Id:67369d47-58d4-4382-976a-ec189419dc3d
using UnityEngine;

namespace MMORPG.UI
{
	public partial class RevivePanel : QFramework.IController
	{

		public TMPro.TextMeshProUGUI TextReveiveTime;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
