// Generate Id:380737e3-451b-4f50-89ec-7ef66495d1e2
using UnityEngine;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : QFramework.IController
	{

		public UnityEngine.UI.Image ImageMpFill;

		public UnityEngine.UI.Text TextMpPercentage;

		public UnityEngine.UI.Image ImageBgHpFill;

		public UnityEngine.UI.Image ImageHpFill;

		public UnityEngine.UI.Text TextHpPercentage;

		public UnityEngine.UI.Text TextLevel;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
