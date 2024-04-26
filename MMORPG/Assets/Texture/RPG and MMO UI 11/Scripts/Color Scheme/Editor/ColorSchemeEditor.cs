using UnityEngine;
using DuloGames.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(ColorScheme))]
    public class ColorSchemeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Apply Color Scheme"))
            {
                ColorScheme scheme = this.target as ColorScheme;

                if (scheme != null)
                {
                    // Apply the color scheme to the loaded scenes
                    scheme.ApplyColorScheme();

                    if (!Application.isPlaying)
                        EditorSceneManager.MarkAllScenesDirty();
                }
            }
        }
        
        private static string GetSavePath()
        {
            return EditorUtility.SaveFilePanelInProject("New color scheme", "ColorScheme", "asset", "Create a new color scheme.");
        }

        [MenuItem("Assets/Create/UI Color Scheme")]
        public static void CreateManager()
        {
            string assetPath = GetSavePath();
            ColorScheme asset = ScriptableObject.CreateInstance("ColorScheme") as ColorScheme;  //scriptable object
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.Refresh();
        }
    }
}
