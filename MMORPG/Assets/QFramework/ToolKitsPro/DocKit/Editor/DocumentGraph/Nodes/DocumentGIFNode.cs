#if UNITY_2019_4_OR_NEWER
/****************************************************************************
 * Copyright (c) 2022.3 liangxiegame UNDER MIT LICENSE
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using GraphProcessor;

namespace QFramework
{
    [Serializable, NodeMenuItem("DocKit/GIFNode GIF 图像节点")]
    public class DocumentGIFNode:DocKitNode
    {
        // []
        // public string GIFFilePath;
        public override string GetMarkdownString()
        {
            return string.Empty;
        }
    }

    public class DocumentGIFNodeView : BaseNodeView
    {
        
        
        
    }
}
#endif