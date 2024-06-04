using System;
using System.Threading.Tasks;
using Common.Proto.Base;
using Common.Proto.Map;
using DuloGames.UI;
using MMORPG.Game;
using UnityEngine;
using QFramework;
using MMORPG.Model;
using MMORPG.System;

namespace MMORPG.UI
{
	public class UIChatPanelData : UIPanelData
	{
	}
	public partial class UIChatPanel : UIPanel, IController
    {
        public Color WorldMessageColor;
        public Color MapMessageColor;
        public Color GroupMessageColor;

        public UITab CurrentTab { get; private set; }
        public UIChatTabContent CurrentTabContent { get; private set; }

        public UITab[] TabMenus { get; private set; }

        private bool _isSendingMessage = false;
        private INetworkSystem _network;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIChatPanelData ?? new UIChatPanelData();
			// please add init code here

            TabMenus = GroupTabsMenu.GetComponentsInChildren<UITab>();

            foreach (var tab in TabMenus)
            {
                tab.onValueChanged.AddListener(isSelected => OnTabChanged(tab, isSelected));
            }

            CurrentTab = (UITab)GroupTabsMenu.GetFirstActiveToggle();
            CurrentTabContent = CurrentTab.targetContent.GetComponent<UIChatTabContent>();

            _network = this.GetSystem<INetworkSystem>();

            _network.Receive<ReceiveChatMessageResponse>(OnReceiveChatMessage)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnTabChanged(UITab tab, bool isSelected)
        {
            if (isSelected)
            {
                CurrentTab = (UITab)GroupTabsMenu.GetFirstActiveToggle();
                CurrentTabContent = CurrentTab.targetContent.GetComponent<UIChatTabContent>();
                switch (CurrentTabContent.ChannelType)
                {
                    case ChatChannelType.Composite:
                        TextChannel.SetText("世界");
                        break;
                    case ChatChannelType.World:
                        TextChannel.SetText("世界");
                        break;
                    case ChatChannelType.Map:
                        TextChannel.SetText("地图");
                        break;
                    case ChatChannelType.Group:
                        TextChannel.SetText("组队");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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


        public async Task OnSubmitMessage()
        {
            Debug.Assert(GroupTabsMenu.GetFirstActiveToggle() == CurrentTab);

            if (_isSendingMessage) return;

            if (InputMessage.text.Trim().Length > 0)
            {

                var messageType = CurrentTabContent.ChannelType switch
                {
                    ChatChannelType.Composite => ChatMessageType.World,
                    ChatChannelType.World => ChatMessageType.World,
                    ChatChannelType.Map => ChatMessageType.Map,
                    ChatChannelType.Group => ChatMessageType.Group,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var messageColor = GetChatMessageColor(messageType);

                _isSendingMessage = true;

                var characterId = this.GetModel<IUserModel>().CharacterId.Value;
                var characterName = this.GetModel<IUserModel>().CharacterName.Value;


                _network.SendToServer(new SubmitChatMessageRequest()
                {
                    CharacterId = characterId,
                    Message = InputMessage.text,
                    MessageType = (Common.Proto.Map.ChatMessageType)messageType
                });

                var response = await _network.ReceiveAsync<SubmitChatMessageResponse>();

                if (response.Error == NetError.Success)
                {
                    var time = response.Timestamp.ToDateTime();
                    if (CurrentTabContent.ChannelType == ChatChannelType.Composite)
                    {
                        TabContentWorldChat.SubmitMessage(time, messageType, characterName, InputMessage.text, messageColor);
                        CurrentTabContent.SubmitMessage(time, messageType, characterName, InputMessage.text, messageColor);
                    }
                    else
                    {
                        TabContentCompositeChat.SubmitMessage(time, messageType, characterName, InputMessage.text, messageColor);
                        CurrentTabContent.SubmitMessage(time, messageType, characterName, InputMessage.text, messageColor);
                    }
                    InputMessage.text = string.Empty;
                }
                else
                {
                    //TODO SubmitChatMessageResponse Error处理
                }

                _isSendingMessage = false;
            }
        }

        private void OnReceiveChatMessage(ReceiveChatMessageResponse response)
        {
            var messageType = (ChatMessageType)response.MessageType;

            var tabContent = messageType switch
            {
                ChatMessageType.World => TabContentWorldChat,
                ChatMessageType.Map => TabContentMapChat,
                ChatMessageType.Group => TabContentGroupChat,
                _ => throw new ArgumentOutOfRangeException()
            };

            var messageColor = GetChatMessageColor(messageType);

            var time = response.Timestamp.ToDateTime();

            TabContentCompositeChat.SubmitMessage(time, messageType, response.CharacterName, InputMessage.text, messageColor);
            tabContent.SubmitMessage(time, messageType, response.CharacterName, InputMessage.text, messageColor);
        }

        public Color GetChatMessageColor(ChatMessageType messageType)
        {
            return messageType switch
            {
                ChatMessageType.World => WorldMessageColor,
                ChatMessageType.Map => MapMessageColor,
                ChatMessageType.Group => GroupMessageColor,
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
            };
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
