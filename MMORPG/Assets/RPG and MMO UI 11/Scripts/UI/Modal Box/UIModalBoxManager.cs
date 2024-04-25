using UnityEngine;
using System.Collections.Generic;

namespace DuloGames.UI
{
    public class UIModalBoxManager : ScriptableObject
    {
        #region singleton
        private static UIModalBoxManager m_Instance;
        public static UIModalBoxManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = Resources.Load("ModalBoxManager") as UIModalBoxManager;

                return m_Instance;
            }
        }
        #endregion

        #pragma warning disable 0649
        [SerializeField] private GameObject m_ModalBoxPrefab;
        #pragma warning restore 0649

        private List<UIModalBox> m_ActiveBoxes = new List<UIModalBox>();

        /// <summary>
        /// Gets the modal box prefab.
        /// </summary>
        public GameObject prefab
        {
            get { return this.m_ModalBoxPrefab; }
        }

        /// <summary>
        /// Gets an array of the currently active modal boxes.
        /// </summary>
        public UIModalBox[] activeBoxes
        {
            get
            {
                // Do a cleanup
                this.m_ActiveBoxes.RemoveAll(item => item == null);
                return this.m_ActiveBoxes.ToArray();
            }
        }

        /// <summary>
        /// Creates a modal box.
        /// </summary>
        /// <param name="rel">Relative game object used to find the canvas.</param>
        public UIModalBox Create(GameObject rel)
        {
            if (this.m_ModalBoxPrefab == null || rel == null)
                return null;

            Canvas canvas = UIUtility.FindInParents<Canvas>(rel);

            if (canvas != null)
            {
                GameObject obj = Instantiate(this.m_ModalBoxPrefab, canvas.transform, false);

                return obj.GetComponent<UIModalBox>();
            }

            return null;
        }

        /// <summary>
        /// Register a box as active (Used internally).
        /// </summary>
        /// <param name="box"></param>
        public void RegisterActiveBox(UIModalBox box)
        {
            if (!this.m_ActiveBoxes.Contains(box))
                this.m_ActiveBoxes.Add(box);
        }

        /// <summary>
        /// Unregister an active box (Used internally).
        /// </summary>
        /// <param name="box"></param>
        public void UnregisterActiveBox(UIModalBox box)
        {
            if (this.m_ActiveBoxes.Contains(box))
                this.m_ActiveBoxes.Remove(box);
        }
    }
}
