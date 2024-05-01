using System;
using UnityEngine;

public abstract class CharacterCondition : MonoBehaviour
{
    public Character Character { get; set; }

    public abstract bool OnStateCondition();

    public virtual void OnStateEnter() { }

    public virtual void OnStateUpdate() { }

    public virtual void OnStateFixedUpdate() { }

    public virtual void OnStateNetworkFixedUpdate() { }

    public virtual void OnStateExit() { }
}


[Serializable]
public class CharacterTransition
{
    public CharacterCondition Condition;
    [DropdownRuntime("GetAllStateName", "TrueState")]
    public string TrueStateName;
    [DropdownRuntime("GetAllStateName", "FalseState")]
    public string FalseStateName;

    public CharacterState TrueState { get; set; }
    public CharacterState FalseState { get; set; }
}
