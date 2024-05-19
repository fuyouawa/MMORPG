using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MMORPG.Game
{
    public enum HandleWeaponMode
    {
        LeftHand,
        RightHand
    }

    public class PlayerHandleWeapon : MonoBehaviour
    {
        [Title("Weapon")]
        [AssetsOnly]
        public Weapon InitialWeapon;

        [ReadOnly]
        [ShowInInspector]
        public Weapon CurrentWeapon { get; private set; }
        [Title("Binding")]
        public Transform WeaponAttachment;
        public HandleWeaponMode HandleMode = HandleWeaponMode.RightHand;
        public Transform LeftHandTarget;
        public Transform RightHandTarget;

        public delegate void WeaponChangedHandler(Weapon current, Weapon previous);
        public event WeaponChangedHandler OnWeaponChanged;

        public PlayerBrain Brain { get; private set; }

        [Button]
        private void UpdateWeaponAttachmentTransform()
        {
            if (WeaponAttachment)
            {
                switch (HandleMode)
                {
                    case HandleWeaponMode.LeftHand:
                        if (LeftHandTarget)
                            WeaponAttachment.SetPositionAndRotation(LeftHandTarget.position, LeftHandTarget.rotation);
                        break;
                    default:
                        if (RightHandTarget)
                            WeaponAttachment.SetPositionAndRotation(RightHandTarget.position, RightHandTarget.rotation);
                        break;
                }
            }
        }

        private void Start()
        {
            if (Brain.IsMine)
            {
                Brain.InputControls.Player.Fire.started += OnFireStarted;
            }
            if (InitialWeapon)
            {
                ChangeWeapon(InitialWeapon);
            }
        }

        private void Update()
        {
            Debug.Log(CurrentWeapon?.StartAnimationParameter);
            UpdateWeaponAttachmentTransform();
        }

        public void Setup(PlayerBrain brain)
        {
            Brain = brain;
        }

        public void ChangeWeapon(Weapon newWeapon, bool combo = false)
        {
            if (CurrentWeapon)
            {
                CurrentWeapon.TurnWeaponOff();
                if (!combo)
                {
                    Destroy(CurrentWeapon.gameObject);
                }
            }

            var tmp = CurrentWeapon;
            if (newWeapon != null)
            {
                InstantiateWeapon(newWeapon, combo);
            }
            else
            {
                CurrentWeapon = null;
            }
            OnWeaponChanged?.Invoke(newWeapon, tmp);
        }

        private void InstantiateWeapon(Weapon newWeapon, bool combo)
        {
            if (!combo)
            {
                CurrentWeapon = Instantiate(newWeapon,
                    WeaponAttachment.transform.position + newWeapon.WeaponAttachmentOffset,
                    WeaponAttachment.transform.rotation);
            }
            else
            {
                CurrentWeapon = newWeapon;
            }
            CurrentWeapon.transform.parent = WeaponAttachment.transform;
            CurrentWeapon.transform.localPosition = newWeapon.WeaponAttachmentOffset;
            CurrentWeapon.Setup(Brain);
            if (!CurrentWeapon.InitializeOnStart)
                CurrentWeapon.Initialize();
        }

        private void OnFireStarted(InputAction.CallbackContext obj)
        {
            if (CurrentWeapon.FSM.CurrentStateId is WeaponStates.Idle)
            {
                CurrentWeapon.WeaponInputStart();
            }
        }
    }
}
