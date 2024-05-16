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
        [FoldoutGroup("Use")]
        public float DelayBeforeUse;
        [FoldoutGroup("Use")]
        public float TimeBetweenUses = 1f;

#if UNITY_EDITOR
        [FoldoutGroup("Position")]
        [LabelText("Debug In Editor")]
        public bool PositionDebugInEditor = false;
#endif
        [FoldoutGroup("Position")]
        public Vector3 WeaponAttachmentOffset;

        [FoldoutGroup("Movement")]
        public bool ModifyMovementWhileAttacking = false;
        public float MovementMultiplier = 0f;
        public bool PreventAllMovementWhileInUse = false;

        [FoldoutGroup("Animator Parameter Names")]
        public string StartAnimationParam;

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
                if (!Interruptable || FSM.CurrentStateId is WeaponState.Idle or WeaponState.Stop or WeaponState.Interrupted)
                    return false;
                return Time.time - _lastTurnWeaponOnAt > InterruptDelay;
            }
        }

        public bool PreventFire = false;

        private float _lastTurnWeaponOnAt = -float.MaxValue;
        private float _lastShootRequestAt = -float.MaxValue;
        private float _delayBeforeUseCounter;
        private float _delayBetweenUsesCounter;
        private bool _triggerReleased;

        public PlayerBrain Brain { get; private set; }

        public enum WeaponState
        {
            Idle,
            Start,
            DelayBeforeUse,
            Use,
            DelayBetweenUses,
            Stop,
            Interrupted
        }

        public FSM<WeaponState> FSM { get; set; } = new();

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
            if (FSM.CurrentStateId == WeaponState.Idle && !PreventFire)
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

            FSM.ChangeState(WeaponState.Start);
            OnWeaponStarted?.Invoke(this);
        }

        public virtual void TurnWeaponOff()
        {
            if (FSM.CurrentStateId is WeaponState.Idle or WeaponState.Stop)
            {
                return;
            }
            _triggerReleased = true;
            FSM.ChangeState(WeaponState.Stop);
            OnWeaponStopped?.Invoke(this);
        }

        public virtual bool TryInterrupt()
        {
            if (CanInterrupt)
            {
                FSM.ChangeState(WeaponState.Interrupted);
                return true;
            }
            return false;
        }


        protected virtual void InitFSM()
        {
            FSM.State(WeaponState.Idle).OnUpdate(CaseWeaponIdle);
            FSM.State(WeaponState.Start).OnUpdate(CaseWeaponStart);
            FSM.State(WeaponState.DelayBeforeUse).OnUpdate(CaseWeaponDelayBeforeUse);
            FSM.State(WeaponState.Use).OnUpdate(CaseWeaponUse);
            FSM.State(WeaponState.DelayBetweenUses).OnUpdate(CaseWeaponDelayBetweenUses);
            FSM.State(WeaponState.Stop).OnUpdate(CaseWeaponStop);
            FSM.State(WeaponState.Interrupted).OnUpdate(CaseWeaponInterrupted);

            FSM.StartState(WeaponState.Idle);
        }

        protected virtual void CaseWeaponIdle()
        {
        }

        protected virtual void CaseWeaponStart()
        {
            if (DelayBeforeUse > 0)
            {
                _delayBeforeUseCounter = DelayBeforeUse;
                FSM.ChangeState(WeaponState.DelayBeforeUse);
            }
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
            FSM.ChangeState(WeaponState.DelayBetweenUses);
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
                if (!_triggerReleased)
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
            FSM.ChangeState(WeaponState.Idle);
        }

        protected virtual void CaseWeaponStop()
        {
            FSM.ChangeState(WeaponState.Idle);
        }

        protected virtual void ShootRequest()
        {
            FSM.ChangeState(WeaponState.Use);
        }

        protected virtual void WeaponUse()
        {
            //TODO
        }

        protected virtual void UpdateAnimator()
        {
            if (!StartAnimationParam.IsNullOrEmpty())
            {
                Brain.AnimationController.Animator.SetBool(StartAnimationParam, FSM.CurrentStateId == WeaponState.Start);
            }
        }
    }
}
