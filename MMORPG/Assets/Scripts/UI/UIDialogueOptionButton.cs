using MMORPG.Command;
using MMORPG.Game;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIDialogueOptionButton : MonoBehaviour, IController
    {
        public TextMeshProUGUI Content;
        public int Idx;

        public void OnClick()
        {
            this.SendCommand(new InteractCommand(0, Idx));
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}
