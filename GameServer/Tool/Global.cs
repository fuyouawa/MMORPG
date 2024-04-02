using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tool
{
    public class Global
    {
        public static NLog.Logger Logger { get; } = NLog.LogManager.GetCurrentClassLogger();
    }
}
