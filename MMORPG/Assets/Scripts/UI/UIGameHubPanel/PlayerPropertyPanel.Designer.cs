// Generate Id:e5ba10a8-3f00-48f8-b0cc-a3491f0a99c9
using UnityEngine;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : QFramework.IController
	{

		public UnityEngine.UI.Text TextMpPercentage;

		public UnityEngine.UI.Image ImageBgHpFill;

		public UnityEngine.UI.Image ImageHpFill;

		public UnityEngine.UI.Text TextHpPercentage;

		public UnityEngine.UI.Text TextLevel;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
