using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.Fight;
using GameServer.Manager;

namespace GameServer.Model
{
    public enum ActorState
    {
        None = 0,
        Idle,
        Move,
        Attack,
    }

    public class Actor : Entity
    {
        public ActorState State;
        public string Name;
        public int Level;
        public AttributeManager AttributeManager;
        public SkillManager SkillManager;

        public Actor(Map map, string name)
        {
            Map = map;
            Name = name;
            SkillManager = new(this);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();
            SkillManager.Update();
        }

        public bool IsDeath()
        {
            return false;
            //return Hp <= 0;
        }
    }
}
