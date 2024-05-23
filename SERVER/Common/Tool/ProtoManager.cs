using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Tool
{
    public static class ProtoManager
    {
        public static IEnumerable<Type> AllProtoType;

        private static readonly Type[] s_sortedProtoTypes;

        private static readonly HashSet<Type> s_eventProtoTypes;

        static ProtoManager()
        {
            AllProtoType = from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(Google.Protobuf.IMessage).IsAssignableFrom(t)
                select t;

            s_sortedProtoTypes = AllProtoType.OrderBy(t => t.Name).ToArray();

            s_eventProtoTypes = (from type in s_sortedProtoTypes
                                 where type.Namespace.StartsWith("Common.Proto.EventLike")
                                 select type).ToHashSet();
        }

        public static int TypeToID(Type type)
        {
            return Array.IndexOf(s_sortedProtoTypes, type);
        }

        public static Type IDToType(int id)
        {
            return s_sortedProtoTypes[id];
        }

        public static bool IsEventLike(Type type)
        {
            return s_eventProtoTypes.Contains(type);
        }
    }
}
