/****************************************************************************
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 ****************************************************************************/

#if UNITY_2019_4_OR_NEWER
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace QFramework.Pro
{
    [InitializeOnLoad]
    public class Documents
    {
        static Documents()
        {
            EditorApplication.update -= ShowDocumentView;
            EditorApplication.update += ShowDocumentView;
            EditorApplication.playModeStateChanged -= PlayModeChanged;
            EditorApplication.playModeStateChanged += PlayModeChanged;
        }

        public static bool ShowAtStartUp
        {
            get => EditorPrefs.GetBool("QF ShowAtStartUp", true);
            set => EditorPrefs.SetBool("QF ShowAtStartUp", value);
        }

        private static void PlayModeChanged(PlayModeStateChange obj)
        {
            ShowDocumentView();
        }

        private static void ShowDocumentView()
        {
            const float startupTime = 120f;
            var showAtStartup = ShowAtStartUp && EditorApplication.timeSinceStartup < startupTime;

            if (showAtStartup)
            {
                PackageKitWindow.OpenWindowWithViewType<Documents>();
            }

            EditorApplication.update -= ShowDocumentView;
        }

        private GraphRootView mRootView;
        private VisualElement mRootVisualElement;

        private IMGUILayout mRoot;

        private DocumentsLocale mLocaleText = new DocumentsLocale();

        private List<DocKitGraphInfos> mGraphInfos;

        class DocKitGraphInfos
        {
            public string Path;
            public string Name;
        }

        public void Init()
        {
            mRoot = EasyIMGUI.Vertical();

            mGraphInfos = AssetDatabase.GetAllAssetPaths().Where(path =>
                    path.Contains("Doc") && path.EndsWith(".asset") &&
                    AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(DocumentGraph))
                .Select(path => new DocKitGraphInfos()
                {
                    Path = path,
                    Name = path.GetFileNameWithoutExtend()
                })
                .OrderBy(info => info.Name)
                .ToList();

            var label = EasyIMGUI.Label()
                .Text(() => mLocaleText.NewForQFDescription)
                .FontBold();

            var shortCutNotice = EasyIMGUI.Label()
                .Text(() => mLocaleText.CtrlEOrCommandE)
                .FontBold();

            EasyIMGUI.Custom().OnGUI(() =>
            {
                GUILayout.BeginHorizontal();
                ShowAtStartUp = GUILayout.Toggle(ShowAtStartUp, mLocaleText.ShowAtStartUp);
                GUILayout.EndHorizontal();

                label.DrawGUI();

                foreach (var graphInfo in mGraphInfos)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(graphInfo.Name);

                    if (GUILayout.Button(mLocaleText.OpenDoc, GUILayout.Width(200)))
                    {
                        var graph = AssetDatabase.LoadAssetAtPath<DocumentGraph>(graphInfo.Path);
                        DocKitGraphWindow.OpenWithGraph(graph);

                        RenderEndCommandExecutor.PushCommand(() =>
                        {
                            EditorWindow.GetWindow<PackageKitWindow>().Close();
                        });
                    }

                    GUILayout.EndHorizontal();
                }
                
                
                GUILayout.FlexibleSpace();
                shortCutNotice.DrawGUI();
                GUILayout.Space(6);
            }).Parent(mRoot);
        }

        public void OnGUI()
        {
            mRoot.DrawGUI();
        }

        public void OnDestroy()
        {
            mRoot = null;
        }
    }

    public class DocumentsLocale
    {
        private static bool CN => LocaleKitEditor.IsCN.Value;

        public string OpenDoc => CN ? "打开文档" : "Open Document";
        public string ShowAtStartUp => CN ? "启动时打开此页" : "Show At Startup";

        public string NewForQFDescription => CN
            ? "如果你第一次使用 QFramework Pro，请打开下列文档进行阅读"
            : "If you are using QFramework Pro for the first time, please open the following documents to read";

        public string CtrlEOrCommandE =>
            CN
                ? " Ctrl + E 或者 Command + E 或者菜单 QFramework/PackageKit 可以打开此窗口"
                : " CTRL + e or command + e or menu QFramework/PackageKit can open this window";
    }
}
#endif
#endif