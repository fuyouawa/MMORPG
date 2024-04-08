using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Tool
{
    public class ProtoManager : Singleton<ProtoManager>
    {
        private readonly Type[] _sortedProtoTypes;

        private readonly HashSet<Type> _suddenProtoTypes;

        public ProtoManager() : base()
        {
            _sortedProtoTypes = Meta.AllProtoType.OrderBy(t => t.Name).ToArray();

            _suddenProtoTypes = (from type in _sortedProtoTypes
                                 where type.Namespace.EndsWith(".Sudden")
                                 select type).ToHashSet();
        }

        public int TypeToID(Type type)
        {
            return Array.IndexOf(_sortedProtoTypes, type);
        }

        public Type IDToType(int id)
        {
            return _sortedProtoTypes[id];
        }

        public bool IsSudden(Type type)
        {
            return _suddenProtoTypes.Contains(type);
        }
    }
}
