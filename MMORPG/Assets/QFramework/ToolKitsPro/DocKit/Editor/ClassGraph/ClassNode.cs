/****************************************************************************
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 ****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework.Pro
{
    /// <summary>
    /// 类节点
    /// </summary>
    [CreateNodeMenu("")]
    public class ClassNode : GUIGraphNode
    {
        [Input] public int Parent;
        [Output] public int Children;

        [HideInInspector] [SerializeField] public ClassInfo ClassInfo;

        /// <summary>
        /// 获得值
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public override object GetValue(GUIGraphNodePort port)
        {
            return null; // Replace this
        }
    }

    [CustomEditor(typeof(ClassNode))]
    public class ClassNodeInspector : GlobalGUIGraphNodeInspector
    {
        public override void OnInspectorGUI()
        {
            LocaleKitEditor.DrawSwitchToggle(GUI.skin.label.normal.textColor);

            if (GUILayout.Button(ClassGraphLocale.BackToGraphInspector, GUILayout.Height(40)))
            {
                Selection.activeObject = this.target.As<ClassNode>().graph;
            }
            // base.OnInspectorGUI();
        }
    }

    [CustomNodeEditorAttribute(typeof(ClassNode))]
    public class ClassNodeEditor : GUIGraphNodeEditor
    {
        public override void AddContextMenuItems(GenericMenu menu)
        {
            base.AddContextMenuItems(menu);

            if (Selection.objects.Length == 1 && Selection.activeObject is ClassNode)
            {
                var node = Selection.activeObject as ClassNode;
                menu.AddItem(new GUIContent("Open Class File"), false, () =>
                {
                    var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(node.ClassInfo.FilePath);
                    AssetDatabase.OpenAsset(monoScript, node.ClassInfo.StartLineNumber);
                });
            }
        }

        public override void OnHeaderGUI()
        {
            if (target.As<ClassNode>().ClassInfo.Inherit.IsNotNullAndEmpty())
            {
                GUILayout.Label(target.As<ClassNode>().ClassInfo.Name + ":" + target.As<ClassNode>().ClassInfo.Inherit,
                    GUIGraphResources.styles.NodeHeader,
                    GUILayout.Height(30));
            }
            else
            {
                GUILayout.Label(target.As<ClassNode>().ClassInfo.Name,
                    GUIGraphResources.styles.NodeHeader,
                    GUILayout.Height(30));
            }
        }


        public override void OnBodyGUI()
        {
            LocaleKitEditor.DrawSwitchToggle(Color.white);
            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            HashSet<string> excludes = new HashSet<string>() { "m_Script", "graph", "position", "ports" };
            serializedObject.Update();
            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                GUIGraphGUILayout.PropertyField(iterator, GUIContent.none, true);
            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (GUIGraphNodePort dynamicPort in target.DynamicPorts)
            {
                if (GUIGraphGUILayout.IsDynamicPortListPort(dynamicPort)) continue;

                // 不渲染文本
                GUIGraphGUILayout.PortField(GUIContent.none, dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();

            var node = target.As<ClassNode>();

            if (node.ClassInfo.Comment.Content.IsNotNullAndEmpty())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(ClassGraphLocale.Description, GUIGraphResources.styles.nodeLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(ClassGraphLocale.See))
                {
                    var monoScript =
                        AssetDatabase.LoadAssetAtPath<MonoScript>(target.As<ClassNode>().ClassInfo.FilePath);
                    AssetDatabase.OpenAsset(monoScript, target.As<ClassNode>().ClassInfo.StartLineNumber);
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();

                if (node.ClassInfo.Comment.Content.IsNotNullAndEmpty())
                {
                    GUILayout.Label(node.ClassInfo.Comment.Content, GUIGraphResources.styles.nodeLabel);
                }


                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button(ClassGraphLocale.See))
                {
                    var monoScript =
                        AssetDatabase.LoadAssetAtPath<MonoScript>(target.As<ClassNode>().ClassInfo.FilePath);
                    AssetDatabase.OpenAsset(monoScript, target.As<ClassNode>().ClassInfo.StartLineNumber);
                }
            }


            if (node.ClassInfo.Properties.Any())
            {
                GUILayout.Space(10);
                GUILayout.Label(ClassGraphLocale.Properties, GUIGraphResources.styles.nodeLabel);
                GUILayout.Space(2);

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();

                foreach (var property in node.ClassInfo.Properties)
                {
                    GUILayout.Space(5);

                    if (property.Comment.Content.IsNotNullAndEmpty())
                    {
                        GUILayout.Label(property.Comment.Content, GUIGraphResources.styles.nodeLabel);
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(property.ToString(), GUIGraphResources.styles.nodeLabel);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(ClassGraphLocale.See))
                    {
                        var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(node.ClassInfo.FilePath);
                        AssetDatabase.OpenAsset(monoScript, property.StartLineNumber);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            if (node.ClassInfo.Fields.Any())
            {
                GUILayout.Space(10);
                GUILayout.Label(ClassGraphLocale.Field, GUIGraphResources.styles.nodeLabel);
                GUILayout.Space(2);

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();

                foreach (var fieldInfo in node.ClassInfo.Fields)
                {
                    GUILayout.Space(5);

                    if (fieldInfo.Comment.Content.IsNotNullAndEmpty())
                    {
                        GUILayout.Label(fieldInfo.Comment.Content, GUIGraphResources.styles.nodeLabel);
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(fieldInfo.ToString(), GUIGraphResources.styles.nodeLabel);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(ClassGraphLocale.See))
                    {
                        var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(node.ClassInfo.FilePath);
                        AssetDatabase.OpenAsset(monoScript, fieldInfo.StartLineNumber);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            GUILayout.Label(ClassGraphLocale.Methods, GUIGraphResources.styles.nodeLabel);
            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginVertical();

            // 目前只渲染  public 类型的访问权限
            foreach (var method in node.ClassInfo.Methods.Where(m => m.AccessType == AccessType.Public))
            {
                GUILayout.Space(5);

                if (method.Comment.Content.IsNotNullAndEmpty())
                {
                    GUILayout.Label(method.Comment.Content, GUIGraphResources.styles.nodeLabel);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(method.ToString(), GUIGraphResources.styles.nodeLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(ClassGraphLocale.See))
                {
                    var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(node.ClassInfo.FilePath);
                    AssetDatabase.OpenAsset(monoScript, method.StartLineNumber);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private int mWidth = 208;

        public static int CalcHeight(ClassInfo classInfo)
        {
            var lineCount = 1; // className;

            lineCount += classInfo.Comment.Content.IsNotNullAndEmpty()
                ? classInfo.Comment.Content.Count(c => c == '\n') + 1
                : 0;

            foreach (var fieldInfo in classInfo.Fields)
            {
                lineCount++;
                lineCount += fieldInfo.Comment.Content.IsNotNullAndEmpty()
                    ? fieldInfo.Comment.Content.Count(c => c == '\n') + 1
                    : 0;
            }

            if (classInfo.Fields.Any())
            {
                lineCount++;
            }

            foreach (var propertyInfo in classInfo.Properties)
            {
                lineCount++;
                lineCount += propertyInfo.Comment.Content.IsNotNullAndEmpty()
                    ? propertyInfo.Comment.Content.Count(c => c == '\n') + 1
                    : 0;
            }

            if (classInfo.Properties.Any())
            {
                lineCount++;
            }

            foreach (var methodInfo in classInfo.Methods)
            {
                lineCount++;
                lineCount += methodInfo.Comment.Content.IsNotNullAndEmpty()
                    ? methodInfo.Comment.Content.Count(c => c == '\n') + 1
                    : 0;
            }

            if (classInfo.Methods.Any())
            {
                lineCount++;
            }

            return lineCount * 20 + 120;
        }

        public static int CalcWidth(ClassInfo classInfo)
        {
            int width = 208;
            var guiContent = new GUIContent();

            void CalcSize(string content)
            {
                guiContent.text = content;
                var size = GUIGraphResources.styles.nodeLabel.CalcSize(guiContent);
                if (size.x + 80 > width)
                {
                    width = (int)size.x + 80;
                }
            }


            if (classInfo.Comment.Content.IsNotNullAndEmpty())
            {
                CalcSize(classInfo.Comment.Content);
            }

            if (classInfo.Inherit.IsNotNullAndEmpty())
            {
                CalcSize(classInfo.Name + ":" + classInfo.Inherit);
            }
            else
            {
                CalcSize(classInfo.Name);
            }


            foreach (var propertyInfo in classInfo.Properties)
            {
                if (propertyInfo.Comment.Content.IsNotNullAndEmpty())
                {
                    CalcSize(propertyInfo.Comment.Content);
                }

                CalcSize(propertyInfo.ToString());
            }

            foreach (var fieldInfo in classInfo.Fields)
            {
                if (fieldInfo.Comment.Content.IsNotNullAndEmpty())
                {
                    CalcSize(fieldInfo.Comment.Content);
                }

                CalcSize(fieldInfo.ToString());
            }

            foreach (var methodInfo in classInfo.Methods)
            {
                if (methodInfo.Comment.Content.IsNotNullAndEmpty())
                {
                    CalcSize(methodInfo.Comment.Content);
                }

                CalcSize(methodInfo.ToString());
            }

            return width;
        }

        public bool ReCalcWidth = true;

        public override int GetWidth()
        {
            if (ReCalcWidth)
            {
                mWidth = CalcWidth(target.As<ClassNode>().ClassInfo);
                ReCalcWidth = false;
            }

            return mWidth;
        }
    }

    // [CustomPropertyDrawer(typeof(CommentInfo),false)]
    // public class CommentInfoDrawer : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         // base.OnGUI(position, property, label);
    //         
    //         // EditorGUI.PropertyField(position, property, label);
    //         // EditorGUI.LabelField(position, label, new GUILayout("No GUI Implemented"));
    //     }
    // }
}