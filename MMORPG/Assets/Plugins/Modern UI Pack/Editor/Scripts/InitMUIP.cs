#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    public class InitMUIP
    {
        [InitializeOnLoad]
        public class InitOnLoad
        {
            static InitOnLoad()
            {
                if (!EditorPrefs.HasKey("MUIPv5.Installed"))
                {
                    EditorPrefs.SetInt("MUIPv5.Installed", 1);
                    EditorPrefs.SetInt("MUIPv5_5.Installed", 1);
                    EditorUtility.DisplayDialog("Hello there!", "Thank you for purchasing Modern UI Pack." +
                        "\n\nTo use the UI Manager, go to Tools > Modern UI Pack > Show UI Manager." +
                        "\n\nIf you need help, feel free to contact us through our support channels.", "Got it!");
                }

                else if (EditorPrefs.HasKey("MUIPv5.Installed") && !EditorPrefs.HasKey("MUIPv5_5.Installed"))
                {
                    EditorPrefs.SetInt("MUIPv5_5.Installed", 1);
                    EditorUtility.DisplayDialog("Modern UI Pack", "Looks like you upgraded MUIP from 5.x to 5.5." +
                       "\n\n5.5 is a major update and is not fully backawards compatible." +
                       "\n\nIt is recommended to delete the previous version and do a fresh re-import." +
                       "\n\nYou may need to manually update your existing MUIP elements. In any case, you can always downgrade to 5.4.x.", "Dismiss");
                }

                if (!EditorPrefs.HasKey("MUIP.HasCustomEditorData"))
                {
                    EditorPrefs.SetInt("MUIP.HasCustomEditorData", 1);
                    EditorPrefs.SetString("MUIP.CustomEditorDark", AssetDatabase.GetAssetPath(Resources.Load("MUIP-EditorDark")));
                    EditorPrefs.SetString("MUIP.CustomEditorLight", AssetDatabase.GetAssetPath(Resources.Load("MUIP-EditorLight")));
                }
            }
        }
    }
}
#endif