using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AiSystem
{
    public abstract class AiBase
    {
        public abstract void Start();
        public abstract void Update();
    }
}
