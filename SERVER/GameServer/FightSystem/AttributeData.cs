namespace GameServer.FightSystem
{
    public class AttributeData
    {
        public float Speed { get; set; }
        /// <summary>
        /// 生命上限
        /// </summary>
        public float MaxHp { get; set; }
        /// <summary>
        /// 魔法上限
        /// </summary>
        public float MaxMp { get; set; }
        /// <summary>
        /// 物攻
        /// </summary>
        public float Ad { get; set; }
        /// <summary>
        /// 魔攻
        /// </summary>
        public float Ap { get; set; }
        /// <summary>
        /// 物防
        /// </summary>
        public float Def { get; set; }
        /// <summary>
        /// 魔防
        /// </summary>
        public float Mdef { get; set; }
        /// <summary>
        /// 暴击率
        /// </summary>
        public float Cri { get; set; }
        /// <summary>
        /// 暴击伤害
        /// </summary>
        public float Crd { get; set; }
        /// <summary>
        /// 力量
        /// </summary>
        public float Str { get; set; }
        /// <summary>
        /// 智力
        /// </summary>
        public float Int { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public float Agi { get; set; }

        /// <summary>
        /// 命中率
        /// </summary>
        public float HitRate { get; set; }
        /// <summary>
        /// 闪避率
        /// </summary>
        public float DodgeRate { get; set; }

        public AttributeData()
        {
            Reset();
        }

        public void Add(AttributeData other)
        {
            Speed += other.Speed;
            MaxHp += other.MaxHp;
            MaxMp += other.MaxMp;
            Ad += other.Ad;
            Ap += other.Ap;
            Def += other.Def;
            Mdef += other.Mdef;
            Cri += other.Cri;
            Crd += other.Crd;
            Str += other.Str;
            Int += other.Int;
            Agi += other.Agi;
            HitRate += other.HitRate;
            DodgeRate += other.DodgeRate;
        }

        public void Sub(AttributeData other)
        {
            Speed -= other.Speed;
            MaxHp -= other.MaxHp;
            MaxMp -= other.MaxMp;
            Ad -= other.Ad;
            Ap -= other.Ap;
            Def -= other.Def;
            Mdef -= other.Mdef;
            Cri -= other.Cri;
            Crd -= other.Crd;
            Str -= other.Str;
            Int -= other.Int;
            Agi -= other.Agi;
            HitRate -= other.HitRate;
            DodgeRate -= other.DodgeRate;
        }

        public void Reset()
        {
            Speed = 0;
            MaxHp = 0;
            MaxMp = 0;
            Ad = 0;
            Ap = 0;
            Def = 0;
            Mdef = 0;
            Cri = 0;
            Str = 0;
            Int = 0;
            Agi = 0;
            HitRate = 0;
            DodgeRate = 0;
        }
    }
}
