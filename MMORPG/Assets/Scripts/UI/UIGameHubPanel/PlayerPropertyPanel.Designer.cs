// Generate Id:689814bb-bdd5-49a1-8fbe-2ee7a604a770
using UnityEngine;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : QFramework.IController
	{

		public UnityEngine.UI.Text TextMpPercentage;

		public UnityEngine.UI.Text TextHpPercentage;

		public UnityEngine.UI.Text TextLevel;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
