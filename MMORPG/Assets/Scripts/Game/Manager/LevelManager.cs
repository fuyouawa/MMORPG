using DamageNumbersPro;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        public DamageNumber DamageNumber;
        public DamageNumber DamageCritNumber;
        public DamageNumber DamageText;

        public void TakeDamage(Vector3 position, float damage, bool isCrit = false, Transform follow = null)
        {
            var damageNumber = isCrit ? DamageCritNumber : DamageNumber;
            var obj = follow != null
                ? damageNumber.Spawn(position, damage, follow)
                : damageNumber.Spawn(position, damage);
        }

        public void TakeText(Vector3 position, string text, Transform follow = null)
        {
            var obj = follow != null
                ? DamageText.Spawn(position, text, follow)
                : DamageText.Spawn(position, text);
        }
    }
}
