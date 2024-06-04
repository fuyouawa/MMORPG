using System;
using QFramework;
using UnityEngine;

namespace MMORPG.UI
{
    public enum ChatChannelType
    {
        Composite,
        World,
        Map,
        Group
    }

    public class UIChatTabContent : MonoBehaviour
    {
        public RectTransform GroupChatItems;
        public ChatChannelType ChannelType;
        public UIChatItem ChatItem;

        private ResLoader _resLoader;

        private void Awake()
        {
            _resLoader = ResLoader.Allocate();
        }

        private void OnDestroy()
        {
            _resLoader.Recycle2Cache();
        }

        public void SubmitMessage(
            DateTime sendTime,
            ChatMessageType messageType,
            string characterName,
            string message,
            Color messageColor)
        {
            var item = Instantiate(ChatItem, GroupChatItems);

            item.Setup(
                sendTime,
                characterName,
                messageType,
                message,
                messageColor,
                ChannelType == ChatChannelType.Composite);
        }
    }
}
