using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(UITooltipManager))]
    public class UITooltipManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This object must always be in the Resources folder in order to function correctly.", MessageType.Info);
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New tooltip manager", "TooltipManager", "asset", "Create a new tooltip manager.");
        }

        [MenuItem("Assets/Create/UI Managers/Tooltip Manager")]
        public static void CreateManager()
        {
            string assetPath = GetSavePath();
            UITooltipManager asset = ScriptableObject.CreateInstance("UITooltipManager") as UITooltipManager;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}
