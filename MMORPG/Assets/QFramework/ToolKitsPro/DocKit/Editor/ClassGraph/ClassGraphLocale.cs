/****************************************************************************
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 ****************************************************************************/

namespace QFramework.Pro
{
    internal static class ClassGraphLocale
    {
        private static bool IsCN => LocaleKitEditor.IsCN.Value;

        public static string ShowInGraph => IsCN ? "图中显示" : "Show In Graph";
        public static string Parse => IsCN ? "解析" : "Parse";

        public static string See => IsCN ? "查看" : "See";

        public static string Description => IsCN ? "描述" : "Description";
        public static string Methods => IsCN ? "方法" : "Methods";
        public static string Properties => IsCN ? "属性" : "Properties";
        public static string Field => IsCN ? "字段" : "Fields";

        public static string BackToGraphInspector => IsCN ? "回到图" : "Back To Graph Inspector";
        public static string Reload => IsCN ? "重新加载" : "Reload";
        public static string ShowGraphInspector => IsCN ? "显示此图 Inspector" : "Show Graph Inspector";

        public static string Maximize => IsCN ? "最大化" : "Maximize";
        public static string MoveToOriginPoint => IsCN ? "回到原点" : "Move To Origin Point";
    }
}