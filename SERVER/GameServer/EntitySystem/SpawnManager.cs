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
using GameServer.MapSystem;
using GameServer.Manager;

namespace GameServer.EntitySystem
{
    /// <summary>
    /// 实体生成器
    /// </summary>
    public class Spawner
    {
        public SpawnManager SpawnManager;
        public SpawnDefine SpawnDefine;
        public Actor? Actor;

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

            var unitDefine = DataHelper.GetUnitDefine(SpawnDefine.UnitID);
            if (unitDefine.Kind == "Monster")
            {
                Actor = SpawnManager.Map.MonsterManager.NewMonster(SpawnDefine.UnitID, pos, dire, unitDefine.Name);
            }
            else if (unitDefine.Kind == "Npc")
            {
                Actor = SpawnManager.Map.NpcManager.NewNpc(SpawnDefine.UnitID, pos, dire, unitDefine.Name);
            }
        }

        public void Update()
        {
            if (Actor == null || !Actor.IsDeath()) return;
            if (!_reviving)
            {
                _reviveTime = Time.time + SpawnDefine.Period;
                _reviving = true;
            }
            if (_reviveTime <= Time.time)
            {
                Actor.Revive();
                _reviving = false;
            }
        }

        private Vector3 ParseVector3(string str)
        {
            var pointArr = DataHelper.ParseJson<float[]>(str);
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
