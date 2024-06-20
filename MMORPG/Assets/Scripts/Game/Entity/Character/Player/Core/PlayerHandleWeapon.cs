using MMORPG.Event;
using MMORPG.Tool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class PlayerHandleWeapon : MonoBehaviour
    {
        [Title("Weapon")]
        [AssetsOnly]
        [Tooltip("初始化时持有的武器")]
        public Weapon InitialWeapon;

        [Title("Binding")]
        [Tooltip("武器附加位置")]
        public Transform WeaponAttachment;

        public Weapon CurrentWeapon { get; private set; }
        public ComboWeapon CurrentComboWeapon { get; private set; }

        public delegate void WeaponChangedHandler(Weapon current, Weapon previous);
        public event WeaponChangedHandler OnWeaponChanged;

        public PlayerBrain Brain { get; private set; }

        private void Start()
        {
            if (InitialWeapon)
            {
                ChangeWeapon(InitialWeapon);
            }
        }

        private void Update()
        {
        }

        public void Setup(PlayerBrain owner)
        {
            Brain = owner;
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
                CurrentWeapon = Instantiate(newWeapon);

                // 如果不是Mine, 关闭Combo切换
                if (CurrentWeapon.TryGetComponent(out ComboWeapon comboWeapon))
                {
                    if (!Brain.IsMine)
                        comboWeapon.DroppableCombo = false;
                }

                CurrentComboWeapon = comboWeapon;
            }
            else
            {
                CurrentWeapon = newWeapon;
            }
            CurrentWeapon.transform.SetParent(WeaponAttachment, false);
            CurrentWeapon.Setup(this);
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
