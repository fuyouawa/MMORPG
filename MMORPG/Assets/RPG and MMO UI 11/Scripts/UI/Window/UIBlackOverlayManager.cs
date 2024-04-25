using UnityEngine;

namespace DuloGames.UI
{
    public class UIBlackOverlayManager : ScriptableObject
    {
        #region singleton
        private static UIBlackOverlayManager m_Instance;
        public static UIBlackOverlayManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = Resources.Load("BlackOverlayManager") as UIBlackOverlayManager;

                return m_Instance;
            }
        }
        #endregion

        #pragma warning disable 0649
        [SerializeField] private GameObject m_BlackOverlayPrefab;
        #pragma warning restore 0649

        /// <summary>
        /// Gets the black overlay prefab.
        /// </summary>
        public GameObject prefab
        {
            get
            {
                return this.m_BlackOverlayPrefab;
            }
        }

        /// <summary>
        /// Creates a black overlay.
        /// </summary>
        /// <param name="parent">The transform parent.</param>
        /// <returns>The black overlay component.</returns>
        public UIBlackOverlay Create(Transform parent)
        {
            if (this.m_BlackOverlayPrefab == null)
                return null;

            GameObject obj = Instantiate(this.m_BlackOverlayPrefab, parent);

            return obj.GetComponent<UIBlackOverlay>();
        }
    }
}
