using System;
using System.Collections;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.TextCore.Text;
using static MMORPG.Game.Weapon;
using static UnityEngine.ParticleSystem;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
    public class Weapon : MonoBehaviour
    {
        [FoldoutGroup("Id")]
        [Required]
        public string WeaponName = "TODO";

        [FoldoutGroup("Use")]
        public float DelayBeforeUse;
        [FoldoutGroup("Use")]
        public float TimeBetweenUses = 1f;

        [FoldoutGroup("Animator Parameter Names")]
        public string StartAnimationParam;

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
            Stop
        }

        public FSM<WeaponState> FSM { get; set; } = new();

        public event Action OnWeaponStart;
        public event Action OnWeaponStop;

        protected virtual void Awake()
        {
            InitFSM();
        }

        protected virtual void Update()
        {
            FSM.Update();
        }


        public virtual void Setup(PlayerBrain brain)
        {
            Brain = brain;
        }

        public virtual void WeaponInputStart()
        {
            if (FSM.CurrentStateId == WeaponState.Idle)
            {
                _triggerReleased = false;
                TurnWeaponOn();
            }
        }

        public virtual void TurnWeaponOn()
        {
            if (Time.time - _lastTurnWeaponOnAt < TimeBetweenUses)
            {
                return;
            }
            _lastTurnWeaponOnAt = Time.time;

            FSM.ChangeState(WeaponState.Start);
            OnWeaponStart?.Invoke();
        }

        public virtual void TurnWeaponOff()
        {
            if (FSM.CurrentStateId is WeaponState.Idle or WeaponState.Stop)
            {
                return;
            }
            _triggerReleased = true;
            FSM.ChangeState(WeaponState.Stop);
            OnWeaponStop?.Invoke();
        }


        protected virtual void InitFSM()
        {
            FSM.State(WeaponState.Idle).OnUpdate(CaseWeaponIdle);
            FSM.State(WeaponState.Start).OnUpdate(CaseWeaponStart);
            FSM.State(WeaponState.DelayBeforeUse).OnUpdate(CaseWeaponDelayBeforeUse);
            FSM.State(WeaponState.Use).OnUpdate(CaseWeaponUse);
            FSM.State(WeaponState.DelayBetweenUses).OnUpdate(CaseWeaponDelayBetweenUses);
            FSM.State(WeaponState.Stop).OnUpdate(CaseWeaponStop);

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
    }
}
