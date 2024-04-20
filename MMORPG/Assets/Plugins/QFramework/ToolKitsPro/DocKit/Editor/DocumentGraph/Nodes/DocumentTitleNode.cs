#if UNITY_2019_4_OR_NEWER
using System;
using GraphProcessor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QFramework
{
    [Serializable, NodeMenuItem("DocKit/TitleNode 标题节点")]
    public class DocumentTitleNode : DocumentTextNode
    {
        public enum Enum
        {
            H1,
            H2,
            H3,
        }

        [SerializeField] [HideInInspector] public Enum Size = Enum.H2;

        public override string name => LocaleKitEditor.IsCN.Value ? "标题" : "Title";

        public override string GetMarkdownString()
        {
            if (Size == Enum.H1) return base.GetMarkdownString().Builder().AddPrefix("# ").ToString();
            if (Size == Enum.H2) return base.GetMarkdownString().Builder().AddPrefix("## ").ToString();
            if (Size == Enum.H3) return base.GetMarkdownString().Builder().AddPrefix("### ").ToString();
            return base.GetMarkdownString();
        }
    }

    [NodeCustomEditor(typeof(DocumentTitleNode))]
    public class DocumentTitleNodeView : DocumentTextNodeView
    {
        public override void Enable()
        {
            var node = nodeTarget as DocumentTitleNode;

            new EnumField(node.Size)
                .Self(self =>
                {
                    self.RegisterValueChangedCallback(e =>
                    {
                        node.Size = (DocumentTitleNode.Enum)e.newValue;
                        UpdateFontSize(self, node.Size);
                    });

                    UpdateFontSize(self, node.Size);
                })
                .AddToParent(controlsContainer);

            base.Enable();
        }

        void UpdateFontSize(EnumField self, DocumentTitleNode.Enum size)
        {
            switch (size)
            {
                case DocumentTitleNode.Enum.H1:
                    self.style.fontSize = 48;
                    break;
                case DocumentTitleNode.Enum.H2:
                    self.style.fontSize = 36;
                    break;
                case DocumentTitleNode.Enum.H3:
                    self.style.fontSize = 24;
                    break;
            }
        }
    }
}
#endif