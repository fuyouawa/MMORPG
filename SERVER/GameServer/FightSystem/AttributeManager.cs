using GameServer.EntitySystem;
using GameServer.Manager;
using GameServer.Tool;

namespace GameServer.FightSystem
{
    public class AttributeManager
    {
        public AttributeData Basic { get; private set; } = new();   // 基础总和
        public AttributeData Equip { get; private set; } = new();   // 装备总和
        public AttributeData Buff { get; private set; } = new();    // buff总和
        public AttributeData Final { get; private set; } = new();   // 最终总和
        public Actor OwnerActor { get; private set; }
        
        public AttributeManager(Actor ownerActor)
        {
            OwnerActor = ownerActor;
        }

        public void Start()
        {
            Recalculate();
        }

        public void Recalculate()
        {
            Basic.Reset();
            //Equip.Reset();
            //Buff.Reset();
            Final.Reset();

            var define = OwnerActor.UnitDefine;
            var level = OwnerActor.Level;
            var initial = new AttributeData()
            {
                Speed = define.Speed,
                MaxHp = define.MaxHp,
                MaxMp = define.MaxMp,
                Ad = define.Ad,
                Ap = define.Ap,
                Def = define.Def,
                Mdef = define.Mdef,
                Cri = define.Cri,
                Crd = define.Crd,
                Con = define.Con,
                Str = define.Str,
                Int = define.Int,
                Agi = define.Agi,
                HitRate = define.HitRate,
                DodgeRate = define.DodgeRate,
            };

            var growth = new AttributeData()
            {
                Con = define.Gcon * (level - 1),
                Str = define.Gstr * (level - 1),
                Int = define.Gint * (level - 1),
                Agi = define.Gagi * (level - 1),
            };

            Basic.Add(initial);
            Basic.Add(growth);

            // 装备和buff

            Final.Add(Basic);
            Final.Add(Equip);
            Final.Add(Buff);

            // 计算附加属性
            var extra = new AttributeData()
            {
                MaxHp = Final.Con * 32f,
                MaxMp = Final.Int * 24f,
                Ad = Final.Str * 4f,
                Ap = Final.Int * 3f,
                Def = Final.Con * 1.8f + Final.Str * 0.6f,
                Mdef = Final.Con * 1.2f + Final.Int * 0.6f,
            };

            Final.Add(extra);
        }
    }
}
