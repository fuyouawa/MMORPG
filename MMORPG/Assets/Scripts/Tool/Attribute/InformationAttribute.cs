using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    public enum InformationType
    {
        None,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// 扩展自Sirenix.OdinInspector.InfoBoxAttribute
    /// 优化中文字体的显示
    /// 使用方法和Sirenix.OdinInspector.InfoBoxAttribute一样
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class InformationAttribute : PropertyAttribute
    {
        public string Message;

        public InformationType MessageType;

        public string VisibleIf;

        public bool GUIAlwaysEnabled;

        public string IconColor;

        public InformationAttribute(string message, InformationType messageType = InformationType.Info)
        {
            Message = message;
            MessageType = messageType;
        }
    }
}
