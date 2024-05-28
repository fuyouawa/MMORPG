using Sirenix.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Tool
{
    public static class SirenixEditorGUIHelper
    {
        /// <summary>
        /// 原函数为Sirenix.Utilities.Editor.SirenixEditorGUI.PrivateMessageBox
        /// 因为被设为私有了无法使用, 所以复制出来并修订
        /// </summary>
        public static Rect PrivateMessageBox(string message, MessageType messageType, GUIStyle messageBoxStyle, bool wide = true, Action<GenericMenu> onContextClick = null)
        {
            Texture icon = messageType switch
            {
                MessageType.Info => EditorIcons.UnityInfoIcon,
                MessageType.Warning => EditorIcons.UnityWarningIcon,
                MessageType.Error => EditorIcons.UnityErrorIcon,
                _ => null,
            };
            var rect = GUILayoutUtility.GetRect(GUIHelper.TempContent(message, icon), messageBoxStyle);
            rect = EditorGUI.IndentedRect(rect);
            GUI.Label(rect, GUIHelper.TempContent(message, icon), messageBoxStyle);
            if (UnityEngine.Event.current.button == 1 && UnityEngine.Event.current.type == EventType.MouseDown && rect.Contains(UnityEngine.Event.current.mousePosition))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Copy message"), false, delegate
                {
                    Clipboard.Copy(message);
                });
                onContextClick?.Invoke(menu);
                menu.ShowAsContext();
                UnityEngine.Event.current.Use();
            }
            return rect;
        }

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
