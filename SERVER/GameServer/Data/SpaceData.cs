using Common.Tool;
using GameServer.Tool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Data
{
    public class SpaceData : Singleton<SpaceData>
    {
        void Load()
        {
            var content = JsonHelper.LoadJsonFromFile("SpaceDefine");
            var dict = JsonConvert.DeserializeObject<Dictionary<int, SpaceDefine>>(content);

        }
    }
}
