// Generate Id:fe161c8f-d875-4633-ad2e-916e8e50b956
using UnityEngine;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : QFramework.IController
	{

		public UnityEngine.UI.Text TextMpPercentage;

		public DuloGames.UI.UIProgressBar ProgressHp;

		public UnityEngine.UI.Text TextHpPercentage;

		public UnityEngine.UI.Text TextLevel;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
