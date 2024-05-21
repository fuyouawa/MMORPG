using System.Collections;
using Sirenix.OdinInspector;
using System.Diagnostics;
using MMORPG.Tool;
using UnityEngine;
using UnityEngine.Serialization;

namespace MMORPG.Game
{
    public class MeleeWeapon : Weapon
    {
        public enum MeleeDamageAreaShapes { Box, Sphere }
        public enum MeleeDamageAreaModes { Generated, Existing }

        [FoldoutGroup("Damage Area")]
        public MeleeDamageAreaModes MeleeDamageAreaMode = MeleeDamageAreaModes.Generated;

        [FoldoutGroup("Damage Area")]
        [ShowIf("MeleeDamageAreaMode", MeleeDamageAreaModes.Generated)]
        public MeleeDamageAreaShapes DamageAreaShape = MeleeDamageAreaShapes.Box;

        [FoldoutGroup("Damage Area")]
        [ShowIf("MeleeDamageAreaMode", MeleeDamageAreaModes.Generated)]
        [LabelText("Debug In Editor")]
        public bool DebugDamageAreaInEditor = false;

        [FoldoutGroup("Damage Area")]
        [ShowIf("MeleeDamageAreaMode", MeleeDamageAreaModes.Generated)]
        public Vector3 AreaOffset = Vector3.zero;

        [FoldoutGroup("Damage Area")]
        [ShowIf("MeleeDamageAreaMode", MeleeDamageAreaModes.Generated)]
        public Vector3 AreaSize = Vector3.one;

        [FoldoutGroup("Damage Area")]
        [ShowIf("MeleeDamageAreaMode", MeleeDamageAreaModes.Existing)]
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
        public Vector3 KnockbackForce = new(10, 2, 0);
        [FoldoutGroup("Damage Caused")]
        public DamageOnTouch.KnockBackDirections KnockBackDirection = DamageOnTouch.KnockBackDirections.BasedOnOwnerPosition;
        [FoldoutGroup("Damage Caused")]
        public float InvincibilityDuration = 0.5f;
        [FoldoutGroup("Damage Caused")]
        public bool CanDamageOwner = false;

        protected Collider _damageAreaCollider;
        protected bool _attackInProgress = false;
        protected BoxCollider _boxCollider;
        protected SphereCollider _sphereCollider;
        protected DamageOnTouch _damageOnTouch;
        protected GameObject _damageArea;


        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            if (_damageArea == null)
            {
                CreateDamageArea();
                DisableDamageArea();
            }
            if (Brain != null)
            {
                _damageOnTouch.Owner = Brain.gameObject;
            }
        }

        /// <summary>
        /// Creates the damage area.
        /// </summary>
        protected virtual void CreateDamageArea()
        {
            if ((MeleeDamageAreaMode == MeleeDamageAreaModes.Existing) && (ExistingDamageArea != null))
            {
                _damageArea = ExistingDamageArea.gameObject;
                _damageAreaCollider = _damageArea.gameObject.GetComponent<Collider>();
                _damageOnTouch = ExistingDamageArea;
                return;
            }

            _damageArea = new GameObject();
            _damageArea.name = this.name + "DamageArea";
            _damageArea.transform.position = this.transform.position;
            _damageArea.transform.rotation = this.transform.rotation;
            _damageArea.transform.SetParent(this.transform);
            _damageArea.transform.localScale = Vector3.one;
            _damageArea.layer = this.gameObject.layer;

            if (DamageAreaShape == MeleeDamageAreaShapes.Box)
            {
                _boxCollider = _damageArea.AddComponent<BoxCollider>();
                _boxCollider.center = AreaOffset;
                _boxCollider.size = AreaSize;
                _damageAreaCollider = _boxCollider;
                _damageAreaCollider.isTrigger = true;
            }
            if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
            {
                _sphereCollider = _damageArea.AddComponent<SphereCollider>();
                _sphereCollider.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
                _sphereCollider.radius = AreaSize.x / 2;
                _damageAreaCollider = _sphereCollider;
                _damageAreaCollider.isTrigger = true;
            }

            if ((DamageAreaShape == MeleeDamageAreaShapes.Box) || (DamageAreaShape == MeleeDamageAreaShapes.Sphere))
            {
                Rigidbody rigidBody = _damageArea.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;

                //TODO 暂时不知道MMRagdollerIgnore是啥玩意
                // rigidBody.gameObject.AddComponent<MMRagdollerIgnore>();
            }

            _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();
            _damageOnTouch.SetGizmoSize(AreaSize);
            _damageOnTouch.SetGizmoOffset(AreaOffset);
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.MinDamageCaused = MinDamageCaused;
            _damageOnTouch.MaxDamageCaused = MaxDamageCaused;
            _damageOnTouch.DamageDirectionMode = DamageOnTouch.DamageDirections.BasedOnOwnerPosition;
            _damageOnTouch.DamageCausedKnockBackType = KnockBack;
            _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
            _damageOnTouch.DamageCausedKnockBackDirection = KnockBackDirection;
            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
            // _damageOnTouch.HitDamageableFeedback = HitDamageableFeedback;
            // _damageOnTouch.HitNonDamageableFeedback = HitNonDamageableFeedback;
            // _damageOnTouch.TriggerFilter = TriggerFilter;

            if (!CanDamageOwner && (Brain != null))
            {
                _damageOnTouch.IgnoreGameObject(Brain.CharacterController.gameObject);
            }
        }

        /// <summary>
        /// When the weapon is used, we trigger our attack routine
        /// </summary>
        protected override void WeaponUse()
        {
            base.WeaponUse();
            StartCoroutine(MeleeWeaponAttack());
        }

        

        /// <summary>
        /// Triggers an attack, turning the damage area on and then off
        /// </summary>
        /// <returns>The weapon attack.</returns>
        protected virtual IEnumerator MeleeWeaponAttack()
        {
            if (_attackInProgress) { yield break; }

            _attackInProgress = true;
            yield return new WaitForSeconds(InitialDelay);
            EnableDamageArea();
            yield return new WaitForSeconds(ActiveDuration);
            DisableDamageArea();
            _attackInProgress = false;
        }

        /// <summary>
        /// Enables the damage area.
        /// </summary>
        protected virtual void EnableDamageArea()
        {
            if (_damageAreaCollider != null)
            {
                _damageAreaCollider.enabled = true;
            }
        }


        /// <summary>
        /// Disables the damage area.
        /// </summary>
        protected virtual void DisableDamageArea()
        {
            if (_damageAreaCollider != null)
            {
                _damageAreaCollider.enabled = false;
            }
        }

        /// <summary>
        /// When selected, we draw a bunch of gizmos
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                DrawGizmos();
            }
        }

        protected virtual void DrawGizmos()
        {
            if (DebugDamageAreaInEditor)
            {
                if (MeleeDamageAreaMode == MeleeDamageAreaModes.Existing)
                {
                    return;
                }

                if (DamageAreaShape == MeleeDamageAreaShapes.Box)
                {
                    Gizmos.DrawWireCube(this.transform.position + AreaOffset, AreaSize);
                }

                if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
                {
                    Gizmos.DrawWireSphere(this.transform.position + AreaOffset, AreaSize.x / 2);
                }
            }
        }

        /// <summary>
        /// On disable we set our flag to false
        /// </summary>
        protected virtual void OnDisable()
        {
            _attackInProgress = false;
        }
    }
}
