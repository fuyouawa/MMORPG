using MMORPG.Game;

namespace MMORPG.Event
{
    /// <summary>
    /// 这个事件只可能是当前玩家触发
    /// </summary>
    public class PlayerChangeWeaponEvent
    {
        public Weapon NewWeapon;
        public bool Combo;

        public PlayerChangeWeaponEvent(Weapon newWeapon, bool combo)
        {
            NewWeapon = newWeapon;
            Combo = combo;
        }
    }
}
