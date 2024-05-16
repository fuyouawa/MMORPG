using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class ComboWeapon : MonoBehaviour
    {
        public bool DroppableCombo = true;
        public float DropComboDelay = 1f;

        [ReadOnly]
        [ShowInInspector]
        public int CurrentWeaponIndex { get; private set; }

        [ReadOnly]
        public Weapon[] Weapons;

        public Weapon CurrentWeapon => Weapons[CurrentWeaponIndex];

        public PlayerBrain OwnerBrain { get; private set; }

        private float _timeSinceLastWeaponStopped;
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
                if (_inCombo && DroppableCombo)
                {
                    _timeSinceLastWeaponStopped += Time.deltaTime;
                    if (_timeSinceLastWeaponStopped > DropComboDelay)
                    {
                        _inCombo = false;

                        CurrentWeaponIndex = 0;
                        OwnerBrain.HandleWeapon.ChangeWeapon(CurrentWeapon);
                    }
                }
            }
        }

        protected virtual void OnWeaponStarted(Weapon weapon)
        {
            _inCombo = false;
        }

        protected virtual void OnWeaponStopped(Weapon weapon)
        {
            ProceedToNextCombo();
        }

        protected virtual void ProceedToNextCombo()
        {
            OwnerBrain = CurrentWeapon.Brain;

            if (OwnerBrain != null)
            {
                if (Weapons.Length > 1)
                {
                    var newIndex = 0;
                    if (CurrentWeaponIndex < Weapons.Length - 1)
                    {
                        newIndex = CurrentWeaponIndex + 1;
                    }

                    _inCombo = true;
                    _timeSinceLastWeaponStopped = 0f;

                    CurrentWeaponIndex = newIndex;
                    OwnerBrain.HandleWeapon.ChangeWeapon(CurrentWeapon);
                }
            }
        }
    }
}
