using System;
using System.Collections;
using System.Linq;
using Common.Proto.Entity;
using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public enum WeaponStates
    {
        Idle,
        Start,
        DelayBeforeUse,
        Use,
        DelayBetweenUses,
        Stop,
        Interrupted
    }

    public class Weapon : MonoBehaviour
    {
        public enum TriggerModes { SemiAuto, Auto }

        [FoldoutGroup("Id")]
        public int WeaponId;
        [FoldoutGroup("Use")]
        [Tooltip("当WeaponInputStart或者TurnWeaponOn后经过多少时间正式WeaponUse")]
        public float DelayBeforeUse;
        [FoldoutGroup("Use")]
        [Tooltip("类似冷却时间")]
        public float TimeBetweenUses = 1f;
        [FoldoutGroup("Use")]
        [Tooltip("SemiAuto: 当用完武器后就停止使用武器\nAuto: 当用完武器后继续使用武器, 直到手动调用WeaponInputStop或者TurnWeaponOff")]
        public TriggerModes TriggerMode = TriggerModes.Auto;

// #if UNITY_EDITOR
//         [FoldoutGroup("Position")]
//         [LabelText("Debug In Editor")]
//         public bool PositionDebugInEditor = false;  //TODO PositionDebugInEditor
// #endif
        [FoldoutGroup("Position")]
        [Tooltip("武器附加时的偏移, 一般在连招中使用")]
        public Vector3 WeaponAttachmentOffset;

        // [FoldoutGroup("Movement")]
        // public bool ModifyMovementWhileAttacking = false;   //TODO ModifyMovementWhileAttacking
        // [FoldoutGroup("Movement")]
        // public float MovementMultiplier = 1f;       //TODO MovementMultiplier
        [FoldoutGroup("Movement")]
        [Tooltip("当使用武器时阻止人物移动")]
        public bool PreventAllMovementWhileInUse = false;

        [FoldoutGroup("Animator Parameter Names")]
        public string IdleAnimationParameter;
        [FoldoutGroup("Animator Parameter Names")]
        public string StartAnimationParameter;
        [FoldoutGroup("Animator Parameter Names")]
        public string DelayBeforeUseAnimationParameter;
        [FoldoutGroup("Animator Parameter Names")]
        public string UseAnimationParameter;
        [FoldoutGroup("Animator Parameter Names")]
        public string DelayBetweenUsesAnimationParameter;
        [FoldoutGroup("Animator Parameter Names")]
        public string StopAnimationParameter;

        [FoldoutGroup("Weapon Feedbacks")]
        [Tooltip("当武器附加时, 根据Feedback的名称在拥有者(Owner)的子物体中找对应的Feedback")]
        public bool FindFeedbackByName = false;

        [FoldoutGroup("Weapon Feedbacks")]
        [ShowIf("FindFeedbackByName")]
        public string WeaponStartFeedbackName;
        [FoldoutGroup("Weapon Feedbacks")]
        [ShowIf("FindFeedbackByName")]
        public string WeaponUsedFeedbackName;
        [FoldoutGroup("Weapon Feedbacks")]
        [ShowIf("FindFeedbackByName")]
        public string WeaponStopFeedbackName;

        [FoldoutGroup("Weapon Feedbacks")]
        [HideIf("FindFeedbackByName")]
        public FeedbacksManager WeaponStartFeedbacks;
        [FoldoutGroup("Weapon Feedbacks")]
        [HideIf("FindFeedbackByName")]
        public FeedbacksManager WeaponUsedFeedbacks;
        [FoldoutGroup("Weapon Feedbacks")]
        [HideIf("FindFeedbackByName")]
        public FeedbacksManager WeaponStopFeedbacks;

        [FoldoutGroup("Hit Feedbacks")]
        public FeedbacksManager HitPlayerFeedbacks;
        [FoldoutGroup("Hit Feedbacks")]
        public FeedbacksManager HitMonsterFeedbacks;
        [FoldoutGroup("Hit Feedbacks")]
        public FeedbacksManager HitNPCFeedbacks;

        [FoldoutGroup("Settings")]
        public bool InitializeOnStart = false;
        [FoldoutGroup("Settings")]
        [Tooltip("是否可以在武器使用时(包括冷却时)打断")]
        public bool Interruptible = false;
        [FoldoutGroup("Settings")]
        [ShowIf("Interruptible")]
        [Tooltip("经过多少时间后可以在武器使用时(包括冷却时)被打断")]
        public float TimeBeforeInterruptible;

        public bool CanInterrupt
        {
            get
            {
                if (!Interruptible || FSM.CurrentStateId is WeaponStates.Idle or WeaponStates.Stop or WeaponStates.Interrupted)
                    return false;
                return Time.time - _lastTurnWeaponOnAt > TimeBeforeInterruptible;
            }
        }

        public bool CanUse => FSM.CurrentStateId == WeaponStates.Idle && !PreventFire;

        public bool PreventFire { get; set; } = false;

        private float _lastTurnWeaponOnAt = -float.MaxValue;
        private float _lastShootRequestAt = -float.MaxValue;
        private float _delayBeforeUseCounter;
        private float _delayBetweenUsesCounter;
        private bool _triggerReleased;

        public CharacterController Owner { get; private set; }

        public FSM<WeaponStates> FSM { get; set; }

        public event Action<Weapon> OnWeaponInitialized;
        public event Action<Weapon> OnWeaponStarted;
        public event Action<Weapon> OnWeaponTryInterrupt;
        public event Action<Weapon> OnWeaponStopped;

        public bool IsInitialized { get; private set; }

        protected virtual void Start()
        {
            if (InitializeOnStart)
            {
                Initialize();
            }
        }

        protected virtual void Update()
        {
            if (!IsInitialized) return;

            UpdateAnimator();
            FSM.Update();
        }


        public virtual void Setup(CharacterController owner)
        {
            Owner = owner;
        }

        public virtual void Initialize()
        {
            if (IsInitialized) return;

            if (FindFeedbackByName)
            {
                var feedbacks = Owner.GetComponentsInChildren<FeedbacksManager>();

                if (WeaponStartFeedbackName.IsNotNullAndEmpty())
                {
                    WeaponStartFeedbacks = feedbacks.FirstOrDefault(x => x.name == WeaponStartFeedbackName);
                    if (WeaponStartFeedbacks == null)
                        throw new Exception($"Invalid WeaponStartFeedbackName({WeaponStartFeedbackName})");
                    WeaponStartFeedbacks.Setup(gameObject);
                    WeaponStartFeedbacks.Initialize();
                }

                if (WeaponUsedFeedbackName.IsNotNullAndEmpty())
                {
                    WeaponUsedFeedbacks = feedbacks.FirstOrDefault(x => x.name == WeaponUsedFeedbackName);
                    if (WeaponUsedFeedbacks == null)
                        throw new Exception($"Invalid WeaponUsedFeedbackName({WeaponUsedFeedbackName})");
                    WeaponUsedFeedbacks.Setup(gameObject);
                    WeaponUsedFeedbacks.Initialize();
                }

                if (WeaponStopFeedbackName.IsNotNullAndEmpty())
                {
                    WeaponStopFeedbacks = feedbacks.FirstOrDefault(x => x.name == WeaponStopFeedbackName);
                    if (WeaponStopFeedbacks == null)
                        throw new Exception($"Invalid WeaponStopFeedbackName({WeaponStopFeedbackName})");
                    WeaponStopFeedbacks.Setup(gameObject);
                    WeaponStopFeedbacks.Initialize();
                }

                if (HitPlayerFeedbacks != null)
                    HitPlayerFeedbacks.Initialize();
                if (HitMonsterFeedbacks != null)
                    HitMonsterFeedbacks.Initialize();
                if (HitNPCFeedbacks != null)
                    HitNPCFeedbacks.Initialize();
            }

            InitFSM();

            OnWeaponInitialized?.Invoke(this);

            IsInitialized = true;
        }

        /// <summary>
        /// <para>使用武器</para>
        /// <para>1. 如果当前武器正在使用中, 会尝试打断(Interrupt)</para>
        /// <para>2. 如果PreventFire为True, 会阻止使用</para>
        /// </summary>
        public virtual void WeaponInputStart()
        {
            if (CanUse)
            {
                _triggerReleased = false;
                TurnWeaponOn();
            }
            else
            {
                OnWeaponTryInterrupt?.Invoke(this);
            }
        }

        /// <summary>
        /// <para>停止武器使用</para>
        /// <para>会在当前武器使用完(包括冷却完成)后才停止</para>
        /// </summary>
        public virtual void WeaponInputStop()
        {
            _triggerReleased = true;
        }

        /// <summary>
        /// <para>直接使用武器</para>
        /// <para>Tip: 推荐使用WeaponInputStart</para>
        /// </summary>
        public virtual void TurnWeaponOn()
        {
            if (Time.time - _lastTurnWeaponOnAt < TimeBetweenUses || PreventFire)
            {
                return;
            }
            _lastTurnWeaponOnAt = Time.time;

            FSM.ChangeState(WeaponStates.Start);
            OnWeaponStarted?.Invoke(this);
        }

        /// <summary>
        /// <para>强行停止武器使用</para>
        /// <para>Tip: 推荐使用WeaponInputStop</para>
        /// </summary>
        public virtual void TurnWeaponOff()
        {
            if (FSM.CurrentStateId is WeaponStates.Idle or WeaponStates.Stop)
            {
                return;
            }
            _triggerReleased = true;
            FSM.ChangeState(WeaponStates.Stop);
            OnWeaponStopped?.Invoke(this);
        }

        public virtual bool TryInterrupt()
        {
            if (CanInterrupt)
            {
                FSM.ChangeState(WeaponStates.Interrupted);
                return true;
            }
            return false;
        }


        protected virtual void InitFSM()
        {
            FSM ??= new();

            FSM.State(WeaponStates.Idle).OnUpdate(CaseWeaponIdle);
            FSM.State(WeaponStates.Start).OnUpdate(CaseWeaponStart);
            FSM.State(WeaponStates.DelayBeforeUse).OnUpdate(CaseWeaponDelayBeforeUse);
            FSM.State(WeaponStates.Use).OnUpdate(CaseWeaponUse);
            FSM.State(WeaponStates.DelayBetweenUses).OnUpdate(CaseWeaponDelayBetweenUses);
            FSM.State(WeaponStates.Stop).OnUpdate(CaseWeaponStop);
            FSM.State(WeaponStates.Interrupted).OnUpdate(CaseWeaponInterrupted);

            FSM.StartState(WeaponStates.Idle);
        }

        protected virtual void CaseWeaponIdle()
        {
        }

        protected virtual void CaseWeaponStart()
        {
            if (WeaponStartFeedbacks != null)
                WeaponStartFeedbacks.Play();
            if (DelayBeforeUse > 0)
            {
                _delayBeforeUseCounter = DelayBeforeUse;
                FSM.ChangeState(WeaponStates.DelayBeforeUse);
            }
            else
            {
                StartCoroutine(ShootRequestCo());
            }

            if (PreventAllMovementWhileInUse)
                Owner.PreventMovement();
        }

        protected virtual void CaseWeaponDelayBeforeUse()
        {
            _delayBeforeUseCounter -= Time.deltaTime;
            if (_delayBeforeUseCounter <= 0)
            {
                StartCoroutine(ShootRequestCo());
            }
        }

        protected virtual IEnumerator ShootRequestCo()
        {
            if (Time.time - _lastShootRequestAt < TimeBetweenUses)
            {
                yield break;
            }
            ShootRequest();
            _lastShootRequestAt = Time.time;
        }

        protected virtual void CaseWeaponUse()
        {
            WeaponUse();
            _delayBetweenUsesCounter = TimeBetweenUses;
            FSM.ChangeState(WeaponStates.DelayBetweenUses);
        }

        protected virtual void CaseWeaponDelayBetweenUses()
        {
            if (_triggerReleased)
            {
                TurnWeaponOff();
                return;
            }

            _delayBetweenUsesCounter -= Time.deltaTime;
            if (_delayBetweenUsesCounter <= 0)
            {
                if (TriggerMode == TriggerModes.Auto && !_triggerReleased)
                {
                    StartCoroutine(ShootRequestCo());
                }
                else
                {
                    TurnWeaponOff();
                }
            }
        }

        protected virtual void CaseWeaponInterrupted()
        {
            TurnWeaponOff();
            FSM.ChangeState(WeaponStates.Idle);
        }

        protected virtual void CaseWeaponStop()
        {
            if (WeaponStopFeedbacks != null)
                WeaponStopFeedbacks.Play();
            FSM.ChangeState(WeaponStates.Idle);
            if (PreventAllMovementWhileInUse)
                Owner.StopPreventMovement();
        }

        protected virtual void ShootRequest()
        {
            FSM.ChangeState(WeaponStates.Use);
        }

        protected virtual void WeaponUse()
        {
            if (WeaponUsedFeedbacks != null)
                WeaponUsedFeedbacks.Play();
        }

        protected virtual void OnWeaponUse()
        {
        }

        protected virtual void UpdateAnimator()
        {
            if (!IdleAnimationParameter.IsNullOrEmpty())
                Owner.Animator.SetBool(IdleAnimationParameter, FSM.CurrentStateId == WeaponStates.Idle);

            if (!StartAnimationParameter.IsNullOrEmpty())
                Owner.Animator.SetBool(StartAnimationParameter, FSM.CurrentStateId == WeaponStates.Start);

            if (!DelayBeforeUseAnimationParameter.IsNullOrEmpty())
                Owner.Animator.SetBool(DelayBeforeUseAnimationParameter, FSM.CurrentStateId == WeaponStates.DelayBeforeUse);

            if (!UseAnimationParameter.IsNullOrEmpty())
                Owner.Animator.SetBool(UseAnimationParameter, FSM.CurrentStateId == WeaponStates.Use);

            if (!DelayBetweenUsesAnimationParameter.IsNullOrEmpty())
                Owner.Animator.SetBool(DelayBetweenUsesAnimationParameter, FSM.CurrentStateId == WeaponStates.DelayBetweenUses);

            if (!StopAnimationParameter.IsNullOrEmpty())
                Owner.Animator.SetBool(StopAnimationParameter, FSM.CurrentStateId == WeaponStates.Stop);
        }

        protected virtual void OnDestroy()
        {
            FSM?.Clear();
        }

        public virtual void OnHitEntity(Health health)
        {
            switch (health.Entity.EntityType)
            {
                case EntityType.Player:
                    if (HitPlayerFeedbacks != null)
                        HitPlayerFeedbacks.Play();
                    break;
                case EntityType.Monster:
                    if (HitMonsterFeedbacks != null)
                        HitMonsterFeedbacks.Play();
                    break;
                case EntityType.Npc:
                    if (HitNPCFeedbacks != null)
                        HitNPCFeedbacks.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
