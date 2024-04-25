using UnityEngine;

namespace DuloGames.UI
{
    public class UILoadingOverlayManager : ScriptableObject
    {
        #region singleton
        private static UILoadingOverlayManager m_Instance;
        public static UILoadingOverlayManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = Resources.Load("LoadingOverlayManager") as UILoadingOverlayManager;

                return m_Instance;
            }
        }
        #endregion

        #pragma warning disable 0649
        [SerializeField] private GameObject m_LoadingOverlayPrefab;
        #pragma warning restore 0649

        /// <summary>
        /// Gets the loading overlay prefab.
        /// </summary>
        public GameObject prefab
        {
            get
            {
                return this.m_LoadingOverlayPrefab;
            }
        }

        /// <summary>
        /// Creates a loading overlay.
        /// </summary>
        /// <returns>The loading overlay component.</returns>
        public UILoadingOverlay Create()
        {
            if (this.m_LoadingOverlayPrefab == null)
                return null;
            
            GameObject obj = Instantiate(this.m_LoadingOverlayPrefab);

            return obj.GetComponent<UILoadingOverlay>();
        }
    }
}
