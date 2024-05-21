using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Tool
{
    public class Health : MonoBehaviour
    {
        public virtual float CurrentHealth { get; private set; } = 100;

        public virtual bool CanTakeDamageThisFrame()
        {
            return true;
        }

        public virtual void DamageOverTime(
            float damage,
            GameObject instigator,
            float invincibilityDuration,
            Vector3 damageDirection,
            int amountOfRepeats,
            float durationBetweenRepeats,
            bool damageOverTimeInterruptible)
        {
        }

        public virtual void Damage(float damage, GameObject instigator, float invincibilityDuration, Vector3 damageDirection)
        {
        }
    }
}
