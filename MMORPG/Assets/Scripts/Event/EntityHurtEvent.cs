using MMORPG.Common.Proto.Fight;
 using MMORPG.Game;

 namespace MMORPG.Event
{
    public class EntityHurtEvent
    {
        public EntityView Wounded { get; }
        public EntityView Attacker { get; }
        public DamageInfo DamageInfo { get; }

        public EntityHurtEvent(
            EntityView wounded,
            EntityView attacker,
            DamageInfo info)

        {
            Wounded = wounded;
            Attacker = attacker;
            DamageInfo = info;
        }
    }
}
