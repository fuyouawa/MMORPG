#if UNITY_2019_4_OR_NEWER
/****************************************************************************
 * Copyright (c) 2022.3 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using GraphProcessor;
using UnityEditor;

namespace QFramework.Pro
{
    public class DocKitGraphWindow : BaseGraphWindow
    {
        private IUnRegister mLanguageChangedRegister = null;

        public static void OpenWithGraph(BaseGraph graph)
        {
            var defaultGraphWindow = GetWindow<DocKitGraphWindow>();

            if (!defaultGraphWindow.graph)
            {
                var sceneView = GetWindow<SceneView>();
                sceneView.AddTab(defaultGraphWindow);
            }

            defaultGraphWindow.InitializeGraph(graph);
            defaultGraphWindow.Show();

            defaultGraphWindow.RegisterLanguageChangedGracefully();
        }

        public static void OpenWithGraphWithoutTab(BaseGraph graph)
        {
            var defaultGraphWindow = GetWindow<DocKitGraphWindow>();

            defaultGraphWindow.InitializeGraph(graph);
            defaultGraphWindow.Show();
            defaultGraphWindow.RegisterLanguageChangedGracefully();
        }

        protected override void OnDestroy()
        {
            if (graphView != null)
            {
                graphView.OnDoubleClicked -= OnDoubleClicked;
            }

            graphView?.Dispose();
        }

        void OnDoubleClicked()
        {
            maximized = !maximized;
        }

        void OnReload()
        {
            OpenWithGraphWithoutTab(graph);
        }


        void RegisterLanguageChangedGracefully()
        {
            UnRegisterLanguageChangedGracefully();

            mLanguageChangedRegister = LocaleKitEditor.IsCN.Register(OnLanguageChanged);
        }

        void UnRegisterLanguageChangedGracefully()
        {
            if (mLanguageChangedRegister != null)
            {
                mLanguageChangedRegister.UnRegister();
                mLanguageChangedRegister = null;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterLanguageChangedGracefully();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterLanguageChangedGracefully();
        }


        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent();


            if (graphView == null)
            {
                graphView = new DocumentGraphView(this);
            }
            else
            {
                graphView.OnDoubleClicked -= OnDoubleClicked;
                graphView.OnReload -= OnReload;
            }

            graphView.OnDoubleClicked += OnDoubleClicked;
            graphView.OnReload += OnReload;

            rootView.Add(graphView);

            OnLanguageChanged(LocaleKitEditor.IsCN.Value);
        }


        private void OnLanguageChanged(bool cn)
        {
            if (cn)
            {
                titleContent.text = graph.CNName.IsNotNullAndEmpty() ? graph.CNName : graph.name;
            }
            else
            {
                titleContent.text = graph.name;
            }

            Repaint();
        }
    }
}
#endif