using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
    [AddComponentMenu("UI/UI Scene/Open")]
    public class UISceneOpen : MonoBehaviour
    {
        enum ActionType
        {
            SpecificID,
            LastScene,
        }

        enum InputKey
        {
            None,
            Submit,
            Cancel,
            Jump
        }

        #pragma warning disable 0649
        [SerializeField] private ActionType m_ActionType = ActionType.SpecificID;
        [SerializeField] private int m_SceneId = 0;
        [SerializeField] private InputKey m_InputKey = InputKey.None;
        [SerializeField] private Button m_HookToButton;
        #pragma warning restore 0649

        protected void OnEnable()
        {
            if (this.m_HookToButton != null)
                this.m_HookToButton.onClick.AddListener(Open);
        }

        protected void OnDisable()
        {
            if (this.m_HookToButton != null)
                this.m_HookToButton.onClick.RemoveListener(Open);
        }

        public void Open()
        {
            UIScene scene = null;

            switch (this.m_ActionType)
            {
                case ActionType.SpecificID:
                    scene = UISceneRegistry.instance.GetScene(this.m_SceneId);
                    break;
                case ActionType.LastScene:
                    scene = UISceneRegistry.instance.lastScene;
                    break;
            }
            
            if (scene != null)
            {
                scene.TransitionTo();
            }
        }

        protected void Update()
        {
            if (!this.isActiveAndEnabled ||!this.gameObject.activeInHierarchy || this.m_InputKey == InputKey.None)
                return;

            // Check if we are using the escape input for this and if the escape key was used in the window manager
            if (this.m_InputKey == InputKey.Cancel)
            {
                if (UIWindowManager.Instance != null && UIWindowManager.Instance.escapeInputName == "Cancel" && UIWindowManager.Instance.escapedUsed)
                    return;
            }

            // Check if we are using the escape input for this and if we have an active modal box
            if (this.m_InputKey == InputKey.Cancel && UIModalBoxManager.Instance != null && UIModalBoxManager.Instance.activeBoxes.Length > 0)
                return;

            string buttonName = string.Empty;

            switch (this.m_InputKey)
            {
                case InputKey.Submit:
                    buttonName = "Submit";
                    break;
                case InputKey.Cancel:
                    buttonName = "Cancel";
                    break;
                case InputKey.Jump:
                    buttonName = "Jump";
                    break;
            }
            
            if (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName))
            {
                this.Open();
            }
        }
    }
}
