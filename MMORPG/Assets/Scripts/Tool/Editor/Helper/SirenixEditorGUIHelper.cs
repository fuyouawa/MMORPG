using Sirenix.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Tool
{
    public static class SirenixEditorGUIHelper
    {
        public static bool Foldout(Rect labelRect, bool isVisible, GUIContent label, out Rect valueRect, GUIStyle style = null)
        {
            valueRect = labelRect;
            if (label == null)
            {
                label = new GUIContent(" ");
                if (EditorGUIUtility.hierarchyMode)
                {
                    labelRect.width = 2f;
                }
                else
                {
                    labelRect.width = 18f;
                    valueRect.xMin += 18f;
                }
            }
            else
            {
                float indent = GUIHelper.CurrentIndentAmount;
                labelRect = new Rect(labelRect.x, labelRect.y, GUIHelper.BetterLabelWidth - indent, labelRect.height);
                valueRect.xMin = labelRect.xMax;
            }
            return SirenixEditorGUI.Foldout(labelRect, isVisible, label);
        }
    }
}
