// Generate Id:1be016d1-f72c-495d-9099-d5db4c89c361
using UnityEngine;

namespace MMORPG.Game
{
	public partial class CharacterSelectItem : QFramework.IController
	{

		public UnityEngine.UI.Image ImageAvatar;

		public TMPro.TextMeshProUGUI TextName;

		public TMPro.TextMeshProUGUI TextHp;

		public TMPro.TextMeshProUGUI TextMp;

		public TMPro.TextMeshProUGUI TextLevel;

		public TMPro.TextMeshProUGUI TextRace;

		public TMPro.TextMeshProUGUI TextClass;

		public UnityEngine.UI.Button BtnDelete;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
