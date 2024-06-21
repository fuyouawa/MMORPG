using UnityEngine;
using QFramework;
using MMORPG.Game;
using TMPro;
using UnityEngine.UIElements;

namespace MMORPG.UI
{
	public partial class UITipPanel : MonoBehaviour, IController
    {
        public static string Content = "";
        public TextMeshProUGUI ContentUI;
        public GameObject Background;

        private float _endTime = 0;


        private void Awake()
        {
            ContentUI.text = "";
        }

        private void Update()
        {
            // 你拾取了[魔法药剂]× 1
            if (Content != ContentUI.text)
            {
                _endTime = Time.time + 3;
                ContentUI.text = Content;
                Background.SetActive(true);
            }

            if (_endTime != 0 && Time.time >= _endTime)
            {
                Background.SetActive(false);
                ContentUI.text = "";
                Content = "";
                _endTime = 0;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
