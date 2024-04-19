using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Tool
{
    public static class ProtoManager
    {
        private static readonly Type[] _sortedProtoTypes;

        private static readonly HashSet<Type> _emergencyProtoTypes;

        static ProtoManager()
        {
            _sortedProtoTypes = Meta.AllProtoType.OrderBy(t => t.Name).ToArray();

            _emergencyProtoTypes = (from type in _sortedProtoTypes
                                 where type.Namespace.EndsWith(".Emergency")
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

        public static bool IsEmergency(Type type)
        {
            return _emergencyProtoTypes.Contains(type);
        }
    }
}
