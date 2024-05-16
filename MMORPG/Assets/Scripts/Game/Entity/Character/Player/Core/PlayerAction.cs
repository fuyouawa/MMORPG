using System;
using System.Collections;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    [Serializable]

    public class PlayerAction
    {
        [InfoBox("Invalid ability!", InfoMessageType.Error, "CheckLocalAbilityNameInvalid")]
        [VerticalGroup("Local Ability")]
        [ValueDropdown("GetLocalAbilityDropdown")]
        [HideLabel]
        public string LocalAbilityName = string.Empty;

        [InfoBox("Invalid ability!", InfoMessageType.Error, "CheckRemoteAbilityNameInvalid")]
        [VerticalGroup("Remote Ability")]
        [ValueDropdown("GetRemoteAbilityDropdown")]
        [HideLabel]
        public string RemoteAbilityName = string.Empty;

        public LocalPlayerAbility LocalAbility { get; private set; }
        public RemotePlayerAbility RemoteAbility { get; private set; }
        public PlayerState OwnerState { get; set; }
        public int OwnerStateId { get; set; }

        public bool IsMine { get; private set; }

        public void Setup(PlayerState state, int stateId)
        {
            OwnerState = state;
            OwnerStateId = stateId;
            IsMine = OwnerState.Brain.CharacterController.Entity.IsMine;
        }

        public void Initialize()
        {
            if (IsMine)
            {
                LocalAbility = OwnerState.Brain.GetAttachLocalAbilities()
                    .First(x => x.GetType().Name == LocalAbilityName);

                LocalAbility.OwnerState = OwnerState;
                LocalAbility.Brain = OwnerState.Brain;
                LocalAbility.OwnerStateId = OwnerStateId;

                LocalAbility.OnStateInit();
            }
            else
            {
                RemoteAbility = OwnerState.Brain.GetAttachRemoteAbilities()
                    .First(x => x.GetType().Name == RemoteAbilityName);

                RemoteAbility.OwnerState = OwnerState;
                RemoteAbility.Brain = OwnerState.Brain;
                RemoteAbility.OwnerStateId = OwnerStateId;

                RemoteAbility.OnStateInit();
            }
        }

        public void Enter()
        {
            if (IsMine)
            {
                LocalAbility.OwnerState = OwnerState;
                LocalAbility.Brain = OwnerState.Brain;
                LocalAbility.OwnerStateId = OwnerStateId;
                LocalAbility.OnStateEnter();
            }
            else
            {
                RemoteAbility.OwnerState = OwnerState;
                RemoteAbility.Brain = OwnerState.Brain;
                RemoteAbility.OwnerStateId = OwnerStateId;
                RemoteAbility.OnStateEnter();
            }
        }

        public void Update()
        {
            AssertCheck();
            if (IsMine)
                LocalAbility.OnStateUpdate();
            else
                RemoteAbility.OnStateUpdate();
        }

        public void FixedUpdate()
        {
            AssertCheck();
            if (IsMine)
                LocalAbility.OnStateFixedUpdate();
            else
                RemoteAbility.OnStateFixedUpdate();
        }

        public void NetworkFixedUpdate()
        {
            AssertCheck();
            if (IsMine)
                LocalAbility.OnStateNetworkFixedUpdate();
            else
                RemoteAbility.OnStateNetworkFixedUpdate();
        }

        public void Exit()
        {
            AssertCheck();
            if (IsMine)
                LocalAbility.OnStateExit();
            else
                RemoteAbility.OnStateExit();
        }

        public void TransformEntitySync(EntityTransformSyncData data)
        {
            Debug.Assert(!IsMine);
            AssertCheck();
            RemoteAbility.OnStateNetworkSyncTransform(data);
        }

        public void AssertCheck()
        {
            Debug.Assert(IsMine == OwnerState.Brain.CharacterController.Entity.IsMine);
            if (IsMine)
            {
                Debug.Assert(LocalAbility.OwnerState == OwnerState);
                Debug.Assert(LocalAbility.Brain == OwnerState.Brain);
                Debug.Assert(LocalAbility.OwnerStateId == OwnerStateId);
            }
            else
            {
                Debug.Assert(RemoteAbility.OwnerState == OwnerState);
                Debug.Assert(RemoteAbility.Brain == OwnerState.Brain);
                Debug.Assert(RemoteAbility.OwnerStateId == OwnerStateId);
            }
        }

#if UNITY_EDITOR
        private IEnumerable GetLocalAbilityDropdown()
        {
            var total = new ValueDropdownList<string> { { "None Local Ability", string.Empty } };
            if (OwnerState?.Brain != null)
            {
                var abilities = OwnerState.Brain.GetAttachLocalAbilities();
                if (abilities == null)
                    return total;
                total.AddRange(abilities.Select((x, i) =>
                    new ValueDropdownItem<string>($"{i} - {x.GetType().Name}", x.GetType().Name))
                );
            }

            return total;
        }

        private IEnumerable GetRemoteAbilityDropdown()
        {
            var total = new ValueDropdownList<string> { { "None Remote Ability", string.Empty } };
            if (OwnerState?.Brain != null)
            {
                var abilities = OwnerState.Brain.GetAttachRemoteAbilities();
                if (abilities == null)
                    return total;
                total.AddRange(abilities.Select((x, i) =>
                    new ValueDropdownItem<string>($"{i} - {x.GetType().Name}", x.GetType().Name))
                );
            }

            return total;
        }


        private bool CheckLocalAbilityNameInvalid()
        {
            if (OwnerState?.Brain == null)
                return false;
            if (LocalAbilityName.IsNullOrEmpty())
                return true;
            var ability = OwnerState.Brain.GetAttachLocalAbilities()
                ?.FirstOrDefault(x => x.GetType().Name == LocalAbilityName);
            if (ability == null)
                return true;
            return false;
        }


        private bool CheckRemoteAbilityNameInvalid()
        {
            if (OwnerState?.Brain == null)
                return false;
            if (RemoteAbilityName.IsNullOrEmpty())
                return true;
            var ability = OwnerState.Brain.GetAttachRemoteAbilities()
                ?.FirstOrDefault(x => x.GetType().Name == RemoteAbilityName);
            if (ability == null)
                return true;
            return false;
        }

        public bool HasError()
        {
            return CheckLocalAbilityNameInvalid() || CheckRemoteAbilityNameInvalid();
        }
#endif
    }

}
