#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(UIManagerProgressBarLoop))]
    public class UIManagerProgressBarLoopEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var UIManagerProgressBarLoop = target as UIManagerProgressBarLoop;

            using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(UIManagerProgressBarLoop.hasBackground == true)))
            {
                if (group.visible == true)
                {
                    UIManagerProgressBarLoop.background = EditorGUILayout.ObjectField("Background", UIManagerProgressBarLoop.background, typeof(Image), true) as Image;
                }
            }
        }
    }
}
#endif