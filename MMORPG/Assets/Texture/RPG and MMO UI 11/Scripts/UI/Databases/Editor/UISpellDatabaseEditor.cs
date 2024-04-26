using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class UISpellDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New spell database", "New spell database", "asset", "Create a new spell database.");
        }

        [MenuItem("Assets/Create/Databases/Spell Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            UISpellDatabase asset = ScriptableObject.CreateInstance("UISpellDatabase") as UISpellDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}
