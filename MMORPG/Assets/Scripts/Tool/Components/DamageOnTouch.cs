using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using static MMORPG.Tool.DamageOnTouch;

namespace MMORPG.Tool
{
    public class DamageOnTouch : MonoBehaviour
    {
        [Flags]
        public enum TriggerAndCollisionMask
        {
            IgnoreAll = 0,
            OnTriggerEnter = 1 << 0,
            OnTriggerStay = 1 << 1,

            All = OnTriggerEnter | OnTriggerStay,
        }

        public enum DamageDirections
        {
            BasedOnOwnerPosition,
            BasedOnVelocity
        }

        [Title("Binding")]
        public GameObject Owner;

        [Title("Settings")]
        public TriggerAndCollisionMask TriggerFilter = TriggerAndCollisionMask.All;
        public LayerMask TargetLayerMask;

        [Title("Damage Caused")]
        public float MinDamageCaused = 10f;
        public float MaxDamageCaused = 10f;
        public DamageDirections DamageDirectionMode;
        public float InvincibilityDuration;
        public List<GameObject> IgnoredGameObjects = new();

        [Title("Damage over time")]
        public bool RepeatDamageOverTime = false;
        [ShowIf("RepeatDamageOverTime")]
        public int AmountOfRepeats = 3;
        [ShowIf("RepeatDamageOverTime")]
        public float DurationBetweenRepeats = 1f;
        [ShowIf("RepeatDamageOverTime")]
        public bool DamageOverTimeInterruptible = true;

        [Title("Events")]
        public UnityEvent<Health> HitDamageableEvent = new();
        public UnityEvent<Collider> HitNonDamageableEvent = new();
        public UnityEvent<Collider> HitAnythingEvent = new();

        protected Health CollidingHealth;
        protected Vector3 DamageDirection;
        protected Vector3 LastDamagePosition;
        protected BoxCollider BoxCollider;
        protected SphereCollider SphereCollider;

        private Color _gizmosColor;
        private Vector3 _lastDamagePosition;

        public virtual void Initialize()
        {
            TryGetComponent(out BoxCollider);
            TryGetComponent(out SphereCollider);

            _gizmosColor = Color.red;
            _gizmosColor.a = 0.25f;
        }

        public void IgnoreGameObject(GameObject obj)
        {
            if (!IgnoredGameObjects.Contains(obj))
            {
                IgnoredGameObjects.Add(obj);
            }
        }

        protected virtual void Update()
        {
            ComputeVelocity();
        }

        protected virtual void DetermineDamageDirection()
        {
            switch (DamageDirectionMode)
            {
                case DamageDirections.BasedOnOwnerPosition:
                    if (Owner == null)
                    {
                        Owner = gameObject;
                    }
                    DamageDirection = CollidingHealth.transform.position - Owner.transform.position;
                    break;
                case DamageDirections.BasedOnVelocity:
                    DamageDirection = transform.position - LastDamagePosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DamageDirection = DamageDirection.normalized;
        }

        protected virtual void ComputeVelocity()
        {
            if (Time.deltaTime != 0f)
            {
                if (Vector3.Distance(_lastDamagePosition, transform.position) > 0.5f)
                {
                    _lastDamagePosition = transform.position;
                }
            }
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay)) return;
            Colliding(collider);
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerEnter)) return;
            Colliding(collider);
        }

        /// <summary>
        /// When colliding, we apply the appropriate damage
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void Colliding(Collider collider)
        {
            if (!EvaluateAvailability(collider.gameObject))
            {
                return;
            }

            CollidingHealth = collider.gameObject.GetComponentInChildren<Health>();

            // if what we're colliding with is damageable
            if (CollidingHealth != null)
            {
                if (CollidingHealth.CurrentHealth > 0)
                {
                    OnCollideWithDamageable(CollidingHealth);
                }
            }
            else // if what we're colliding with can't be damaged
            {
                OnCollideWithNonDamageable();
                HitNonDamageableEvent?.Invoke(collider);
            }

            OnAnyCollision(collider);
            HitAnythingEvent?.Invoke(collider);
        }

        protected virtual bool EvaluateAvailability(GameObject collider)
        {
            if (!isActiveAndEnabled) { return false; }

            if (IgnoredGameObjects.Contains(collider)) { return false; }

            if (TargetLayerMask.Contain(collider.layer)) { return false; }

            return true;
        }

        /// <summary>
        /// Describes what happens when colliding with a damageable object
        /// </summary>
        /// <param name="health">Health.</param>
        protected virtual void OnCollideWithDamageable(Health health)
        {
            CollidingHealth = health;

            if (health.CanTakeDamageThisFrame())
            {
                HitDamageableEvent?.Invoke(CollidingHealth);

                // we apply the damage to the thing we've collided with
                float randomDamage =
                    UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

                DetermineDamageDirection();

                if (RepeatDamageOverTime)
                {
                    CollidingHealth.DamageOverTime(
                        randomDamage,
                        gameObject,
                        InvincibilityDuration,
                        DamageDirection,
                        AmountOfRepeats,
                        DurationBetweenRepeats,
                        DamageOverTimeInterruptible);
                }
                else
                {
                    CollidingHealth.Damage(
                        randomDamage,
                        gameObject,
                        InvincibilityDuration,
                        DamageDirection);
                }
            }
        }


        protected virtual void OnCollideWithNonDamageable()
        {
        }

        protected virtual void OnAnyCollision(Collider other)
        {
        }

#if UNITY_EDITOR

        /// <summary>
        /// draws a cube or sphere around the damage area
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = _gizmosColor;

            if (BoxCollider != null)
            {
                GizmosHelper.DrawCube(transform, BoxCollider.center, BoxCollider.size, !BoxCollider.enabled);
            }

            if (SphereCollider != null)
            {
                if (SphereCollider.enabled)
                    Gizmos.DrawSphere(transform.position, SphereCollider.radius);
                else
                    Gizmos.DrawWireSphere(transform.position, SphereCollider.radius);
            }
        }
#endif
    }
}
