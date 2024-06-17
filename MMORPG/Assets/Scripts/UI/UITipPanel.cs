using UnityEngine;
using QFramework;
using MMORPG.Game;
using TMPro;

namespace MMORPG.UI
{
	public partial class UITipPanel : MonoBehaviour, IController
    {
        public TextMeshProUGUI Content;


        private void Awake()
        {
            
        }
        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
