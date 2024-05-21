using System;
using UnityEngine;

namespace MMORPG.Game
{
    //TODO DamageOnTouch
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

        public enum KnockbackStyles
        {
            NoKnockback,
            AddForce
        }

        public enum KnockbackDirections
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

        public PlayerBrain Brain { get; set; }
        public TriggerAndCollisionMask TriggerFilter = TriggerAndCollisionMask.All;
        public LayerMask TargetLayerMask;
        public float MinDamageCaused;
        public float MaxDamageCaused;
        public DamageDirections DamageDirectionMode;
        public KnockbackStyles DamageCausedKnockbackType;
        public Vector3 DamageCausedKnockbackForce;
        public KnockbackDirections DamageCausedKnockbackDirection;
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
