using System;
using UnityEngine;

 namespace MMORPG.Game
{
    public class PlayerHandleWeapon : MonoBehaviour
    {
        public PlayerBrain Brain { get; private set; }

        public delegate void WeaponChangedHandler(Weapon current, Weapon previous);
        public event WeaponChangedHandler OnWeaponChanged;

        public void ChangeWeapon(Weapon newWeapon)
        {
            
        }

        public void Setup(PlayerBrain brain)
        {
            Brain = brain;
        }
    }
}
