// Generate Id:cb2f0a21-e438-43ba-b766-f2dab3ec2c8e
using UnityEngine;

namespace MMORPG.Game
{
	public partial class UICharacterSelectItem : QFramework.IController
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
