﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.EntitySystem;
using GameServer.Manager;
using MMORPG.Common.Proto.Entity;

namespace GameServer.BuffSystem
{
    public class BuffManager
    {
        private Dictionary<int, LinkedList<Buff>> _buffDict = new();

        public Actor OwnerActor {get; private set; }
        public FlagStates FlagState { get; private set; }

        public BuffManager(Actor ownerActor)
        {
            OwnerActor = ownerActor;
        }

        public void Start()
        {

        }

        public void Update()
        {
            foreach (var list in _buffDict.Values)
            {
                var cur = list.First;
                while (cur != null)
                {
                    var next = cur.Next;
                    cur.Value.Update();
                    cur = next;
                }
            }
        }

        public void AddBuff(int buffId, Actor? caster = null)
        {
            if (!DataManager.Instance.BuffDict.TryGetValue(buffId, out var buffDefine)) return;
            
            if (!_buffDict.TryGetValue(buffId, out var buffList))
            {
                buffList = new LinkedList<Buff>();
                _buffDict[buffId] = buffList;
            }

            Buff? buff = null;
            if (buffDefine.Type == "Attribute")
            {
                buff = new AttributeBuff(buffId, this, caster, buffDefine.Duration, buffDefine.Modifier);
            }
            if (buffDefine.Type == "State")
            {
                buff = new StateBuff(buffId, this, caster, buffDefine.Duration, buffDefine.Modifier);
            }

            if (buff != null)
            {
                buff.Node = buffList.AddLast(buff);
                buff.Start();
                if (Math.Abs(buffDefine.Duration) <= 1e-6) RemoveBuff(buff);
            }
        }

        public void RemoveBuff(Buff buff)
        {
            if (!_buffDict.TryGetValue(buff.BuffId, out var buffList)) return;
            buff.Exit();
            buffList.Remove(buff.Node);
        }
    }
}
