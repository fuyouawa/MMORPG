// Generate Id:a10569bf-5a87-4a44-85b3-250830141c8b
using UnityEngine;

namespace MMORPG.UI
{
	public partial class RevivePanel : QFramework.IController
	{

		public UnityEngine.UI.Text TextReviveTime;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
