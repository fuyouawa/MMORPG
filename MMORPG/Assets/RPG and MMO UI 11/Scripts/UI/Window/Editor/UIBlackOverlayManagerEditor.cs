using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(UIBlackOverlayManager))]
    public class UIBlackOverlayManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This object must always be in the Resources folder in order to function correctly.", MessageType.Info);
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New black overlay manager", "BlackOverlayManager", "asset", "Create a new black overlay manager.");
        }

        [MenuItem("Assets/Create/UI Managers/Black Overlay Manager")]
        public static void CreateManager()
        {
            string assetPath = GetSavePath();
            UIBlackOverlayManager asset = ScriptableObject.CreateInstance("UIBlackOverlayManager") as UIBlackOverlayManager;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}
