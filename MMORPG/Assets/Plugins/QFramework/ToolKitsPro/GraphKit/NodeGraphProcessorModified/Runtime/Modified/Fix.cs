/****************************************************************************
 * Copyright (c) 2022.3 liangxiegame UNDER MIT LICENSE
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

#if UNITY_2019_4_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace QFramework
{
    public static class FixEx
    {
        public static TextField FixIME(this TextField self)
        {
            self.RegisterCallback<FocusInEvent>(e=>Input.imeCompositionMode = IMECompositionMode.On);
            self.RegisterCallback<FocusOutEvent>(e=>Input.imeCompositionMode = IMECompositionMode.Auto);
            return self;
        }
    }
}