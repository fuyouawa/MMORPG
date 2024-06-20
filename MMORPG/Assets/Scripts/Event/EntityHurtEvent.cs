using MMORPG.Common.Proto.Fight;
 using MMORPG.Game;

 namespace MMORPG.Event
{
    public class EntityHurtEvent
    {
        public EntityView Wounded { get; }
        public EntityView Attacker { get; }
        public int Amount { get; }
        public DamageType DamageType { get; }
        public bool IsCrit { get; }
        public bool IsMiss { get; }

        public EntityHurtEvent(
            EntityView wounded,
            EntityView attacker,
            int amount,
            DamageType damageType,
            bool isCrit,
            bool isMiss)

        {
            Wounded = wounded;
            Attacker = attacker;
            Amount = amount;
            DamageType = damageType;
            IsCrit = isCrit;
            IsMiss = isMiss;
        }
    }
}
