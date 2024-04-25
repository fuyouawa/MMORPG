using UnityEngine;

namespace DuloGames.UI
{
	public class UIItemDatabase : ScriptableObject {

        #region singleton
        private static UIItemDatabase m_Instance;
        public static UIItemDatabase Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = Resources.Load("Databases/ItemDatabase") as UIItemDatabase;

                return m_Instance;
            }
        }
        #endregion

        public UIItemInfo[] items;
		
		/// <summary>
		/// Get the specified ItemInfo by index.
		/// </summary>
		/// <param name="index">Index.</param>
		public UIItemInfo Get(int index)
		{
			return (this.items[index]);
		}
		
		/// <summary>
		/// Gets the specified ItemInfo by ID.
		/// </summary>
		/// <returns>The ItemInfo or NULL if not found.</returns>
		/// <param name="ID">The item ID.</param>
		public UIItemInfo GetByID(int ID)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				if (this.items[i].ID == ID)
					return this.items[i];
			}
			
			return null;
		}
	}
}
