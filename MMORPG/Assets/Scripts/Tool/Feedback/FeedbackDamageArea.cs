using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MMORPG.Tool
{
    [AddFeedbackMenu("Area/Damage Area")]
    public class FeedbackDamageArea : Feedback
    {
        public enum DamageAreaShapes { Box, Sphere }
        public enum DamageAreaModes { Generated, Existing }

        [FoldoutGroup("Damage Area")]
        public GameObject OwnerGameObject;
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

        [FoldoutGroup("Damage Caused")]
        public DamageOnTouch.TriggerMasks TriggerMask = DamageOnTouch.TriggerMasks.OnTriggerEnter;
        [FoldoutGroup("Damage Caused")]
        public LayerMask TargetLayerMask;
        [FoldoutGroup("Damage Caused")]
        public float MinDamageCaused = 10f;
        [FoldoutGroup("Damage Caused")]
        public float MaxDamageCaused = 10f;
        [FoldoutGroup("Damage Caused")]
        [Tooltip("额外无敌时间")]
        public float ExtraInvincibilityDuration = 0f;
        [FoldoutGroup("Damage Caused")]
        public DamageOnTouch.DamageDirections DamageDirectionMode;
        [FoldoutGroup("Damage Caused")]
        public bool IgnoreOwnerGameObject = true;
        [FoldoutGroup("Damage Caused")]
        public float ActiveDuration = 1f;
        [FoldoutGroup("Damage Caused")]
        public List<GameObject> IgnoredGameObjects = new();

        [FoldoutGroup("Damage over time")]
        public bool RepeatDamageOverTime = false;

        [FoldoutGroup("Damage over time")]
        [ShowIf("RepeatDamageOverTime")]
        public int AmountOfRepeats = 3;

        [FoldoutGroup("Damage over time")]
        [ShowIf("RepeatDamageOverTime")]
        public float DurationBetweenRepeats = 1f;

        [FoldoutGroup("Damage over time")]
        [ShowIf("RepeatDamageOverTime")]
        public bool DamageOverTimeInterruptible = true;

        protected Collider _damageAreaCollider;
        protected bool _attackInProgress = false;
        protected BoxCollider _boxCollider;
        protected SphereCollider _sphereCollider;
        protected DamageOnTouch _damageOnTouch;
        protected GameObject _damageArea;

        public override float GetDuration()
        {
            return ActiveDuration;
        }

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
            if (IgnoreOwnerGameObject && OwnerGameObject)
                IgnoredGameObjects.Add(OwnerGameObject);
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

            _damageArea = new()
            {
                name = nameof(DamageOnTouch),
                transform =
                {
                    position = Transform.position,
                    rotation = Transform.rotation,
                    localScale = Vector3.one
                }
            };

            _damageArea.transform.SetParent(Transform);
            _damageArea.layer = Owner.gameObject.layer;

            switch (DamageAreaShape)
            {
                case DamageAreaShapes.Box:
                    _boxCollider = _damageArea.AddComponent<BoxCollider>();
                    _boxCollider.center = AreaOffset;
                    _boxCollider.size = AreaSize;
                    _damageAreaCollider = _boxCollider;
                    _damageAreaCollider.isTrigger = true;
                    break;
                case DamageAreaShapes.Sphere:
                    _sphereCollider = _damageArea.AddComponent<SphereCollider>();
                    _sphereCollider.transform.position = Transform.position + Transform.rotation * AreaOffset;
                    _sphereCollider.radius = AreaSize.x / 2;
                    _damageAreaCollider = _sphereCollider;
                    _damageAreaCollider.isTrigger = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (DamageAreaShape is DamageAreaShapes.Box or DamageAreaShapes.Sphere)
            {
                var rigidBody = _damageArea.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }

            _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();
            _damageOnTouch.InitialOnStart = false;
            _damageOnTouch.TriggerMask = TriggerMask;
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.MinDamageCaused = MinDamageCaused;
            _damageOnTouch.MaxDamageCaused = MaxDamageCaused;
            _damageOnTouch.ExtraInvincibilityDuration = ExtraInvincibilityDuration;
            _damageOnTouch.DamageDirectionMode = DamageDirectionMode;
            _damageOnTouch.IgnoredGameObjects = IgnoredGameObjects;
            _damageOnTouch.RepeatDamageOverTime = RepeatDamageOverTime;
            _damageOnTouch.AmountOfRepeats = AmountOfRepeats;
            _damageOnTouch.DurationBetweenRepeats = DurationBetweenRepeats;
            _damageOnTouch.DamageOverTimeInterruptible = DamageOverTimeInterruptible;

            _damageOnTouch.Initialize();
        }

        protected override void OnFeedbackPlay()
        {
            StartCoroutine(ProcessDamageCo());
        }

        protected virtual IEnumerator ProcessDamageCo()
        {
            EnableDamageArea();
            yield return new WaitForSeconds(ActiveDuration);
            DisableDamageArea();
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
