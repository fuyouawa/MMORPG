using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public enum HandleWeaponMode
    {
        NoWeapon,
        LeftHand,
        RightHand
    }

    public class CharacterHandleWeapon : MonoBehaviour
    {
        [Title("Weapon")]
        [AssetsOnly]
        [Tooltip("初始化时持有的武器")]
        public Weapon InitialWeapon;

        [ReadOnly]
        [ShowInInspector]
        public Weapon CurrentWeapon { get; private set; }
        [Title("Binding")]
        [Tooltip("武器附加位置")]
        public Transform WeaponAttachment;
        public HandleWeaponMode HandleMode = HandleWeaponMode.RightHand;
        [ShowIf("HandleMode", HandleWeaponMode.LeftHand)]
        public Transform LeftHandTarget;
        [ShowIf("HandleMode", HandleWeaponMode.RightHand)]
        public Transform RightHandTarget;

        public delegate void WeaponChangedHandler(Weapon current, Weapon previous);
        public event WeaponChangedHandler OnWeaponChanged;

        public CharacterController Owner { get; private set; }

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
                    case HandleWeaponMode.RightHand:
                        if (RightHandTarget)
                            WeaponAttachment.SetPositionAndRotation(RightHandTarget.position, RightHandTarget.rotation);
                        break;
                    default:
                        break;
                }
            }
        }

        private void Start()
        {
            if (InitialWeapon)
            {
                ChangeWeapon(InitialWeapon);
            }
        }

        private void Update()
        {
            UpdateWeaponAttachmentTransform();
        }

        public void Setup(CharacterController owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// 改变持有武器
        /// </summary>
        /// <param name="newWeapon"></param>
        /// <param name="combo">当前武器是否只是为了Combo切换</param>
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
            CurrentWeapon.Setup(Owner);
            if (!CurrentWeapon.InitializeOnStart)
                CurrentWeapon.Initialize();
        }

        /// <summary>
        /// 使用武器
        /// </summary>
        public void ShootStart()
        {
            if (CurrentWeapon == null)
            {
                return;
            }
            CurrentWeapon.WeaponInputStart();
        }
    }
}
