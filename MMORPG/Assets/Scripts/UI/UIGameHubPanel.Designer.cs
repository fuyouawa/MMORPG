using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:3ae36f90-93e4-48e0-b752-8081f39959ef
	public partial class UIGameHubPanel
	{
		public const string Name = "UIGameHubPanel";
		
		[SerializeField]
		public MMORPG.UI.UIPlayerKnapsackPanel PlayerKnapsackPanel;
		
		private UIGameHubPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			PlayerKnapsackPanel = null;
			
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
