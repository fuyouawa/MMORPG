/****************************************************************************
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 ****************************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QFramework.Pro
{
    [CreateNodeMenu("")]
    public class OutsideBaseClassNode : GUIGraphNode
    {
        [SerializeField]
        [HideInInspector]
        public string Name;

        [Output] public int Children;
    }

    [GUIGraphNodeEditor.CustomNodeEditorAttribute(typeof(OutsideBaseClassNode))]
    public class OutsideBaseClassNodeEditor : GUIGraphNodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Space(5);
            GUILayout.Label(target.As<OutsideBaseClassNode>().Name,
                GUIGraphResources.styles.NodeHeader,
                GUILayout.Height(30));
            
        }

        public override void OnBodyGUI()
        {
            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            HashSet<string> excludes = new HashSet<string>() { "m_Script", "graph", "position", "ports" };
            serializedObject.Update();
            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren)) {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                GUIGraphGUILayout.PropertyField(iterator,GUIContent.none, true);
            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (GUIGraphNodePort dynamicPort in target.DynamicPorts) {
                if (GUIGraphGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                
                // 不渲染文本
                GUIGraphGUILayout.PortField(GUIContent.none, dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}