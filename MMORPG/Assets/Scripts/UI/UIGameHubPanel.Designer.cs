using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:1bef8ad2-b650-4c1d-bc4c-0f8a2da264cc
	public partial class UIGameHubPanel
	{
		public const string Name = "UIGameHubPanel";
		
		[SerializeField]
		public MMORPG.UI.UISkillPanel SkillPanel;
		[SerializeField]
		public MMORPG.UI.UIPlayerKnapsackPanel PlayerKnapsackPanel;
		[SerializeField]
		public UnityEngine.UI.Image DialoguePanel;
		
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
