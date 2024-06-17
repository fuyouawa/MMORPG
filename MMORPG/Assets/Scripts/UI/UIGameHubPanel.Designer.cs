using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:04a43405-b343-42d1-ab33-ac5f865c1e5a
	public partial class UIGameHubPanel
	{
		public const string Name = "UIGameHubPanel";
		
		[SerializeField]
		public MMORPG.UI.UISkillPanel SkillPanel;
		[SerializeField]
		public MMORPG.UI.UIPlayerKnapsackPanel PlayerKnapsackPanel;
		[SerializeField]
		public MMORPG.UI.UIDialoguePanel DialoguePanel;
		[SerializeField]
		public UnityEngine.UI.Image TipPanel;
		
		private UIGameHubPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SkillPanel = null;
			PlayerKnapsackPanel = null;
			DialoguePanel = null;
			TipPanel = null;
			
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
