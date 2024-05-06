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
    public int OwnerStateId { get; set; }
    public bool IsMine => Brain.CharacterController.Entity.IsMine;

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

    public virtual void OnStateExit() { }

    public virtual void OnStateNetworkSyncTransform(EntityTransformSyncData data)
    {
        Brain.CharacterController.SmoothMove(data.Position);
        Brain.CharacterController.SmoothRotate(data.Rotation);
    }
}
