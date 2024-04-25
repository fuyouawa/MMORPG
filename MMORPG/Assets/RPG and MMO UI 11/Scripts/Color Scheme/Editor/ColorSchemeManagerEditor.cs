using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(ColorSchemeManager))]
    public class ColorSchemeManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This object must always be in the Resources folder in order to function correctly.", MessageType.Info);
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New color scheme manager", "ColorSchemeManager", "asset", "Create a new color scheme manager.");
        }

        [MenuItem("Assets/Create/UI Managers/Color Scheme Manager")]
        public static void CreateManager()
        {
            string assetPath = GetSavePath();
            ColorSchemeManager asset = ScriptableObject.CreateInstance("ColorSchemeManager") as ColorSchemeManager;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}
