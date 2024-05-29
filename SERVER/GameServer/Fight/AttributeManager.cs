using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Fight
{
    public class AttributeManager
    {
        public AttributeData Basic { get; private set; } = new();   // 基础总和
        public AttributeData Equip { get; private set; } = new();   // 装备总和
        public AttributeData Buff { get; private set; } = new();    // buff总和
        public AttributeData Final { get; private set; } = new();   // 最终总和
        public int Level {get; private set; }

        public AttributeManager(UnitDefine define)
        {
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
                Agi = define.Agi
            };

            var growth = new AttributeData()
            {
                Str = define.Gstr * Level - 1,
                Int = define.Gint * Level - 1,
                Agi = define.Gagi * Level - 1,
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
