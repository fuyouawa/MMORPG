using System;
using System.Collections;
using System.Linq;
using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.TextCore.Text;
using static MMORPG.Game.Weapon;
using static UnityEngine.ParticleSystem;

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

        [FoldoutGroup("Use")]
        public float DelayBeforeUse;
        [FoldoutGroup("Use")]
        public float TimeBetweenUses = 1f;
        [FoldoutGroup("Use")]
        public TriggerModes TriggerMode = TriggerModes.Auto;

#if UNITY_EDITOR
        [FoldoutGroup("Position")]
        [LabelText("Debug In Editor")]
        public bool PositionDebugInEditor = false;  //TODO PositionDebugInEditor
#endif
        [FoldoutGroup("Position")]
        public Vector3 WeaponAttachmentOffset;

        [FoldoutGroup("Movement")]
        public bool ModifyMovementWhileAttacking = false;   //TODO ModifyMovementWhileAttacking
        [FoldoutGroup("Movement")]
        public float MovementMultiplier = 1f;       //TODO MovementMultiplier
        [FoldoutGroup("Movement")]
        public bool PreventAllMovementWhileInUse = false;

        //TODO 动画丢失的bug
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

        [FoldoutGroup("Feedbacks")]
        public bool FindFeedbackByName = false;

        [FoldoutGroup("Feedbacks")]
        [ShowIf("FindFeedbackByName")]
        public string WeaponStartFeedbackName;
        [FoldoutGroup("Feedbacks")]
        [ShowIf("FindFeedbackByName")]
        public string WeaponUsedFeedbackName;
        [FoldoutGroup("Feedbacks")]
        [ShowIf("FindFeedbackByName")]
        public string WeaponStopFeedbackName;

        [FoldoutGroup("Feedbacks")]
        [HideIf("FindFeedbackByName")]
        public FeedbacksManager WeaponStartFeedbacks;
        [FoldoutGroup("Feedbacks")]
        [HideIf("FindFeedbackByName")]
        public FeedbacksManager WeaponUsedFeedbacks;
        [FoldoutGroup("Feedbacks")]
        [HideIf("FindFeedbackByName")]
        public FeedbacksManager WeaponStopFeedbacks;

        [FoldoutGroup("Settings")]
        public bool InitializeOnStart = false;
        [FoldoutGroup("Settings")]
        public bool Interruptable = false;
        [FoldoutGroup("Settings")]
        public float InterruptDelay;

        public bool CanInterrupt
        {
            get
            {
                if (!Interruptable || FSM.CurrentStateId is WeaponStates.Idle or WeaponStates.Stop or WeaponStates.Interrupted)
                    return false;
                return Time.time - _lastTurnWeaponOnAt > InterruptDelay;
            }
        }

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
            }

            InitFSM();

            OnWeaponInitialized?.Invoke(this);

            IsInitialized = true;
        }

        public virtual void WeaponInputStart()
        {
            if (FSM.CurrentStateId == WeaponStates.Idle)
            {
                if (!PreventFire)
                {
                    _triggerReleased = false;
                    TurnWeaponOn();
                }
            }
            else
            {
                OnWeaponTryInterrupt?.Invoke(this);
            }
        }
        public virtual void WeaponInputReleased()
        {

        }

        public virtual void WeaponInputStop()
        {
            _triggerReleased = true;
        }

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
            WeaponStartFeedbacks?.Play();
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
            WeaponStopFeedbacks?.Play();
            FSM.ChangeState(WeaponStates.Idle);
            Owner.StopPreventMovement();
        }

        protected virtual void ShootRequest()
        {
            FSM.ChangeState(WeaponStates.Use);
        }

        protected virtual void WeaponUse()
        {
            WeaponUsedFeedbacks?.Play();
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
    }
}
