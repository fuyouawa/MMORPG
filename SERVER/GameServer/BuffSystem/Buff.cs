using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.EntitySystem;
using GameServer.Tool;

namespace GameServer.BuffSystem
{
    public class Buff
    {
        public LinkedListNode<Buff> Node;
        public int BuffId { get; }
        public BuffManager BuffManager { get; protected set; }
        public Actor? Caster { get; protected set; }
        public float Duration { get; protected set; }     // s
        public int Layer { get; protected set; }
        public int Level { get; protected set; }

        private float _endTime;

        public Buff(int buffId, BuffManager buffManager, Actor? caster, float duration)
        {
            BuffId = buffId;
            BuffManager = buffManager;
            Caster = caster;
            Duration = duration;
            _endTime = Time.time + Duration;
        }

        public virtual void Start() { }

        public virtual void Update()
        {
            if (Time.time >= _endTime)
            {
                BuffManager.RemoveBuff(this);
            }
        }

        public virtual void Exit() { }
    }
}
