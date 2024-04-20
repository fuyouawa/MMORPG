#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(UIManagerButton))]
    public class UIManagerButtonEditor : Editor
    {
        private UIManagerButton uimTarget;

        private void OnEnable()
        {
            uimTarget = (UIManagerButton)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUI.indentLevel++;

            if (uimTarget.buttonManager == null) { return; }
           
            uimTarget.disabledBackground = EditorGUILayout.ObjectField("Background Disabled", uimTarget.disabledBackground, typeof(Image), true) as Image;
            uimTarget.normalBackground = EditorGUILayout.ObjectField("Background Normal", uimTarget.normalBackground, typeof(Image), true) as Image;
            uimTarget.highlightedBackground = EditorGUILayout.ObjectField("Background Highlighted", uimTarget.highlightedBackground, typeof(Image), true) as Image;
            
            if (uimTarget.buttonManager.enableIcon)
            {
                uimTarget.disabledIcon = EditorGUILayout.ObjectField("Icon Disabled", uimTarget.disabledIcon, typeof(Image), true) as Image;
                uimTarget.normalIcon = EditorGUILayout.ObjectField("Icon Normal", uimTarget.normalIcon, typeof(Image), true) as Image;
                uimTarget.highlightedIcon = EditorGUILayout.ObjectField("Icon Highlighted", uimTarget.highlightedIcon, typeof(Image), true) as Image;
            }

            if (uimTarget.buttonManager.enableText)
            {
                uimTarget.disabledText = EditorGUILayout.ObjectField("Text Disabled", uimTarget.disabledText, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                uimTarget.normalText = EditorGUILayout.ObjectField("Text Normal", uimTarget.normalText, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                uimTarget.highlightedText = EditorGUILayout.ObjectField("Text Highlighted", uimTarget.highlightedText, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            }
        }
    }
}
#endif