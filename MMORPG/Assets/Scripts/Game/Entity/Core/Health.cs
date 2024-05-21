using MMORPG.Tool;
using UnityEngine;

namespace MMORPG.Game
{
    public class Health : AbstractHealth
    {
        public EntityView Entity;

        protected override void OnDamage(float damage, GameObject instigator, Vector3 damageDirection)
        {
            Debug.Log($"{Entity.name}受到{instigator.gameObject}的攻击, 造成{damage}点伤害");
        }
    }
}
