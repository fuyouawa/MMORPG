using MMORPG.Tool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class Health : AbstractHealth
    {
        public EntityView Entity;
        [Title("Damage Number")]
        public Transform DamageNumberPoint;

        protected override void OnHurt(float damage, GameObject instigator, Vector3 damageDirection)
        {
            Debug.Log($"{Entity.name}受到{instigator.gameObject}的攻击, 造成{damage}点伤害");
            LevelManager.Instance.TakeDamage(LevelDamageNumberType.Monster, DamageNumberPoint.position, damage);
        }
    }
}
