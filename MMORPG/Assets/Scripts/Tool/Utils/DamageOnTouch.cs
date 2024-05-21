using System;
using UnityEngine;

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

        public enum KnockBackStyles
        {
            NoKnockback,
            AddForce
        }

        public enum KnockBackDirections
        {
            BasedOnOwnerPosition,
            BasedOnSpeed,
            BasedOnDirection,
            BasedOnScriptDirection
        }

        public enum DamageDirections
        {
            BasedOnOwnerPosition,
            BasedOnVelocity,
            BasedOnScriptDirection
        }

        public GameObject Owner;
        public TriggerAndCollisionMask TriggerFilter;
        public LayerMask TargetLayerMask;
        public float MinDamageCaused;
        public float MaxDamageCaused;
        public DamageDirections DamageDirectionMode;
        public KnockBackStyles DamageCausedKnockBackType;
        public Vector3 DamageCausedKnockbackForce;
        public KnockBackDirections DamageCausedKnockBackDirection;
        public float InvincibilityDuration;

        public void SetGizmoSize(Vector3 areaSize)
        {
        }

        public void SetGizmoOffset(Vector3 areaOffset)
        {
        }

        public void IgnoreGameObject(GameObject characterControllerGameObject)
        {
        }
    }
}
