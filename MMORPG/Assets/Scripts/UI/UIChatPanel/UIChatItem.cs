using UnityEngine;
using QFramework;
using System;
using MMORPG.Model;
using MMORPG.Tool;

namespace MMORPG.UI
{
    public enum ChatMessageType
    {
        World,
        Map,
        Group
    }

    public partial class UIChatItem : ViewController
    {
		void Start()
		{
			// Code Here
		}

        public void Setup(
            DateTime sendTime,
            string characterName,
            ChatMessageType type,
            string message,
            Color messageColor,
            bool isComposite)
        {
            var messageColorHex = messageColor.ToHex();

            if (isComposite)
            {
                var typeStr = type switch
                {
                    ChatMessageType.World => "世界",
                    ChatMessageType.Group => "组队",
                    ChatMessageType.Map => "地图",
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };

                TextMessage.SetText("<b>" +
                                    $"<color={messageColorHex}>[{sendTime:HH:mm:ss}][{typeStr}]</color>" +
                                    $"<color=#b59e8aff>[{characterName}]</color>" +
                                    "</b>" +
                                    $":{message}");
            }
            else
            {
                TextMessage.SetText("<b>" +
                                    $"<color={messageColorHex}>[{sendTime:HH:mm:ss}]</color>" +
                                    $"<color=#b59e8aff>[{characterName}]</color>" +
                                    "</b>" +
                                    $":{message}");
            }
        }
    }
}
