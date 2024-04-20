/****************************************************************************
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 ****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEditor;
using UnityEngine;

namespace QFramework.Pro
{
    /// <summary>
    /// 可视化的方式查看类图
    /// see class diagram by visual way
    /// </summary>
    [CreateAssetMenu(menuName = "@DocKit/Create Class Graph-创建类图", fileName = "New ClassGraph",
        order = int.MinValue + 2)]
    public class ClassGraph : GUIGraph
    {
        /// <summary>
        /// 将脚本解析成图
        /// Parse scripts to diagram
        /// </summary>
        public void Parse()
        {
            var graphPath = AssetDatabase.GetAssetPath(this);

            var graphFolderPath = graphPath.GetFolderPath();

            var files = Directory.GetFiles(graphFolderPath, "*.cs", SearchOption.AllDirectories);

            var classList = new List<Tuple<string, ClassDeclarationSyntax>>();

            // 清空所有节点
            nodes.ForEachReverse(this.RemoveNode);


            foreach (var file in files)
            {
                var code = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetCompilationUnitRoot();

                SearchClass(root.Members, classList, file);
            }


            var classDB = new ClassDB();
            var rootOutsideClasses = new HashSet<string>();

            foreach (var pair in classList)
            {
                var classInfo = new ClassInfo(pair.Item1).Parse(pair.Item2);

                classDB.Add(classInfo);

                if (!classDB.NameIndex.Get(classInfo.Inherit).Any())
                {
                    rootOutsideClasses.Add(classInfo.Inherit);
                }
            }

            var rootTreeNodes = rootOutsideClasses.Select(className => new ClassTreeNode()
            {
                Name = className,
                Outside = true
            }.Parse(classDB)).ToList();

            RenderClassTree(rootTreeNodes, classDB, 0, 0);

            GUIGraphWindow.OpenWithGraph(this);
        }


        int RenderClassTree(List<ClassTreeNode> treeNodes, ClassDB classDB, int x, int y,
            GUIGraphNodePort parentNodeChildrenPort = null)
        {
            if (treeNodes == null || treeNodes.Count == 0) return 30;
            int totalHeight = 0;

            foreach (var treeNode in treeNodes)
            {
                if (treeNode.Outside)
                {
                    var baseClassNode = AddNode<OutsideBaseClassNode>();

                    baseClassNode.Name = treeNode.Name;
                    baseClassNode.position.x = x;
                    baseClassNode.position.y = y;
                    
                    var childHeight = RenderClassTree(treeNode.Children, classDB, x + 300, y,
                        baseClassNode.GetOutputPort("Children"));

                    y += Mathf.Max(300, childHeight);
                    totalHeight += Mathf.Max(300, childHeight);
                }
                else
                {
                    var classInfo = classDB.NameIndex.Get(treeNode.Name).FirstOrDefault();

                    var classNode = AddNode<ClassNode>();

                    classNode.ClassInfo = classInfo;
                    classNode.position.x = x;
                    classNode.position.y = y;

                    classNode.GetInputPort("Parent").Connect(parentNodeChildrenPort);

                    var width = ClassNodeEditor.CalcWidth(classInfo); // 加个间距
                    var height = ClassNodeEditor.CalcHeight(classInfo); // 加个间距
                    var childHeight = RenderClassTree(treeNode.Children, classDB, x + width + 50, y,
                        classNode.GetOutputPort("Children"));

                    y += Mathf.Max(height, childHeight);
                    totalHeight += Mathf.Max(height, childHeight);
                }
            }

            return totalHeight + 30; // 加个默认间距
        }

        public class ClassTreeNode
        {
            public string Name;

            public List<ClassTreeNode> Children = new List<ClassTreeNode>();

            public bool Outside;


            public ClassTreeNode Parse(ClassDB classDB)
            {
                Children.AddRange(classDB.ParentIndex.Get(Name).Select(c => new ClassTreeNode()
                {
                    Name = c.Name,
                    Outside = false
                }.Parse(classDB)).ToList());
                return this;
            }
        }

        public class ClassDB : Table<ClassInfo>
        {
            public TableIndex<string, ClassInfo> ParentIndex = new TableIndex<string, ClassInfo>(c => c.Inherit);
            public TableIndex<string, ClassInfo> NameIndex = new TableIndex<string, ClassInfo>(c => c.Name);


            protected override void OnAdd(ClassInfo item)
            {
                ParentIndex.Add(item);
                NameIndex.Add(item);
            }

            protected override void OnRemove(ClassInfo item)
            {
                ParentIndex.Remove(item);
                NameIndex.Remove(item);
            }

            protected override void OnClear()
            {
                ParentIndex.Clear();
                NameIndex.Clear();
            }

            public override IEnumerator<ClassInfo> GetEnumerator()
            {
                return ParentIndex.Dictionary.Values.SelectMany(v => v).GetEnumerator();
            }

            protected override void OnDispose()
            {
                ParentIndex.Dispose();
                NameIndex.Dispose();
            }
        }

        void SearchClass(SyntaxList<MemberDeclarationSyntax> members,
            List<Tuple<string, ClassDeclarationSyntax>> outputList, string filePath)
        {
            foreach (var member in members)
            {
                if (member.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    SearchClass(member.As<NamespaceDeclarationSyntax>().Members, outputList, filePath);
                }
                else if (member.IsKind(SyntaxKind.ClassDeclaration))
                {
                    outputList.Add(
                        new Tuple<string, ClassDeclarationSyntax>(filePath, member.As<ClassDeclarationSyntax>()));
                }
            }
        }
    }

    [CustomEditor(typeof(ClassGraph), true)]
    public class ClassGraphInspector : GUIGlobalGraphInspector
    {
        GUIContent mGUIContent = new GUIContent(ClassGraphLocale.ShowInGraph);

        private bool mNextFrame = true;
        public override void OnInspectorGUI()
        {
            if (mNextFrame) // 会报错 所以先等一帧
            {
                mNextFrame = false;
                LocaleKitEditor.DrawSwitchToggle(GUI.skin.label.normal.textColor);
            }

            if (GUILayout.Button(ClassGraphLocale.Parse, GUILayout.Height(40)))
            {
                target.As<ClassGraph>().Parse();
            }

            GUILayout.BeginVertical("box");

            foreach (var node in target.As<ClassGraph>().nodes.Where(n => n is ClassNode))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(node.As<ClassNode>().ClassInfo.Name);
                GUILayout.FlexibleSpace();

                mGUIContent.text = ClassGraphLocale.ShowInGraph;
                if (GUILayout.Button(mGUIContent, GUILayout.Width(GUIStyle.none.CalcSize(mGUIContent).x + 10)))
                {
                    Selection.activeObject = node;
                    GUIGraphWindow.OpenWithGraph(node.graph).Home();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            // base.OnInspectorGUI();
        }
    }
}