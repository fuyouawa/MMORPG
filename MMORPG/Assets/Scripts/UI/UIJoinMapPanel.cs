using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.SceneManagement;
using System.Collections;

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

			BtnEnter.onClick.AddListener(() => JoinMapScene(1));
		}

        private void JoinMapScene(int mapId)
        {
            //TODO mapid
            var op = SceneManager.LoadSceneAsync("Space1Scene");
            op.completed += _ =>
            {
                this.GetSystem<IMapManagerSystem>().JoinedMap(mapId);
            };
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
