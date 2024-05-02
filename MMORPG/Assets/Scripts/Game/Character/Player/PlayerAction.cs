using Malee.List;
using System;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public Character Character { get; set; }

    public virtual void OnStateEnter() {}

    public virtual void OnStateUpdate() {}

    public virtual void OnStateFixedUpdate() {}

    public virtual void OnStateNetworkFixedUpdate() {}

    public virtual void OnStateExit() {}
}

[Serializable]
public class PlayerActionArray : ReorderableArray<PlayerAction> { }
