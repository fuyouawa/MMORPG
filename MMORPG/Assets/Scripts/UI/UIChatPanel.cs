using System;
using DuloGames.UI;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.UI
{
	public class UIChatPanelData : UIPanelData
	{
	}
	public partial class UIChatPanel : UIPanel
    {
        public Color WorldMessageColor;
        public Color MapMessageColor;
        public Color GroupMessageColor;

        public UITab CurrentTab { get; private set; }
        public UIChatTabContent CurrentTabContent { get; private set; }

        public UITab[] TabMenus { get; private set; }

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


        public void OnSubmitMessage()
        {
            Debug.Assert(GroupTabsMenu.GetFirstActiveToggle() == CurrentTab);
            if (InputMessage.text.Trim().Length > 0)
            {
                Color messageColor;
                ChatMessageType messageType;

                switch (CurrentTabContent.ChannelType)
                {
                    case ChatChannelType.Composite:
                        messageColor = WorldMessageColor;
                        messageType = ChatMessageType.World;
                        break;
                    case ChatChannelType.World:
                        messageColor = WorldMessageColor;
                        messageType = ChatMessageType.World;
                        break;
                    case ChatChannelType.Map:
                        messageColor = MapMessageColor;
                        messageType = ChatMessageType.Map;
                        break;
                    case ChatChannelType.Group:
                        messageColor = GroupMessageColor;
                        messageType = ChatMessageType.Group;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (CurrentTabContent.ChannelType == ChatChannelType.Composite)
                {
                    TabContentWorldChat.SubmitMessage(messageType, InputMessage.text, messageColor);
                    CurrentTabContent.SubmitMessage(messageType, InputMessage.text, messageColor);
                }
                else
                {
                    TabContentCompositeChat.SubmitMessage(messageType, InputMessage.text, messageColor);
                    CurrentTabContent.SubmitMessage(messageType, InputMessage.text, messageColor);
                }
                InputMessage.text = string.Empty;
            }
        }
	}
}
