using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Entity;
using Common.Proto.Fight;
using GameServer.Fight;
using GameServer.Manager;
using GameServer.Tool;
using Serilog;

namespace GameServer.Model
{
    public class Actor : Entity
    {
        public string Name { get; private set; }
        public int Level { get; private set; }
        public float Speed { get; private set; }
        public float Hp { get; private set; }
        public float Mp { get; private set; }
        public AttributeManager AttributeManager { get; private set; }
        public SkillManager SkillManager { get; private set; }
        public Spell Spell { get; private set; }

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

        public virtual bool IsDeath()
        {
            return Hp <= 0;
        }

        public void ChangeHP(float amount)
        {
            Hp += amount;
            if (Hp <= 0) Hp = 0;
            if (Hp > AttributeManager.Final.MaxHp) Hp = AttributeManager.Final.MaxHp;
            EntityAttributeEntrySync(EntityAttributeEntryType.Hp, Hp);
        }

        public void ChangeMp(float amount)
        {
            Mp += amount;
            if (Mp <= 0) Mp = 0;
            if (Mp > AttributeManager.Final.MaxMp) Mp = AttributeManager.Final.MaxMp;
            EntityAttributeEntrySync(EntityAttributeEntryType.Mp, Mp);
        }

        public virtual void OnDamage(DamageInfo damageInfo)
        {
            var resp = new DamageResponse();
            resp.Damages.Add(damageInfo);
            Map.PlayerManager.Broadcast(resp, this);
            ChangeHP(-damageInfo.Amount);
        }

        private void EntityAttributeEntrySync<T>(EntityAttributeEntryType type, T value)
        {
            var resp = new EntityAttributeSyncResponse()
            {
                EntityId = EntityId,
            };
            var entry = new EntityAttributeEntry()
            {
                Type = type,
            };
            if (typeof(T) == typeof(int))
            {
                entry.Int32 = Convert.ToInt32(value);
            }
            else if (typeof(T) == typeof(float))
            {
                entry.Float = Convert.ToSingle(value);
            }
            else if (typeof(T) == typeof(string))
            {
                entry.String = Convert.ToString(value);
            }
            else
            {
                Log.Error("[Actor.SyncAttributeEntry]无效的类型");
            }
            resp.Entrys.Add(entry);
            Map.PlayerManager.Broadcast(resp, this);
        }
    }
}
