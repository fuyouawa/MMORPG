using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:c7a4917c-a06a-4cf1-a13c-f647ee8de127
	public partial class UIGameHubPanel
	{
		public const string Name = "UIGameHubPanel";
		
		[SerializeField]
		public MMORPG.UI.UISkillPanel SkillPanel;
		[SerializeField]
		public MMORPG.UI.UIPlayerKnapsackPanel PlayerKnapsackPanel;
		[SerializeField]
		public MMORPG.UI.UIDialoguePanel DialoguePanel;
		
		private UIGameHubPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SkillPanel = null;
			PlayerKnapsackPanel = null;
			DialoguePanel = null;
			
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
