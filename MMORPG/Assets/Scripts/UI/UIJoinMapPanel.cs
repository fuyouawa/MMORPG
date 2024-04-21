using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.SceneManagement;

namespace MMORPG.UI
{
	public class UIJoinMapPanelData : UIPanelData
	{
	}
	public partial class UIJoinMapPanel : UIPanel, IController
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIJoinMapPanelData ?? new UIJoinMapPanelData();
			// please add init code here

			BtnEnter.onClick.AddListener(async () =>
			{
				await SceneManager.LoadSceneAsync("Space1Scene");
				this.GetSystem<IMapManagerSystem>().JoinedMap(1);
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

        public IArchitecture GetArchitecture()
        {
			return GameApp.Interface;
        }
    }
}
