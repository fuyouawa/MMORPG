using DamageNumbersPro;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public enum LevelDamageNumberType
    {
        Monster
    }

    public class LevelManager : MonoSingleton<LevelManager>
    {
        public DamageNumber MonsterDamageNumber;
        public DamageNumber OtherDamageNumber;

        public void TakeDamage(LevelDamageNumberType type, Vector3 position, float damage, Transform follow = null)
        {
            var damageNumber = type switch
            {
                LevelDamageNumberType.Monster => MonsterDamageNumber,
                _ => OtherDamageNumber
            };
            var obj = follow != null
                ? damageNumber.Spawn(position, damage, follow)
                : damageNumber.Spawn(position, damage);

        }
    }
}
