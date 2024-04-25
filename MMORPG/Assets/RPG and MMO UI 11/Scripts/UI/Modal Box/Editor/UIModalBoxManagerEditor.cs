using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(UIModalBoxManager))]
    public class UIModalBoxManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This object must always be in the Resources folder in order to function correctly.", MessageType.Info);
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New modal box manager", "ModalBoxManager", "asset", "Create a new modal box manager.");
        }

        [MenuItem("Assets/Create/UI Managers/Modal Box Manager")]
        public static void CreateManager()
        {
            string assetPath = GetSavePath();
            UIModalBoxManager asset = ScriptableObject.CreateInstance("UIModalBoxManager") as UIModalBoxManager;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}
