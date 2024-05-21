using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace MMORPG.Tool
{
    [AddFeedbackMenu("Area/Damage Area")]
    public class FeedbackDamageArea : Feedback
    {
        public enum DamageAreaShapes { Box, Sphere }
        public enum DamageAreaModes { Generated, Existing }

        [FoldoutGroup("Damage Area")]
        public DamageAreaModes DamageAreaMode = DamageAreaModes.Generated;

        [FoldoutGroup("Damage Area")]
        [ShowIf("DamageAreaMode", DamageAreaModes.Generated)]
        public DamageAreaShapes DamageAreaShape = DamageAreaShapes.Box;

        [FoldoutGroup("Damage Area")]
        [ShowIf("DamageAreaMode", DamageAreaModes.Generated)]
        [LabelText("Debug In Editor")]
        public bool DebugDamageAreaInEditor = false;

        [FoldoutGroup("Damage Area")]
        [ShowIf("DamageAreaMode", DamageAreaModes.Generated)]
        public Vector3 AreaOffset = Vector3.zero;

        [FoldoutGroup("Damage Area")]
        [ShowIf("DamageAreaMode", DamageAreaModes.Generated)]
        public Vector3 AreaSize = Vector3.one;

        [FoldoutGroup("Damage Area")]
        [ShowIf("DamageAreaMode", DamageAreaModes.Existing)]
        public DamageOnTouch ExistingDamageArea;


        [FoldoutGroup("Damage Area Timing")]
        public float InitialDelay = 0f;
        [FoldoutGroup("Damage Area Timing")]
        public float ActiveDuration = 1f;

        [FoldoutGroup("Damage Caused")]
        public LayerMask TargetLayerMask;
        [FoldoutGroup("Damage Caused")]
        public float MinDamageCaused = 10f;
        [FoldoutGroup("Damage Caused")]
        public float MaxDamageCaused = 10f;
        [FoldoutGroup("Damage Caused")]
        public DamageOnTouch.KnockBackStyles KnockBack;
        [FoldoutGroup("Damage Caused")]
        public Vector3 KnockBackForce = new(10, 2, 0);
        [FoldoutGroup("Damage Caused")]
        public DamageOnTouch.KnockBackDirections KnockBackDirection = DamageOnTouch.KnockBackDirections.BasedOnOwnerPosition;
        [FoldoutGroup("Damage Caused")]
        public float InvincibilityDuration = 0.5f;

        protected Collider _damageAreaCollider;
        protected bool _attackInProgress = false;
        protected BoxCollider _boxCollider;
        protected SphereCollider _sphereCollider;
        protected DamageOnTouch _damageOnTouch;
        protected GameObject _damageArea;

        protected override void OnFeedbackInit()
        {
            if (_damageArea == null)
            {
                CreateDamageArea();
                DisableDamageArea();
            }
            if (Owner != null)
            {
                _damageOnTouch.Owner = Owner.gameObject;
            }
        }


        /// <summary>
        /// Creates the damage area.
        /// </summary>
        protected virtual void CreateDamageArea()
        {
            if ((DamageAreaMode == DamageAreaModes.Existing) && (ExistingDamageArea != null))
            {
                _damageArea = ExistingDamageArea.gameObject;
                _damageAreaCollider = _damageArea.gameObject.GetComponent<Collider>();
                _damageOnTouch = ExistingDamageArea;
                return;
            }

            _damageArea = new GameObject();
            _damageArea.name = nameof(DamageOnTouch);
            _damageArea.transform.position = Transform.position;
            _damageArea.transform.rotation = Transform.rotation;
            _damageArea.transform.SetParent(Transform);
            _damageArea.transform.localScale = Vector3.one;
            _damageArea.layer = Owner.gameObject.layer;

            if (DamageAreaShape == DamageAreaShapes.Box)
            {
                _boxCollider = _damageArea.AddComponent<BoxCollider>();
                _boxCollider.center = AreaOffset;
                _boxCollider.size = AreaSize;
                _damageAreaCollider = _boxCollider;
                _damageAreaCollider.isTrigger = true;
            }
            if (DamageAreaShape == DamageAreaShapes.Sphere)
            {
                _sphereCollider = _damageArea.AddComponent<SphereCollider>();
                _sphereCollider.transform.position = Transform.position + Transform.rotation * AreaOffset;
                _sphereCollider.radius = AreaSize.x / 2;
                _damageAreaCollider = _sphereCollider;
                _damageAreaCollider.isTrigger = true;
            }

            if ((DamageAreaShape == DamageAreaShapes.Box) || (DamageAreaShape == DamageAreaShapes.Sphere))
            {
                Rigidbody rigidBody = _damageArea.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }

            _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();
            _damageOnTouch.SetGizmoSize(AreaSize);
            _damageOnTouch.SetGizmoOffset(AreaOffset);
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.MinDamageCaused = MinDamageCaused;
            _damageOnTouch.MaxDamageCaused = MaxDamageCaused;
            _damageOnTouch.DamageDirectionMode = DamageOnTouch.DamageDirections.BasedOnOwnerPosition;
            _damageOnTouch.DamageCausedKnockBackType = KnockBack;
            _damageOnTouch.DamageCausedKnockbackForce = KnockBackForce;
            _damageOnTouch.DamageCausedKnockBackDirection = KnockBackDirection;
            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
        }

        protected override void OnFeedbackPlay()
        {
            StartCoroutine(ProcessDamageCo());
        }

        protected virtual IEnumerator ProcessDamageCo()
        {
            if (_attackInProgress) { yield break; }

            _attackInProgress = true;
            yield return new WaitForSeconds(InitialDelay);
            EnableDamageArea();
            yield return new WaitForSeconds(ActiveDuration);
            DisableDamageArea();
            _attackInProgress = false;
        }

        protected virtual void EnableDamageArea()
        {
            if (_damageAreaCollider != null)
            {
                _damageAreaCollider.enabled = true;
            }
        }

        protected virtual void DisableDamageArea()
        {
            if (_damageAreaCollider != null)
            {
                _damageAreaCollider.enabled = false;
            }
        }

#if UNITY_EDITOR
        public override void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying && Owner != null)
            {
                DrawGizmos();
            }
        }

        protected virtual void DrawGizmos()
        {
            if (DebugDamageAreaInEditor)
            {
                if (DamageAreaMode == DamageAreaModes.Existing)
                {
                    return;
                }

                if (DamageAreaShape == DamageAreaShapes.Box)
                {
                    Gizmos.DrawWireCube(Transform.position + AreaOffset, AreaSize);
                }

                if (DamageAreaShape == DamageAreaShapes.Sphere)
                {
                    Gizmos.DrawWireSphere(Transform.position + AreaOffset, AreaSize.x / 2);
                }
            }
        }
#endif

        /// <summary>
        /// On disable we set our flag to false
        /// </summary>
        protected virtual void OnDisable()
        {
            _attackInProgress = false;
        }
    }
}
