using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:ee751116-b63a-44fe-9a98-8ad9d2c33d18
	public partial class UIGameHubPanel
	{
		public const string Name = "UIGameHubPanel";
		
		[SerializeField]
		public MMORPG.UI.RevivePanel RevivePanel;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextReveiveTime;
		[SerializeField]
		public MMORPG.Game.PlayerPropertyPanel PlayerPropertyPanel;
		[SerializeField]
		public UnityEngine.UI.Image ImageMpFill;
		[SerializeField]
		public UnityEngine.UI.Text TextMpPercentage;
		[SerializeField]
		public UnityEngine.UI.Image ImageBgHpFill;
		[SerializeField]
		public UnityEngine.UI.Image ImageHpFill;
		[SerializeField]
		public UnityEngine.UI.Text TextHpPercentage;
		[SerializeField]
		public UnityEngine.UI.Text TextLevel;
		[SerializeField]
		public MMORPG.UI.UISkillPanel SkillPanel;
		[SerializeField]
		public MMORPG.UI.UIPlayerKnapsackPanel PlayerKnapsackPanel;
		[SerializeField]
		public RectTransform TipPanel;
		[SerializeField]
		public MMORPG.UI.DialoguePanel DialoguePanel;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextName;
		[SerializeField]
		public TMPro.TextMeshProUGUI Content;
		[SerializeField]
		public RectTransform GroupOptionBox;
		[SerializeField]
		public MMORPG.UI.ChatPanel ChatPanel;
		[SerializeField]
		public UnityEngine.UI.ToggleGroup GroupTabsMenu;
		[SerializeField]
		public MMORPG.UI.UIChatTabContent TabContentCompositeChat;
		[SerializeField]
		public MMORPG.UI.UIChatTabContent TabContentWorldChat;
		[SerializeField]
		public MMORPG.UI.UIChatTabContent TabContentMapChat;
		[SerializeField]
		public MMORPG.UI.UIChatTabContent TabContentGroupChat;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextChannel;
		[SerializeField]
		public TMPro.TMP_InputField InputMessage;
		
		private UIGameHubPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			RevivePanel = null;
			TextReveiveTime = null;
			PlayerPropertyPanel = null;
			ImageMpFill = null;
			TextMpPercentage = null;
			ImageBgHpFill = null;
			ImageHpFill = null;
			TextHpPercentage = null;
			TextLevel = null;
			SkillPanel = null;
			PlayerKnapsackPanel = null;
			TipPanel = null;
			DialoguePanel = null;
			TextName = null;
			Content = null;
			GroupOptionBox = null;
			ChatPanel = null;
			GroupTabsMenu = null;
			TabContentCompositeChat = null;
			TabContentWorldChat = null;
			TabContentMapChat = null;
			TabContentGroupChat = null;
			TextChannel = null;
			InputMessage = null;
			
			mData = null;
		}
		
		public UIGameHubPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGameHubPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGameHubPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
