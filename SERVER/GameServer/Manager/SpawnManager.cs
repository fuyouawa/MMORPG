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
using System.Data;

namespace GameServer.Manager
{
    /// <summary>
    /// 怪物生成器
    /// </summary>
    public class Spawner
    {
        public SpawnManager SpawnManager;
        public SpawnDefine SpawnDefine;
        public Monster? Monster;

        private bool _reviving;      // 复活中
        private float _reviveTime;   // 复活时间

        public Spawner(SpawnManager manager, SpawnDefine define)
        {
            SpawnManager = manager;
            SpawnDefine = define;
        }

        public void Start()
        {
            var pos = ParseVector3(SpawnDefine.Pos);
            var dire = ParseVector3(SpawnDefine.Dir);
            Monster = SpawnManager.Map.MonsterManager.NewMonster(SpawnDefine.UnitID, pos, dire, DataHelper.GetUnitDefine(SpawnDefine.UnitID).Name);
        }

        public void Update()
        {
            if (Monster == null || !Monster.IsDeath()) return;
            if (!_reviving)
            {
                _reviveTime = Time.time + SpawnDefine.Period;
                _reviving = true;
            }
            if (_reviveTime <= Time.time)
            {
                Monster.Revive();
                _reviving = false;
            }
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
        private List<Spawner> _spawners = new();

        public SpawnManager(Map map)
        {
            Map = map;
        }

        public void Start()
        {
            var rules = DataManager.Instance.SpawnDict.Values.Where(r => r.MapId == Map.MapId);
            foreach (var rule in rules)
            {
                var spawner = new Spawner(this, rule);
                _spawners.Add(spawner);
                spawner.Start();
                Log.Information($"加载刷怪配置：{DataHelper.GetMapDefine(rule.MapId).Name}");
            }
        }

        public void Update()
        {
            foreach (var spawner in _spawners)
            {
                spawner.Update();
            }
        }
    }
}
