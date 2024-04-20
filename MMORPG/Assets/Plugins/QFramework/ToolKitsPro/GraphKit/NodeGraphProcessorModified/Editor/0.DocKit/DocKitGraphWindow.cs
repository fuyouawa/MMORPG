#if UNITY_2019_4_OR_NEWER
using UnityEngine;
using GraphProcessor;
using UnityEditor;

namespace QFramework.Pro
{
    public class DefaultGraphWindow : BaseGraphWindow
    {
        public static void OpenWithGraph(BaseGraph graph)
        {
            var sceneView = GetWindow<SceneView>();
            var defaultGraphWindow = GetWindow<DefaultGraphWindow>();

            defaultGraphWindow.InitializeGraph(graph);
            defaultGraphWindow.Show();
            sceneView.AddTab(defaultGraphWindow);
            
            defaultGraphWindow.graphView.ResetPositionAndZoom();
        }

        public static void OpenWithGraphWithoutTab(BaseGraph graph)
        {
            var defaultGraphWindow = GetWindow<DefaultGraphWindow>();

            defaultGraphWindow.InitializeGraph(graph);
            defaultGraphWindow.Show();
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

        protected override void OnEnable()
        {
            base.OnEnable();
            LocaleKitEditor.IsCN.Register(OnLanguageChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LocaleKitEditor.IsCN.UnRegister(OnLanguageChanged);
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent();
            OnLanguageChanged(LocaleKitEditor.IsCN.Value);
            

            if (graphView == null)
            {
                graphView = new BaseGraphView(this);
            }
            else
            {
                graphView.OnDoubleClicked -= OnDoubleClicked;
                graphView.OnReload -= OnReload;
            }

            graphView.OnDoubleClicked += OnDoubleClicked;
            graphView.OnReload += OnReload;
            
            rootView.Add(graphView);
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
            
        }
    }
}
#endif