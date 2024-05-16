using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class ComboWeapon : MonoBehaviour
    {
        public bool DroppableCombo = true;
        public float DropComboDelay = 1f;
        public float ComboCoolTime = 1f;

        [ReadOnly]
        [ShowInInspector]
        public int CurrentWeaponIndex { get; private set; }

        [ReadOnly]
        public Weapon[] Weapons;

        public Weapon CurrentWeapon => Weapons[CurrentWeaponIndex];

        public PlayerBrain OwnerBrain { get; private set; }

        private float _timeSinceComboFinished;
        private float _timeSinceLastWeaponStopped;
        private bool _inComboCooling;
        private bool _inCombo;

        public Weapon[] GetAttachedWeapons()
        {
            return GetComponents<Weapon>();
        }

        protected virtual void Awake()
        {
        }

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
            InitializeUnusedWeapons();
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
                if (_inCombo && DroppableCombo && Time.time - _timeSinceLastWeaponStopped < ComboCoolTime)
                {
                    _inCombo = false;
                    CurrentWeaponIndex = 0;
                    OwnerBrain.HandleWeapon.ChangeWeapon(CurrentWeapon, true);
                }
            }

            if (_inComboCooling && Time.time - _timeSinceComboFinished < ComboCoolTime)
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

        protected virtual void ProceedToNextCombo()
        {
            OwnerBrain = CurrentWeapon.Brain;
            Debug.Assert(OwnerBrain != null);

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
