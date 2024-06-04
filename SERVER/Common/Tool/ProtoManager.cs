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

        static ProtoManager()
        {
            AllProtoType = from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(Google.Protobuf.IMessage).IsAssignableFrom(t)
                select t;

            s_sortedProtoTypes = AllProtoType.OrderBy(t => t.Name).ToArray();
        }

        public static int TypeToID(Type type)
        {
            return Array.IndexOf(s_sortedProtoTypes, type);
        }

        public static Type IDToType(int id)
        {
            return s_sortedProtoTypes[id];
        }
    }
}
