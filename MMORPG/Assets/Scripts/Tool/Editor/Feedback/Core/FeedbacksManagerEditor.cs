using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Tool.Editor
{
    [CustomEditor(typeof(FeedbacksManager))]
    public class FeedbacksManagerEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            (target as FeedbacksManager)?.OnSceneGUI();
        }
    }
}
