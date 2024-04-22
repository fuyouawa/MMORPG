using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Tool
{
    public static class ProtoManager
    {
        private static readonly Type[] _sortedProtoTypes;

        private static readonly HashSet<Type> _eventProtoTypes;

        static ProtoManager()
        {
            _sortedProtoTypes = Meta.AllProtoType.OrderBy(t => t.Name).ToArray();

            _eventProtoTypes = (from type in _sortedProtoTypes
                                 where type.Namespace.StartsWith("Common.Proto.Event")
                                 select type).ToHashSet();
        }

        public static int TypeToID(Type type)
        {
            return Array.IndexOf(_sortedProtoTypes, type);
        }

        public static Type IDToType(int id)
        {
            return _sortedProtoTypes[id];
        }

        public static bool IsEvent(Type type)
        {
            return _eventProtoTypes.Contains(type);
        }
    }
}
