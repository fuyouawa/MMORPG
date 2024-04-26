using UnityEngine;

namespace DuloGames.UI
{
	public class UISpellDatabase : ScriptableObject {

        #region singleton
        private static UISpellDatabase m_Instance;
        public static UISpellDatabase Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = Resources.Load("Databases/SpellDatabase") as UISpellDatabase;

                return m_Instance;
            }
        }
        #endregion

        public UISpellInfo[] spells;
	
		/// <summary>
		/// Get the specified SpellInfo by index.
		/// </summary>
		/// <param name="index">Index.</param>
		public UISpellInfo Get(int index)
		{
			return (spells[index]);
		}
	
		/// <summary>
		/// Gets the specified SpellInfo by ID.
		/// </summary>
		/// <returns>The SpellInfo or NULL if not found.</returns>
		/// <param name="ID">The spell ID.</param>
		public UISpellInfo GetByID(int ID)
		{
			for (int i = 0; i < this.spells.Length; i++)
			{
				if (this.spells[i].ID == ID)
					return this.spells[i];
			}
	
			return null;
		}
	}
}
