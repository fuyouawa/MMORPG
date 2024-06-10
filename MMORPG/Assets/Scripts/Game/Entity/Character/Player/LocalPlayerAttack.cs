using Common.Proto.Fight;
using MMORPG.Event;
using MMORPG.System;
using MMORPG.Tool;
using QFramework;
using Serilog;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MMORPG.Game
{
    public class LocalPlayerAttack : LocalPlayerAbility, IController
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

        private bool _prepareFire;
        private INetworkSystem _network;

        public override void OnStateInit()
        {
            _network = this.GetSystem<INetworkSystem>();
            _network.Receive<SpellFailResponse>(OnReceivedSpellFail)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<PlayerChangeWeaponEvent>(e =>
            {
                ChangeWeapon(e.NewWeapon, e.Combo);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);


            if (InitialWeapon)
            {
                ChangeWeapon(InitialWeapon);
            }
        }

        public override void OnStateEnter()
        {
            Spell();
        }

        /// <summary>
        /// 发送攻击请求, 在响应成功后正式攻击
        /// </summary>
        public void Spell()
        {
            if (_prepareFire) return;

            if (CurrentWeapon == null) return;

            if (CurrentWeapon.CanUse)
            {
                _prepareFire = true;
                _network.SendToServer(new SpellRequest()
                {
                    Info = new()
                    {
                        SkillId = CurrentWeapon.WeaponId,
                        CasterId = Brain.CharacterController.Entity.EntityId
                    }
                });

            }
        }

        private void OnReceivedSpellFail(SpellFailResponse response)
        {
            if (response.Reason == CastResult.Success)
            {
                ShootStart();
            }
            else
            {
                Log.Error($"攻击请求失败! 原因:{response.Reason}");
            }
            _prepareFire = false;
        }

        [StateCondition]
        public bool InputFire()
        {
            return Brain.InputControls.Player.Fire.inProgress;
        }

        public override void OnStateExit()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
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

            CurrentWeapon = newWeapon != null
                ? newWeapon.Spawn(WeaponAttachment, OwnerState.Brain.CharacterController, combo)
                : null;
            // OnWeaponChanged?.Invoke(newWeapon, tmp);
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

        public void OnHitDamageable(AbstractHealth health)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.OnHitEntity((Health)health);
            }
        }
    }
}
