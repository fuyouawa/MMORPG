using System;
using System.Collections;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.TextCore.Text;
using static MMORPG.Game.Weapon;
using static UnityEngine.ParticleSystem;

namespace MMORPG.Game
{
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

        [FoldoutGroup("Settings")]
        public bool InitializeOnStart = true;
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

        public PlayerBrain Brain { get; private set; }

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

        public FSM<WeaponStates> FSM { get; set; } = new();

        public event Action<Weapon> OnWeaponStarted;
        public event Action<Weapon> OnWeaponStopped;

        private bool _initialized = false;

        protected virtual void Start()
        {
            if (InitializeOnStart)
            {
                Initialize();
            }
        }

        protected virtual void Update()
        {
            if (!_initialized) return;

            UpdateAnimator();
            FSM.Update();
        }


        public virtual void Setup(PlayerBrain brain)
        {
            Brain = brain;
        }

        public virtual void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            InitFSM();
        }

        public virtual void WeaponInputStart()
        {
            if (FSM.CurrentStateId == WeaponStates.Idle && !PreventFire)
            {
                _triggerReleased = false;
                TurnWeaponOn();
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
            if (DelayBeforeUse > 0)
            {
                _delayBeforeUseCounter = DelayBeforeUse;
                FSM.ChangeState(WeaponStates.DelayBeforeUse);
            }
            else
            {
                StartCoroutine(ShootRequestCo());
            }

            Brain.PreventMovement = PreventAllMovementWhileInUse;
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
            FSM.ChangeState(WeaponStates.Idle);
            Brain.PreventMovement = false;
        }

        protected virtual void ShootRequest()
        {
            FSM.ChangeState(WeaponStates.Use);
        }

        protected virtual void WeaponUse()
        {
            //TODO
        }

        protected virtual void UpdateAnimator()
        {
            if (!IdleAnimationParameter.IsNullOrEmpty())
                Brain.AnimationController.Animator.SetBool(IdleAnimationParameter, FSM.CurrentStateId == WeaponStates.Idle);

            if (!StartAnimationParameter.IsNullOrEmpty())
                Brain.AnimationController.Animator.SetBool(StartAnimationParameter, FSM.CurrentStateId == WeaponStates.Start);

            if (!DelayBeforeUseAnimationParameter.IsNullOrEmpty())
                Brain.AnimationController.Animator.SetBool(DelayBeforeUseAnimationParameter, FSM.CurrentStateId == WeaponStates.DelayBeforeUse);

            if (!UseAnimationParameter.IsNullOrEmpty())
                Brain.AnimationController.Animator.SetBool(UseAnimationParameter, FSM.CurrentStateId == WeaponStates.Use);

            if (!DelayBetweenUsesAnimationParameter.IsNullOrEmpty())
                Brain.AnimationController.Animator.SetBool(DelayBetweenUsesAnimationParameter, FSM.CurrentStateId == WeaponStates.DelayBetweenUses);

            if (!StopAnimationParameter.IsNullOrEmpty())
                Brain.AnimationController.Animator.SetBool(StopAnimationParameter, FSM.CurrentStateId == WeaponStates.Stop);
        }

        protected virtual void OnDestroy()
        {
            FSM.Clear();
        }
    }
}
