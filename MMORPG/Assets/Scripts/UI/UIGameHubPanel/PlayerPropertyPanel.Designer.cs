// Generate Id:f523efa3-a18e-4bf6-9774-804b437e5fc5
using UnityEngine;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : QFramework.IController
	{

		public UnityEngine.UI.Text TextMpPercentage;

		public DuloGames.UI.UIProgressBar ProgressBgHp;

		public DuloGames.UI.UIProgressBar ProgressHp;

		public UnityEngine.UI.Text TextHpPercentage;

		public UnityEngine.UI.Text TextLevel;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
