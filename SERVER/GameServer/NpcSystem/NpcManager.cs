﻿using MMORPG.Common.Proto.Entity;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.MapSystem;
using GameServer.EntitySystem;

namespace GameServer.NpcSystem
{
    /// <summary>
    /// Npc管理器
    /// 负责管理地图内的所有Npc
    /// 线程安全
    /// </summary>
    public class NpcManager
    {
        private Dictionary<int, Npc> _npcDict = new();
        private Map _map;

        public NpcManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {

        }

        public void Update()
        {
            foreach (var npc in _npcDict.Values)
            {
                npc.Update();
            }
        }

        public Npc NewNpc(int unitId, Vector3 pos, Vector3 dire, string name)
        {
            var npc = new Npc(EntityManager.Instance.NewEntityId(), unitId, _map, name, pos, dire);
            EntityManager.Instance.AddEntity(npc);

            lock (_npcDict)
            {
                _npcDict.Add(npc.EntityId, npc);
            }
            _map.EntityEnter(npc);

            npc.Start();
            return npc;
        }

    }
}