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

        public InformationAttribute(string message)
        {
            Message = message;
        }

        public InformationAttribute(string message, InformationType messageType = InformationType.Info, string visiableIf = null)
        {
            Message = message;
            MessageType = messageType;
            VisibleIf = visiableIf;
        }

        public InformationAttribute(string message, InfoMessageType messageType = InfoMessageType.Info, string visiableIf = null)
        {
            Message = message;
            MessageType = messageType switch
            {
                InfoMessageType.Info => InformationType.Info,
                InfoMessageType.Warning => InformationType.Warning,
                InfoMessageType.Error => InformationType.Error,
                _ => InformationType.None
            };
            VisibleIf = visiableIf;
        }
    }
}
