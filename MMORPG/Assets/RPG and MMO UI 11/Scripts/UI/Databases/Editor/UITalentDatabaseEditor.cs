using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    public class UITalentDatabaseEditor
    {
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New talent database", "New talent database", "asset", "Create a new talent database.");
        }

        [MenuItem("Assets/Create/Databases/Talent Database")]
        public static void CreateDatabase()
        {
            string assetPath = GetSavePath();
            UITalentDatabase asset = ScriptableObject.CreateInstance("UITalentDatabase") as UITalentDatabase;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}
