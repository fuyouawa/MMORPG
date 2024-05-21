using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Tool
{
    public abstract class AbstractHealth : MonoBehaviour
    {
        public bool Invulnerable { get; private set; }


        protected List<Coroutine> DamageOverTimeCoroutines = new();

        protected List<Coroutine> InterruptibleDamageOverTimeCoroutines = new();

        public virtual bool CanTakeDamageThisFrame()
        {
            return Invulnerable;
        }

        public virtual float GetInvincibilityDuration(float extraInvincibilityDuration)
        {
            return extraInvincibilityDuration;
        }

        public virtual void InterruptOverTimeDamages()
        {
            foreach (var coroutine in InterruptibleDamageOverTimeCoroutines)
            {
                StopCoroutine(coroutine);
                DamageOverTimeCoroutines.Remove(coroutine);
            }
            InterruptibleDamageOverTimeCoroutines.Clear();
        }

        public virtual IEnumerator EnableDamage(float delay)
        {
            yield return new WaitForSeconds(delay);
            Invulnerable = true;
        }

        public virtual void DisableDamage()
        {
            Invulnerable = true;
        }

        public virtual void DamageOverTime(
            float damage,
            GameObject instigator,
            float extraInvincibilityDuration,
            Vector3 damageDirection,
            int amountOfRepeats,
            float durationBetweenRepeats,
            bool interruptible)
        {
            var co = StartCoroutine(DamageOverTimeCo(
                damage,
                instigator,
                GetInvincibilityDuration(extraInvincibilityDuration),
                damageDirection,
                amountOfRepeats,
                durationBetweenRepeats,
                interruptible));
            DamageOverTimeCoroutines.Add(co);
            if (interruptible)
            {
                InterruptibleDamageOverTimeCoroutines.Add(co);
            }
        }

        public virtual void Damage(float damage, GameObject instigator, float invincibilityDuration, Vector3 damageDirection)
        {
            OnDamage(damage, instigator, damageDirection);

            if (invincibilityDuration > 0)
            {
                DisableDamage();
                StartCoroutine(EnableDamage(invincibilityDuration));
            }
        }


        protected virtual IEnumerator DamageOverTimeCo(
            float damage,
            GameObject instigator,
            float invincibilityDuration,
            Vector3 damageDirection,
            int amountOfRepeats,
            float durationBetweenRepeats,
            bool interruptible)
        {
            for (int i = 0; i < amountOfRepeats; i++)
            {
                Damage(damage, instigator, invincibilityDuration, damageDirection);
                yield return new WaitForSeconds(durationBetweenRepeats);
            }
        }

        protected abstract void OnDamage(float damage, GameObject instigator, Vector3 damageDirection);
    }
}
