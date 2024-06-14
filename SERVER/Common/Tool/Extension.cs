using System;
using System.Collections.Generic;
using System.Text;

namespace MMORPG.Common.Tool
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
                case Proto.Base.NetError.LoginConflict:
                    info.Description = "当前账号已被登录";
                    break;
                case Proto.Base.NetError.IncorrectUsernameOrPassword:
                    info.Description = "错误的用户名或密码";
                    break;
                case Proto.Base.NetError.IllegalUsername:
                    info.Description = "非法用户名";
                    break;
                case Proto.Base.NetError.IllegalCharacterName:
                    info.Description = "非法角色名";
                    break;
                case Proto.Base.NetError.RepeatUsername:
                    info.Description = "用户名已被注册";
                    break;
                case Proto.Base.NetError.RepeatCharacterName:
                    info.Description = "角色名已被注册";
                    break;
                case Proto.Base.NetError.InvalidCharacter:
                    info.Description = "无效角色";
                    break;
                case Proto.Base.NetError.InvalidMap:
                    info.Description = "无效地图";
                    break;
                case Proto.Base.NetError.CharacterCreationLimitReached:
                    info.Description = "角色创建已达最大限制!";
                    break;
                case Proto.Base.NetError.UnknowError:
                default:
                    info.Description = "未知错误";
                    break;
            }
            return info;
        }

        public static LinkedListNode<T>? FindIf<T>(this LinkedList<T> list, Func<T, bool> predicate)
        {
            var node = list.First;
            while (node != null)
            {
                if (predicate(node.Value))
                    return node;
                node = node.Next;
            }
            return node;
        }

        public static void RemoveIf<T>(this LinkedList<T> list, Func<T, bool> predicate)
        {
            list.Remove(list.FindIf(predicate));
        }
    }
}
