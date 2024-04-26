using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DuloGames.UI
{
    [AddComponentMenu("UI/Modal Box Create", 8), DisallowMultipleComponent]
    public class UIModalBoxCreate : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private string m_Text1;
        [SerializeField][TextArea] private string m_Text2;
        [SerializeField] private string m_ConfirmText;
        [SerializeField] private string m_CancelText;

        [SerializeField] private Button m_HookToButton;
        #pragma warning restore 0649

        [Header("Events")]
        public UnityEvent onConfirm = new UnityEvent();
        public UnityEvent onCancel = new UnityEvent();

        protected void OnEnable()
        {
            if (this.m_HookToButton != null)
                this.m_HookToButton.onClick.AddListener(CreateAndShow);
        }

        protected void OnDisable()
        {
            if (this.m_HookToButton != null)
                this.m_HookToButton.onClick.RemoveListener(CreateAndShow);
        }

        public void CreateAndShow()
        {
            if (UIModalBoxManager.Instance == null)
            {
                Debug.LogWarning("Could not load the modal box manager while creating a modal box.");
                return;
            }
            
            UIModalBox box = UIModalBoxManager.Instance.Create(this.gameObject);
            if (box != null)
            {
                box.SetText1(this.m_Text1);
                box.SetText2(this.m_Text2);
                box.SetConfirmButtonText(this.m_ConfirmText);
                box.SetCancelButtonText(this.m_CancelText);
                box.onConfirm.AddListener(OnConfirm);
                box.onCancel.AddListener(OnCancel);
                box.Show();
            }
        }

        public void OnConfirm()
        {
            if (this.onConfirm != null)
            {
                this.onConfirm.Invoke();
            }
        }

        public void OnCancel()
        {
            if (this.onCancel != null)
            {
                this.onCancel.Invoke();
            }
        }
    }
}
