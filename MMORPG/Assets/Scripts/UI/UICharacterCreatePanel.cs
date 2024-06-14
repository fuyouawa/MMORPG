using MMORPG.Common.Proto.Base;
using MMORPG.Common.Proto.Character;
using MMORPG.Common.Tool;
using MMORPG.Command;
using MMORPG.Game;
using MMORPG.System;
using QFramework;
using Serilog;

namespace MMORPG.UI
{
	public class UICharacterCreatePanelData : UIPanelData
	{
	}
	public partial class UICharacterCreatePanel : UIPanel, IController
    {
        private INetworkSystem _network;

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UICharacterCreatePanelData ?? new UICharacterCreatePanelData();
			// please add init code here

            _network = this.GetSystem<INetworkSystem>();

            _network.Receive<CharacterCreateResponse>(OnReceivedCharacterCreate)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void OnCreateCharacter()
        {
            if (InputCharacterName.text.Length is >= 4 and <= 12)
            {
                _network.SendToServer(new CharacterCreateRequest()
                {
                    Name = InputCharacterName.text,
                    UnitId = 1  //TODO UnitId
                });
            }
        }

        private void OnReceivedCharacterCreate(CharacterCreateResponse response)
        {
            if (response.Error != NetError.Success)
            {
                Log.Error($"创建人物时出现报错: {response.Error.GetInfo().Description}");
                return;
            }

            this.SendCommand(new JoinMapCommand(response.Character.MapId, response.Character.CharacterId));
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
