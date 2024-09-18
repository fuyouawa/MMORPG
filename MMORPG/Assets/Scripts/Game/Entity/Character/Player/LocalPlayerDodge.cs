using System;
using System.Collections;
using MMORPG.Global;
using MMORPG.Tool;
using UnityEngine;

namespace MMORPG.Game
{
    public enum DodgeStates
    {
        None,
        Front,
        Back,
        Left,
        Right
    }

    public class LocalPlayerDodge : LocalPlayerAbility
    {
        public float DodgeHoriDuration = 0.7f;
        public float DodgeVertDuration = 0.5f;
        public float DodgeCoolTime = 1.3f;
        public float SpeedMultiply = 1.5f;

        private DoubleClickDetector _dodgeFrontDetector;
        private DoubleClickDetector _dodgeBackDetector;
        private DoubleClickDetector _dodgeLeftDetector;
        private DoubleClickDetector _dodgeRightDetector;

        private DodgeStates _dodgeState;
        private float _lastDodgeTime;

        public override void OnStateInit()
        {
            _dodgeFrontDetector = new(OwnerState.Brain.InputControls.Player.W, () => OnDodgeClick(DodgeStates.Front));
            _dodgeBackDetector = new(OwnerState.Brain.InputControls.Player.S, () => OnDodgeClick(DodgeStates.Back));
            _dodgeLeftDetector = new(OwnerState.Brain.InputControls.Player.A, () => OnDodgeClick(DodgeStates.Left));
            _dodgeRightDetector = new(OwnerState.Brain.InputControls.Player.D, () => OnDodgeClick(DodgeStates.Right));
        }


        public override void OnStateEnter()
        {
            switch (_dodgeState)
            {
                case DodgeStates.None:
                    break;
                case DodgeStates.Front:
                    OwnerState.Brain.ActorController.Animator.SetTrigger("DodgeF");
                    break;
                case DodgeStates.Back:
                    OwnerState.Brain.ActorController.Animator.SetTrigger("DodgeB");
                    break;
                case DodgeStates.Left:
                    OwnerState.Brain.ActorController.Animator.SetTrigger("DodgeL");
                    break;
                case DodgeStates.Right:
                    OwnerState.Brain.ActorController.Animator.SetTrigger("DodgeR");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StartCoroutine("BackIdleCo", _dodgeState);
        }

        public override void OnStateExit()
        {
            StopCoroutine("BackIdleCo");
        }

        private IEnumerator BackIdleCo(DodgeStates state)
        {
            _lastDodgeTime = Time.time;
            OwnerState.Brain.AnimationController.SpeedMultiply = SpeedMultiply;

            if (state is DodgeStates.Front or DodgeStates.Back)
                yield return new WaitForSeconds(DodgeHoriDuration);
            else
                yield return new WaitForSeconds(DodgeVertDuration);

            OwnerState.Brain.AnimationController.SpeedMultiply = 1f;
            OwnerState.Brain.ChangeStateByName("Idle");
        }


        private void OnDodgeClick(DodgeStates dodge)
        {
            if (!InputManager.CanInput)
                return;
            StartCoroutine(OnDodgeClickCo(dodge));
        }

        private IEnumerator OnDodgeClickCo(DodgeStates dodge)
        {
            _dodgeState = dodge;
            yield return null;
            _dodgeState = DodgeStates.None;
        }

        [StateCondition]
        public bool IsDodgeClick()
        {
            return _dodgeState != DodgeStates.None && Time.time - _lastDodgeTime >= DodgeCoolTime;
        }
    }
}
