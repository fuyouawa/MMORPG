using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.BuffSystem;
using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Fight;
using GameServer.FightSystem;
using GameServer.Tool;
using Serilog;
using GameServer.MapSystem;

namespace GameServer.EntitySystem
{
    public class Actor : Entity
    {
        public string Name { get; protected set; }
        public int Level { get; protected set; }
        public float Speed { get; protected set; }
        public float Hp { get; protected set; }
        public float Mp { get; protected set; }
        public DamageInfo? DamageSourceInfo { get; set; }
        public AttributeManager AttributeManager { get; protected set; }
        public SkillManager SkillManager { get; protected set; }
        public BuffManager BuffManager { get; protected set; }
        public Spell Spell { get; protected set; }

        public Actor(EntityType entityType, int entityId, UnitDefine unitDefine,
            Map map, Vector3 pos, Vector3 dire, string name, int level) 
            : base(entityType, entityId, unitDefine, map, pos, dire)
        {
            Name = name;
            Level = level;
            AttributeManager = new(this);
            SkillManager = new(this);
            BuffManager = new(this);
            Spell = new(this);
        }

        public override void Start()
        {
            base.Start();
            AttributeManager.Start();
            SkillManager.Start();
            BuffManager.Start();

            //var unitDefine = DataHelper.GetUnitDefine(UnitId);
            //Spell.Start();
        }

        public override void Update()
        {
            base.Update();
            SkillManager.Update();
            BuffManager.Update();
        }

        public virtual bool IsDeath()
        {
            return Hp <= 0;
        }

        public virtual void Revive() { }


        public void ChangeHP(float amount)
        {
            Hp += amount;
            if (Hp <= 0) Hp = 0;
            if (Hp > AttributeManager.Final.MaxHp) Hp = AttributeManager.Final.MaxHp;
            EntityAttributeEntrySync(EntityAttributeEntryType.Hp, (int)Hp);
        }

        public void ChangeMp(float amount)
        {
            Mp += amount;
            if (Mp <= 0) Mp = 0;
            if (Mp > AttributeManager.Final.MaxMp) Mp = AttributeManager.Final.MaxMp;
            EntityAttributeEntrySync(EntityAttributeEntryType.Mp, (int)Mp);
        }

        public virtual void OnHurt(DamageInfo info)
        {
            DamageSourceInfo = info;
            Map.PlayerManager.Broadcast(new EntityHurtResponse{ Info = info }, this);
            ChangeHP(-info.Amount);
            Log.Debug($"Actor:{Name}({EntityId})受到{info.AttackerId}的攻击, 扣除{info.Amount}血量, 剩余血量:{Hp}!");
        }

        private EntityAttributeEntry ConstructAttributeEntry<T>(EntityAttributeEntryType type, T value)
        {
            var entry = new EntityAttributeEntry()
            {
                Type = type,
            };
            switch (value)
            {
                case int intVal:
                    entry.Int32 = intVal;
                    break;
                case float floatVal:
                    entry.Float = floatVal;
                    break;
                case string stringVal:
                    entry.String = stringVal;
                    break;
                default:
                    Log.Error("[Actor.SyncAttributeEntry]无效的类型");
                    break;
            }
            return entry;
        }

        private void EntityAttributeEntrySync<T>(EntityAttributeEntryType type, T value)
        {
            var resp = new EntityAttributeSyncResponse()
            {
                EntityId = EntityId,
            };
            resp.Entrys.Add(ConstructAttributeEntry(type, value));
            Map.PlayerManager.Broadcast(resp, this);
        }

        private void EntityAttributeEntrySync(params EntityAttributeEntry[] entries)
        {
            var resp = new EntityAttributeSyncResponse()
            {
                EntityId = EntityId,
            };
            resp.Entrys.Add(entries);
            Map.PlayerManager.Broadcast(resp, this);
        }
    }
}
