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
        public Actor Actor { get; private set; }
        
        public AttributeManager(Actor actor)
        {
            Actor = actor;
        }

        public void Start()
        {
            var define = DataManager.Instance.UnitDict[Actor.UnitId];
            var level = Actor.Level;
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
                Str = define.Str,
                Int = define.Int,
                Agi = define.Agi,
                HitRate = define.HitRate,
                DodgeRate = define.DodgeRate,
            };

            var growth = new AttributeData()
            {
                Str = define.Gstr * level - 1,
                Int = define.Gint * level - 1,
                Agi = define.Gagi * level - 1,
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
                MaxHp = Final.Str * 5,
                Ap = Final.Int * 1.5f,
            };

            Final.Add(extra);
        }
    }
}
