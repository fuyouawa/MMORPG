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
using Google.Protobuf.WellKnownTypes;

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

        private void OnReceivedCharacterList(CharacterListResponse response)
        {
            foreach (var character in response.CharacterList)
            {
                var item = Instantiate(CharacterSelectItemPrefab, CharactersGroup, false);
                item.OnSelectionChanged += OnCharacterItemSelectionChanged;
                item.SetCharacter(character);

                if (CurrentSelectItem == null)
                {
                    item.Toggle.SetIsOnWithoutNotify(true);
                    CurrentSelectItem = item;
                }
                else
                {
                    item.Toggle.SetIsOnWithoutNotify(false);
                }
            }
        }

        private void OnCharacterItemSelectionChanged(CharacterSelectItem sender, bool toggle)
        {
            if (CurrentSelectItem != null)
            {
                CurrentSelectItem.Toggle.SetIsOnWithoutNotify(CurrentSelectItem == sender);
            }

            if (toggle)
            {
                CurrentSelectItem = sender;
            }
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

            _network = this.GetSystem<INetworkSystem>();

            _network.Receive<CharacterListResponse>(OnReceivedCharacterList)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            BtnPlay.onClick.AddListener(OnPlayGame);
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

        public void OnPlayGame()
        {
            if (CurrentSelectItem != null)
            {
                this.SendCommand(new JoinMapCommand(CurrentSelectItem.MapId, CurrentSelectItem.CharacterId));
            }
        }

        public void OnCreateCharacter()
        {
            SceneManager.LoadScene("CharacterCreateScene");
        }
    }
}
