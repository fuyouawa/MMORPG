#if UNITY_2019_4_OR_NEWER
/****************************************************************************
 * Copyright (c) 2022.3 liangxiegame UNDER MIT LICENSE
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GraphProcessor;
using QFramework.Pro;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@DocKit/Create Doc Graph-创建文档", fileName = "New Document", order = int.MinValue)]
    public class DocumentGraph : BaseGraph
    {
        [SerializeField] [HideInInspector] private long mGraphCurrentID2Generate = 0;


        public long GenerateID()
        {
            mGraphCurrentID2Generate++;
            this.PushSaveCommand(this.Save);
            return mGraphCurrentID2Generate;
        }

        public void GenerateMarkdown()
        {
            var processor = new DocumentGraphMarkdownProcessor(this);
            processor.Run();
        }

        public void ClearUnusedImages()
        {
            var imagePathHashSet = new HashSet<string>();

            foreach (var baseNode in nodes)
            {
                if (baseNode is DocumentImageNode)
                {
                    var imageNode = baseNode.As<DocumentImageNode>();
                    if (imageNode.CNTexture) imagePathHashSet.Add(AssetDatabase.GetAssetPath(imageNode.CNTexture));
                    if (imageNode.ENTexture) imagePathHashSet.Add(AssetDatabase.GetAssetPath(imageNode.ENTexture));
                }
            }

            var imageFolder = AssetDatabase.GetAssetPath(this).GetFolderPath().Builder().Append("/").Append(this.name)
                .Append("/Editor").ToString();

            var imageFiles = Directory.GetFiles(imageFolder);


            var localeText = new DocumentGraphLocale();
            foreach (var image in imageFiles.Where(imagePath => !imagePath.EndsWith(".meta")))
            {
                if (!imagePathHashSet.Contains(image))
                {
                    File.Delete(image);
                    File.Delete(image + ".meta");

                    LogKit.Builder()
                        .GreenColor(s =>
                            s.Append(localeText.DeleteFile)
                                .Append(" ")
                                .Append(image)
                                .Append(" ")
                                .Append(localeText.Success)
                        )
                        .LogInfo();
                }
            }

            AssetDatabase.Refresh();
        }
    }

    public class DocumentGraphMarkdownProcessor : BaseGraphProcessor
    {
        List<DocKitNode> processList;

        /// <summary>
        /// Manage graph scheduling and processing
        /// </summary>
        /// <param name="graph">Graph to be processed</param>
        public DocumentGraphMarkdownProcessor(BaseGraph graph) : base(graph)
        {
        }

        public override void UpdateComputeOrder()
        {
            foreach (var graphNode in graph.nodes)
            {
                Debug.Log(graphNode);
            }

            processList = graph.nodes.OrderBy(n => n.position.y)
                .ThenBy(n => n.position.x).Cast<DocKitNode>().ToList();
        }


        public override void Run()
        {
            int count = processList.Count;

            var builder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                processList[i].OnProcess();
                var markdownString = processList[i].GetMarkdownString();
                builder.AppendLine(markdownString);
                builder.AppendLine();
            }

            Debug.Log(builder.ToString());


            var graphPath = AssetDatabase.GetAssetPath(graph);
            var parentFolder = graphPath.GetFolderPath();
            var mdFilePath = parentFolder + "/" + graph.name + "_" +
                             (LocaleKitEditor.IsCN.Value ? "CN" : "EN") + ".md";
            File.WriteAllText(mdFilePath, builder.ToString());
            AssetDatabase.Refresh();
            Debug.Log(mdFilePath);
            
        }
    }

    [CustomEditor(typeof(DocumentGraph), true)]
    public class DocumentGraphInspector : GraphInspector, IUnRegisterList
    {
        public readonly DocumentGraphLocale mLocaleText = new DocumentGraphLocale();

        protected override void CreateInspector()
        {
            base.CreateInspector();

            var docGraph = graph as DocumentGraph;

            new IMGUIContainer(() => { LocaleKitEditor.DrawSwitchToggle(GUI.skin.label.normal.textColor); })
                .AddToParent(root);
            
            new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal("box");
                if (LocaleKitEditor.IsCN.Value)
                {
                    GUILayout.Label("名字:",GUILayout.Width(40));
                    EditorGUI.BeginChangeCheck();
                    docGraph.CNName = EditorGUILayout.TextField(docGraph.CNName);
                    if (EditorGUI.EndChangeCheck())
                    {
                        docGraph.PushSaveCommand(()=>docGraph.Save());
                    }
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    GUILayout.Label("Name:" + docGraph.name);
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }).AddToParent(root);


            var openWindowButton = new Button(() => DocKitGraphWindow.OpenWithGraph(target as DocumentGraph))
                .AddToParent(root);

            var generateMarkdown = new Button(() => { graph.As<DocumentGraph>().GenerateMarkdown(); })
                .AddToParent(root);

            var clearUnusedImagesButton =
                new Button(() => { graph.As<DocumentGraph>().ClearUnusedImages(); }).AddToParent(root);

            LocaleKitEditor.IsCN.RegisterWithInitValue(_ =>
            {
                openWindowButton.text = mLocaleText.OpenWindow;
                generateMarkdown.text = mLocaleText.GenerateMarkdown;
                clearUnusedImagesButton.text = mLocaleText.ClearUnusedImages;
            }).AddToUnregisterList(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.UnRegisterAll();
        }

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is DocumentGraph graph)
            {
                DocKitGraphWindow.OpenWithGraph(graph);

                return true;
            }

            return false;
        }
    }


    public class DocumentGraphView : BaseGraphView
    {
        public DocumentGraphView(EditorWindow window) : base(window)
        {
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GraphView)
            {
                Vector2 position = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
                evt.menu.AppendAction(LocaleKitEditor.IsCN.Value ? "文本节点" : "TextNode",
                    d =>
                    {
                        AddNode(BaseNode.CreateFromType<DocumentTextNode>(position));
                    },
                    DropdownMenuAction.AlwaysEnabled);
                
                evt.menu.AppendAction(LocaleKitEditor.IsCN.Value ? "图像节点" : "ImageNode",
                    d =>
                    {
                        AddNode(BaseNode.CreateFromType<DocumentImageNode>(position));
                    },
                    DropdownMenuAction.AlwaysEnabled);
                
                evt.menu.AppendAction(LocaleKitEditor.IsCN.Value ? "标题节点" : "TitleNode",
                    d =>
                    {
                        AddNode(BaseNode.CreateFromType<DocumentTitleNode>(position));
                    },
                    DropdownMenuAction.AlwaysEnabled);
                
                evt.menu.AppendSeparator();
                
            }
            
            base.BuildContextualMenu(evt);
            
            
        }
    }
}
#endif