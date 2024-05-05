using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public PlayerBrain Brain { get; set; }
    public Entity Entity => Brain.Character.Entity;

    public virtual IEnumerable<MethodInfo> GetStateConditions()
    {
        return from method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            where method.HasAttribute<StateConditionAttribute>()
            select method;
    }

    public virtual void OnStateInit() {}

    public virtual void OnStateEnter() {}

    public virtual void OnStateUpdate() {}

    public virtual void OnStateFixedUpdate() {}

    public virtual void OnStateNetworkFixedUpdate() {}

    public virtual void OnStateExit() {}
}
