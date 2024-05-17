using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
    public class ComboWeapon : MonoBehaviour
    {
        public bool DroppableCombo = true;
        public float DropComboDelay = 1f;
        public float ComboCoolTime = 1f;


        [ReadOnly]
        public Weapon[] Weapons;

        public int CurrentWeaponIndex { get; private set; }

        public Weapon CurrentWeapon => Weapons[CurrentWeaponIndex];

        public PlayerBrain OwnerBrain { get; private set; }

        private float _timeSinceComboFinished = -float.MaxValue;
        private float _timeSinceLastWeaponStopped = -float.MaxValue;
        private bool _inComboCooling;
        private bool _inCombo;
        private bool _fireInNextWeapon = false;


        protected virtual void Start()
        {
            Initialization();
        }

        protected virtual void Update()
        {
            ResetCombo();
        }

        /// <summary>
        /// Grabs all Weapon components and initializes them
        /// </summary>
        protected virtual void Initialization()
        {
            Weapons = GetComponents<Weapon>();
            OwnerBrain = Weapons[0].Brain;
            OwnerBrain.InputControls.Player.Fire.started += OnFireStarted;
            OwnerBrain.HandleWeapon.OnWeaponChanged += OnWeaponChanged;
            Weapons.ForEach(x =>
            {
                x.OnWeaponStarted += OnWeaponStarted;
                x.OnWeaponStopped += OnWeaponStopped;
            });
            InitializeUnusedWeapons();
        }

        protected virtual void OnWeaponChanged(Weapon current, Weapon previous)
        {
            if (_fireInNextWeapon)
            {
                current.WeaponInputStart();
                _fireInNextWeapon = false;
            }
        }

        protected virtual void InitializeUnusedWeapons()
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                if (i != CurrentWeaponIndex)
                {
                    Weapons[i].Setup(CurrentWeapon.Brain);
                }
            }
        }

        protected virtual void ResetCombo()
        {
            if (Weapons.Length > 1)
            {
                if (_inCombo && DroppableCombo && Time.time - _timeSinceLastWeaponStopped > DropComboDelay)
                {
                    _inCombo = false;
                    CurrentWeaponIndex = 0;
                    OwnerBrain.HandleWeapon.ChangeWeapon(CurrentWeapon, true);
                }
            }

            if (_inComboCooling && Time.time - _timeSinceComboFinished > ComboCoolTime)
            {
                Weapons[0].PreventFire = false;
                _inComboCooling = false;
            }
        }

        protected virtual void OnWeaponStarted(Weapon weapon)
        {
        }

        protected virtual void OnWeaponStopped(Weapon weapon)
        {
            ProceedToNextCombo();
        }

        protected virtual void OnFireStarted(InputAction.CallbackContext obj)
        {
            if (Weapons.Length > 1)
            {
                if (CurrentWeapon.TryInterrupt())
                {
                    _fireInNextWeapon = true;
                }
            }
        }

        protected virtual void ProceedToNextCombo()
        {
            Debug.Assert(OwnerBrain == CurrentWeapon.Brain);

            if (Weapons.Length > 1)
            {
                var newIndex = 0;
                if (CurrentWeaponIndex < Weapons.Length - 1)
                {
                    newIndex = CurrentWeaponIndex + 1;
                    _inCombo = true;
                }
                else
                {
                    _timeSinceComboFinished = Time.time;
                    Weapons[0].PreventFire = true;
                    _inComboCooling = true;
                    _inCombo = false;
                }

                _timeSinceLastWeaponStopped = Time.time;

                CurrentWeaponIndex = newIndex;
                OwnerBrain.HandleWeapon.ChangeWeapon(CurrentWeapon, true);
            }
        }
    }
}
