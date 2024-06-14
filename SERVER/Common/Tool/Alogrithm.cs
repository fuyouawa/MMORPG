using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMORPG.Common.Tool
{
    public static class Alogrithm
    {
        public static bool IsUniqueNull<T1, T2>(T1 t1, T2 t2)
        {
            return (t1 == null && t2 != null) || (t1 != null && t2 == null);
        }
    }
}
