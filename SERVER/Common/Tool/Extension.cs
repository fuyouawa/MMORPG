using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Tool
{
    public struct ErrorInfo
    {
        public Proto.Base.NetError Error;
        public string Description;
    }

    public static class Extension
    {
        public static ErrorInfo GetInfo(this Proto.Base.NetError error)
        {
            var info = new ErrorInfo() { Error = error };
            switch (error)
            {
                case Proto.Base.NetError.Success:
                    info.Description = "请求成功";
                    break;
                case Proto.Base.NetError.Error:
                    info.Description = "请求失败, 原因未知";
                    break;
                default:
                    throw new NotImplementedException();
            }
            return info;
        }
    }
}
