#if UNITY_2019_3_OR_NEWER
using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QFramework.Pro
{
    [Serializable]
    public class GraphRootView
    {
        public GraphRootView(VisualElement rootVisualElement)
        {
            RootVisualElement = rootVisualElement;

            RootVisualElement.name = "graphRootView";
            RootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("BlueGraphEditor/Variables"));
            RootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(GraphWindowStyle));
        }

        public VisualElement RootVisualElement;
        public BaseGraphView GraphView;

        public readonly string GraphWindowStyle = "GraphProcessorStyles/BaseGraphView";

        public bool IsGraphLoaded => GraphView != null && GraphView.graph != null;

        [SerializeField] public BaseGraph Graph;
    }
}
#endif