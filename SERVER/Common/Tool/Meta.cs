using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Tool
{
    public static class Meta
    {
        public static IEnumerable<Type> AllProtoType => from t in Assembly.GetExecutingAssembly().GetTypes()
                                                        where typeof(Google.Protobuf.IMessage).IsAssignableFrom(t)
                                                        select t;
    }
}
