using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Tool
{
    public class ProtoManager : Singleton<ProtoManager>
    {
        private Type[] _sortedProtoTypes;

        public ProtoManager() : base()
        {
            _sortedProtoTypes = Meta.AllProtoType.OrderBy(t => t.Name).ToArray();
        }

        public int TypeToID(Type type)
        {
            return Array.IndexOf(_sortedProtoTypes, type);
        }

        public Type IDToType(int id)
        {
            return _sortedProtoTypes[id];
        }
    }
}
