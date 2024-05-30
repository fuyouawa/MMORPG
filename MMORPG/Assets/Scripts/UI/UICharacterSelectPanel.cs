using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Proto.Character;
using MMORPG.Command;
using MMORPG.Game;
using MMORPG.System;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.UI
{
	public class UICharacterSelectPanelData : UIPanelData
	{
	}

	public partial class UICharacterSelectPanel : UIPanel, IController
    {
        public CharacterSelectItem CharacterSelectItemPrefab;
        public CharacterSelectItem CurrentSelectItem { get; private set; }

        private INetworkSystem _network;

        private void Awake()
        {
            _network = this.GetSystem<INetworkSystem>();

            _network.ReceiveInUnityThread<CharacterListResponse>(OnReceivedCharacterList)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnReceivedCharacterList(CharacterListResponse response)
        {
            foreach (var character in response.CharacterList)
            {
                var item = Instantiate(CharacterSelectItemPrefab, CharactersGroup, false);
                item.OnSelected += OnCharacterItemSelected;
                item.SetCharacter(character);

                if (CurrentSelectItem == null)
                {
                    item.Toggle.isOn = true;
                    CurrentSelectItem = item;
                }
                else
                {
                    item.Toggle.isOn = false;
                }
            }
        }

        private void OnCharacterItemSelected(CharacterSelectItem sender)
        {
            if (CurrentSelectItem != null)
            {
                CurrentSelectItem.Toggle.isOn = false;
            }
            CurrentSelectItem = sender;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        public CharacterSelectItem[] GetCharacterItems()
        {
            return CharactersGroup.GetComponentsInChildren<CharacterSelectItem>();
        }

        public void ClearCharacterItems()
        {
            foreach (var item in GetCharacterItems())
            {
                Destroy(item.gameObject);
            }
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UICharacterSelectPanelData ?? new UICharacterSelectPanelData();
            // please add init code here

            BtnPlay.onClick.AddListener(() => JoinMapScene(1));
        }
		
		protected override void OnOpen(IUIData uiData = null)
        {
            ClearCharacterItems();
            _network.SendToServer(new CharacterListRequest());
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


        private void JoinMapScene(int mapId)
        {
            var op = SceneManager.LoadSceneAsync("Space1Scene");
            op.completed += _ =>
            {
                this.SendCommand(new JoinMapCommand(mapId));
            };
        }

    }
}
