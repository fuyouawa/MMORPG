/****************************************************************************
 * Copyright (c) 2022.3 liangxiegame UNDER MIT LICENSE
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    public class DocumentGraphLocale
    {
        public bool CN => LocaleKitEditor.IsCN.Value;

        public string OpenWindow => CN ? "打开窗口" : "Open Window";
        public string GenerateMarkdown => CN ? "生成 Markdown 文件" : "Generate Markdown File";

        public string ClearUnusedImages => CN ? "清空不用的图片资源" : "Clear Unused Images";

        public string DeleteFile => CN ? "删除文件" : "Delete File";
        
        public string Success => CN ? "成功" : "Success";
    }
}