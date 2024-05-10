using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    public PlayerState OwnerState { get; set; }
    public PlayerBrain Brain { get; set; }
    public int OwnerStateId { get; set; }

    public virtual void OnStateInit() {}

    public virtual void OnStateEnter() {}

    public virtual void OnStateUpdate() {}

    public virtual void OnStateFixedUpdate() {}

    public virtual void OnStateNetworkFixedUpdate() {}

    public virtual void OnStateExit() { }
}
