// Generate Id:3a585b54-5b3a-42c8-9289-4e13cfa11205
using UnityEngine;

namespace MMORPG.UI
{
	public partial class UIMonsterCanvas : QFramework.IController
	{

		public TMPro.TextMeshProUGUI TextReviveTime;

		public RectTransform GroupHub;

		public TMPro.TextMeshProUGUI TextName;

		public TMPro.TextMeshProUGUI TextLevel;

		public MMORPG.UI.UIHpBar UIHpBar;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MMORPG.Game.GameApp.Interface;
	}
}
