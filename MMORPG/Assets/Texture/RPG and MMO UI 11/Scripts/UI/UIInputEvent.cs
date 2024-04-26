using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace DuloGames.UI
{
    public class UIInputEvent : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private string m_InputName;

        [SerializeField] private UnityEvent m_OnButton;
        [SerializeField] private UnityEvent m_OnButtonDown;
        [SerializeField] private UnityEvent m_OnButtonUp;
        #pragma warning restore 0649

        private Selectable m_Selectable;

        protected void Awake()
        {
            this.m_Selectable = this.gameObject.GetComponent<Selectable>();
        }

        protected void Update()
        {
            if (!this.isActiveAndEnabled || !this.gameObject.activeInHierarchy || string.IsNullOrEmpty(this.m_InputName))
                return;

            // Break if the currently selected game object is a selectable
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                // Check for selectable
                Selectable targetSelectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

                if ((this.m_Selectable == null && targetSelectable != null) || (this.m_Selectable != null && targetSelectable != null && !this.m_Selectable.Equals(targetSelectable)))
                    return;
            }

            // Check if we are using the escape input for this and if the escape key was used in the window manager
            if (UIWindowManager.Instance != null && UIWindowManager.Instance.escapeInputName == this.m_InputName && UIWindowManager.Instance.escapedUsed)
                return;

            try
            {
                if (Input.GetButton(this.m_InputName))
                    this.m_OnButton.Invoke();

                if (Input.GetButtonDown(this.m_InputName))
                    this.m_OnButtonDown.Invoke();

                if (Input.GetButtonUp(this.m_InputName))
                    this.m_OnButtonUp.Invoke();
            }
            catch (ArgumentException)
            {
                this.enabled = false;
                Debug.LogWarning("Input \"" + this.m_InputName + "\" used by game object \"" + gameObject.name + "\" is not defined.");
            }
        }
    }
}
