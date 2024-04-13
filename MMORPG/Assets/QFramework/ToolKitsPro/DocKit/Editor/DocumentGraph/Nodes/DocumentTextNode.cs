#if UNITY_2019_4_OR_NEWER
/****************************************************************************
 * Copyright (c) 2022.3 liangxiegame UNDER MIT LICENSE
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QFramework
{
    [Serializable, NodeMenuItem("DocKit/TextNode 文本节点")]
    public class DocumentTextNode : DocKitNode
    {
        public string Text
        {
            get => LocaleKitEditor.IsCN.Value ? CNText : ENText;
            set
            {
                if (LocaleKitEditor.IsCN.Value)
                {
                    CNText = value;
                }
                else
                {
                    ENText = value;
                }
            }
        }

        // [IsCompatibleWithGraph]
        // public bool IsCompatibleWithGraph()
        // {
        //     return true;
        // }

        [HideInInspector] public string CNText;
        [HideInInspector] public string ENText;

        public override string name => LocaleKitEditor.IsCN.Value ? "文本" : "Text";

        public override string GetMarkdownString()
        {
            return Text;
        }
    }

    public abstract class DocKitNode : BaseNode
    {
        public abstract string GetMarkdownString();
    }

    [NodeCustomEditor(typeof(DocumentTextNode))]
    public class DocumentTextNodeView : BaseNodeView
    {
        protected UnityEngine.UIElements.TextField mTextField;

        public override void Disable()
        {
            owner.graph.Save();
            base.Disable();
        }

        public override void Enable()
        {
            base.Enable();

            var node = nodeTarget as DocumentTextNode;

            new IMGUIContainer(() => LocaleKitEditor.DrawSwitchToggle(Color.white))
                .AddToParent(new VisualElement()
                    .Padding(6, 0, 0, 0)
                    .AddToParent(rightTitleContainer)
                );

            mTextField = new UnityEngine.UIElements.TextField
            {
                value = node.Text,
                multiline = true
            };

            mTextField.FixIME();
            mTextField.isDelayed = true;
            mTextField.RegisterValueChangedCallback(e =>
            {
                owner.RegisterCompleteObjectUndo("Updated textNode input");
                node.Text = mTextField.value;
                owner.graph.Save();
                // node.PushSaveCommand(() => { owner.graph.Save(); });
            });

            controlsContainer.Add(mTextField);

            IUnRegister unRegister = null;
            unRegister = LocaleKitEditor.IsCN.RegisterWithInitValue(cn =>
            {
                try
                {
                    mTextField.value = cn ? node.CNText : node.ENText;
                    UpdateTitle();
                }
                catch
                {
                    unRegister.UnRegister();
                }
            });
        }
    }
}
#endif