using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PlayerAction
{
    //TODO AbilityName check valid
    [Required("Can't be none!")]
    [VerticalGroup("Local Ability")]
    [ValueDropdown("GetLocalAbilityDropdown")]
    [HideLabel]
    public string LocalAbilityName = string.Empty;

    [Required("Can't be none!")]
    [VerticalGroup("Remote Ability")]
    [ValueDropdown("GetRemoteAbilityDropdown")]
    [HideLabel]
    public string RemoteAbilityName = string.Empty;

    public LocalPlayerAbility LocalAbility {get; private set; }
    public RemotePlayerAbility RemoteAbility { get; private set; }
    public PlayerState OwnerState { get; set; }
    public int OwnerStateId { get; set; }

    public bool IsMine { get; private set; }

    public void Initialize(PlayerState state, int stateId)
    {
        OwnerState = state;
        OwnerStateId = stateId;
        IsMine = OwnerState.Brain.CharacterController.Entity.IsMine;

        if (IsMine)
        {
            LocalAbility = OwnerState.Brain.GetAttachLocalAbilities().First(x => x.GetType().Name == LocalAbilityName);

            LocalAbility.OwnerState = state;
            LocalAbility.Brain = state.Brain;
            LocalAbility.OwnerStateId = stateId;

            LocalAbility.OnStateInit();
        }
        else
        {
            RemoteAbility = OwnerState.Brain.GetAttachRemoteAbilities().First(x => x.GetType().Name == RemoteAbilityName);

            RemoteAbility.OwnerState = state;
            RemoteAbility.Brain = state.Brain;
            RemoteAbility.OwnerStateId = stateId;

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
    public IEnumerable GetLocalAbilityDropdown()
    {
        var total = new ValueDropdownList<string> { { "None Local Ability", string.Empty } };
        if (OwnerState?.Brain != null)
        {
            total.AddRange(OwnerState.Brain.GetAttachLocalAbilities().Select((x, i) =>
                new ValueDropdownItem<string>($"{i} - {x.GetType().Name}", x.GetType().Name))
            );
        }
        return total;
    }
    public IEnumerable GetRemoteAbilityDropdown()
    {
        var total = new ValueDropdownList<string> { { "None Remote Ability", string.Empty } };
        if (OwnerState?.Brain != null)
        {
            total.AddRange(OwnerState.Brain.GetAttachRemoteAbilities().Select((x, i) =>
                new ValueDropdownItem<string>($"{i} - {x.GetType().Name}", x.GetType().Name))
            );
        }
        return total;
    }
#endif
}
