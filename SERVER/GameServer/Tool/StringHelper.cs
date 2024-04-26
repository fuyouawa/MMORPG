using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tool
{
    public static class StringHelper
    {
        public static bool NameVerify(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            var trimmedName = name.Trim();
            if (trimmedName.Length != name.Length)
            {
                return false;
            }
            if (name.Length < 4 || name.Length > 12)
            {
                return false;
            }
            return true;
        }

    }
}
