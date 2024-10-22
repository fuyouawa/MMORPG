using MMORPG.Common.Proto.Fight;
using MMORPG.System;
using QFramework;
using Serilog;
using UnityEngine.EventSystems;

namespace MMORPG.Game
{
    public class LocalPlayerAttack : LocalPlayerAbility, IController
    {
        private bool _prepareFire;
        private INetworkSystem _network;

        public override void OnStateInit()
        {
            _network = this.GetSystem<INetworkSystem>();
        }

        public override void OnStateEnter()
        {
            Spell();
        }

        /// <summary>
        /// 发送攻击请求, 在响应成功后正式攻击
        /// </summary>
        public async void Spell()
        {
            if (_prepareFire) return;

            if (OwnerState.Brain.HandleWeapon == null) return;

            var weapon = OwnerState.Brain.HandleWeapon.CurrentWeapon;

            if (weapon == null) return;

            if (weapon.CanUse)
            {
                _prepareFire = true;
                _network.SendToServer(new SpellRequest()
                {
                    Info = new()
                    {
                        SkillId = weapon.WeaponId,
                        CasterId = Brain.ActorController.Entity.EntityId
                    }
                });

                var response = await _network.ReceiveAsync<SpellFailResponse>();

                if (response.Reason == CastResult.Success)
                {
                    OwnerState.Brain.HandleWeapon.ShootStart();
                }
                else
                {
                    Log.Error($"攻击请求失败! 原因:{response.Reason}");
                }
                _prepareFire = false;
            }
        }

        [StateCondition]
        public bool InputFire()
        {
            return Brain.InputControls.Player.Fire.inProgress && !EventSystem.current.IsPointerOverGameObject();
        }

        public override void OnStateExit()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
