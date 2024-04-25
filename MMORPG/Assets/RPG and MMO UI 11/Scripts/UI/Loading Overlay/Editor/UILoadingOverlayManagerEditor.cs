using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(UILoadingOverlayManager))]
    public class UILoadingOverlayManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This object must always be in the Resources folder in order to function correctly.", MessageType.Info);
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New loading overlay manager", "LoadingOverlayManager", "asset", "Create a new loading overlay manager.");
        }

        [MenuItem("Assets/Create/UI Managers/Loading Overlay Manager")]
        public static void CreateManager()
        {
            string assetPath = GetSavePath();
            UILoadingOverlayManager asset = ScriptableObject.CreateInstance("UILoadingOverlayManager") as UILoadingOverlayManager;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}
