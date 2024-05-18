using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GameServer.Tool;
using Serilog;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace GameServer.Manager
{
    /// <summary>
    /// 怪物生成器
    /// </summary>
    public class Spawner
    {
        public SpawnManager SpawnManager;
        public SpawnDefine SpawnDefine;

        public Spawner(SpawnManager manager, SpawnDefine define)
        {
            SpawnManager = manager;
            SpawnDefine = define;
            var pos = ParseVector3(define.Pos);
            var dire = ParseVector3(define.Dir);

            // 构造时不应该刷怪，这时Map还没构造完成，应该等到主动调用
            SpawnManager.Map.MonsterManager.NewMonster(define.UnitID, pos, dire, DataHelper.GetUnitDefine(define.UnitID).Name);
        }

        private Vector3 ParseVector3(string str)
        {
            var pointArr = JsonConvert.DeserializeObject<float[]>(str);
            return new Vector3(pointArr[0], pointArr[1], pointArr[2]);
        }
    }


    /// <summary>
    /// 怪物生成管理器
    /// </summary>
    public class SpawnManager
    {
        public Map Map;
        public List<Spawner> Rules = new();

        public SpawnManager(Map map)
        {
            Map = map;

            var rules = DataManager.Instance.SpawnDict.Values.Where(r => r.MapId == Map.MapId);
            foreach (var rule in rules)
            {
                Rules.Add(new(this, rule));
                Log.Information($"加载刷怪配置：{DataHelper.GetMapDefine(rule.MapId).Name}");
            }
        }

        private void Spawn()
        {


            //_map.MonsterManager.NewMonster();
        }
    }
}
