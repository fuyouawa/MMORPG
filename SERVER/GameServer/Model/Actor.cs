using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Entity;
using GameServer.Fight;
using GameServer.Manager;
using GameServer.Tool;

namespace GameServer.Model
{
    public class Actor : Entity
    {
        public string Name;
        public float Speed;
        public int Level;
        public AttributeManager AttributeManager;
        public SkillManager SkillManager;
        public Spell Spell;

        public Actor(EntityType entityType, int entityId, int unitId, 
            Map map, string name) : base(entityType, entityId, unitId, map)
        {
            Name = name;
            var unitDefine = DataHelper.GetUnitDefine(unitId);
            Speed = unitDefine.Speed;

            AttributeManager = new(this);
            SkillManager = new(this);
            Spell = new(this);
        }

        public override void Start()
        {
            base.Start();
            AttributeManager.Start();
            SkillManager.Start();
            //Spell.Start();
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
