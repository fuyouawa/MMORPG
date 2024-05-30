using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	// Generate Id:72c94463-4e72-430a-a06a-0958c3d1ce2c
	public partial class UICharacterSelectPanel
	{
		public const string Name = "UICharacterSelectPanel";
		
		[SerializeField]
		public RectTransform CharactersGroup;
		[SerializeField]
		public UnityEngine.UI.Button BtnCreateCharacter;
		[SerializeField]
		public UnityEngine.UI.Button BtnPlay;
		
		private UICharacterSelectPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			CharactersGroup = null;
			BtnCreateCharacter = null;
			BtnPlay = null;
			
			mData = null;
		}
		
		public UICharacterSelectPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UICharacterSelectPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UICharacterSelectPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
