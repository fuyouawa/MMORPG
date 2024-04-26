using UnityEngine;
using UnityEditor;
using System.Collections;

public static class CreatePrefabs
{
    [MenuItem("Tools/Create Prefabs from Selection")]
    static void CreatePrefabsFromSelection()
    {
        if (Selection.transforms.Length == 0)
        {
            Debug.LogWarning("Please make a selection first.");
            return;
        }

        string savePath = GetSavePath();

        if (!string.IsNullOrEmpty(savePath))
        {
            savePath = savePath.Remove(0, savePath.IndexOf("Assets")) + "/";

            foreach (Transform trans in Selection.transforms)
            {
                PrefabUtility.SaveAsPrefabAsset(trans.gameObject, savePath + trans.name + ".prefab");
            }

            AssetDatabase.Refresh();
        }
    }

    private static string GetSavePath()
    {
        return EditorUtility.SaveFolderPanel("Prefabs directory", "assets", ""); 
    }
}
