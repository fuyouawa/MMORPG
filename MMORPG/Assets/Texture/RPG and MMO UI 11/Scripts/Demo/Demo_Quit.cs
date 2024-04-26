using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuloGames.UI
{
    public class Demo_Quit : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Button m_HookToButton;
        #pragma warning restore 0649

        protected void OnEnable()
        {
            if (this.m_HookToButton != null)
                this.m_HookToButton.onClick.AddListener(ExitGame);
        }

        protected void OnDisable()
        {
            if (this.m_HookToButton != null)
                this.m_HookToButton.onClick.RemoveListener(ExitGame);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
