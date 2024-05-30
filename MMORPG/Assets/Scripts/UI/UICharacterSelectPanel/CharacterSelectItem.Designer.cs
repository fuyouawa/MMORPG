// Generate Id:abf65059-c29e-4cdc-b119-ea45b8258880
using UnityEngine;

namespace MMORPG.Game
{
	public partial class CharacterSelectItem : QFramework.IController
	{

		public UnityEngine.UI.Image ImageAvatar;

		public TMPro.TextMeshProUGUI TextName;

		public TMPro.TextMeshProUGUI TextLevel;

		public TMPro.TextMeshProUGUI TextRace;

		public TMPro.TextMeshProUGUI TextClass;

		public UnityEngine.UI.Button BtnDelete;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
