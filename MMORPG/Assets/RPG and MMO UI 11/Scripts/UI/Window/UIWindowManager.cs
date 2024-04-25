using UnityEngine;
using System.Collections.Generic;

namespace DuloGames.UI
{
	public class UIWindowManager : MonoBehaviour {

        private static UIWindowManager m_Instance;

        /// <summary>
        /// Gets the current instance of the window manager.
        /// </summary>
        public static UIWindowManager Instance
        {
            get { return m_Instance; }
        }
        
        [SerializeField] private string m_EscapeInputName = "Cancel";
        private bool m_EscapeUsed = false;

        /// <summary>
        /// Gets the escape input name.
        /// </summary>
        public string escapeInputName
        {
            get { return this.m_EscapeInputName; }
        }

        /// <summary>
        /// Gets a value indicating whether the escape input was used to hide a window in this frame.
        /// </summary>
        public bool escapedUsed
        {
            get { return this.m_EscapeUsed; }
        }

        protected virtual void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_Instance.Equals(this))
                m_Instance = null;
        }

        protected virtual void Update()
		{
            // Reset the escape used variable
            if (this.m_EscapeUsed)
                this.m_EscapeUsed = false;

			// Check for escape key press
			if (Input.GetButtonDown(this.m_EscapeInputName))
			{
                // Check for currently opened modal and exit this method if one is found
                UIModalBox[] modalBoxes = FindObjectsOfType<UIModalBox>();

                if (modalBoxes.Length > 0)
                {
                    foreach (UIModalBox box in modalBoxes)
                    {
                        // If the box is active
                        if (box.isActive && box.isActiveAndEnabled && box.gameObject.activeInHierarchy)
                            return;
                    }
                }

				// Get the windows list
				List<UIWindow> windows = UIWindow.GetWindows();
				
				// Loop through the windows and hide if required
				foreach (UIWindow window in windows)
				{
					// Check if the window has escape key action
					if (window.escapeKeyAction != UIWindow.EscapeKeyAction.None)
					{
						// Check if the window should be hidden on escape
						if (window.IsOpen && (window.escapeKeyAction == UIWindow.EscapeKeyAction.Hide || window.escapeKeyAction == UIWindow.EscapeKeyAction.Toggle || (window.escapeKeyAction == UIWindow.EscapeKeyAction.HideIfFocused && window.IsFocused)))
						{
							// Hide the window
							window.Hide();

                            // Mark the escape input as used
                            this.m_EscapeUsed = true;
                        }
					}
				}

                // Exit the method if the escape was used for hiding windows
                if (this.m_EscapeUsed)
                    return;
                
				// Loop through the windows again and show any if required
				foreach (UIWindow window in windows)
				{
					// Check if the window has escape key action toggle and is not shown
					if (!window.IsOpen && window.escapeKeyAction == UIWindow.EscapeKeyAction.Toggle)
					{
						// Show the window
						window.Show();
					}
				}
			}
		}
	}
}
