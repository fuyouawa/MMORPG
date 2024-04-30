using System.Collections.Generic;
using System;

[Serializable]
public class CharacterTransition
{
    [DropdownRuntime("GetAttachedConditions")]
    public CharacterCondition Condition;
    public string ExtraParam;
    [DropdownRuntime("GetAllStateName", "TrueState")]
    public string TrueStateName;
    [DropdownRuntime("GetAllStateName", "FalseState")]
    public string FalseStateName;

    public CharacterState TrueState { get; set; }
    public CharacterState FalseState { get; set; }
}

[Serializable]
public class CharacterState
{
    public string StateName;
    [DropdownRuntime("GetAttachedActions")]
    public List<CharacterAction> Actions = new();
    public List<CharacterTransition> Transitions = new();

    public CharacterBrain Brain { get; set; }

    public void Enter()
    {
        foreach (var action in Actions)
        {
            action.Character = Brain.Character;
            action.OnStateEnter();
        }

        foreach (var transition in Transitions)
        {
            transition.Condition.Character = Brain.Character;
            transition.Condition.ExtraParam = transition.ExtraParam;
            transition.TrueState ??= Brain.GetState(transition.TrueStateName);
            transition.FalseState ??= Brain.GetState(transition.FalseStateName);
            transition.Condition.OnStateEnter();
        }
    }

    public void Update()
    {
        foreach (var action in Actions)
            action.OnStateUpdate();
        foreach (var transition in Transitions)
            transition.Condition.OnStateUpdate();
    }

    public void FixedUpdate()
    {
        foreach (var action in Actions)
            action.OnStateFixedUpdate();
        foreach (var transition in Transitions)
            transition.Condition.OnStateFixedUpdate();
    }

    public void NetworkFixedUpdate()
    {
        foreach (var action in Actions)
            action.OnStateNetworkFixedUpdate();
        foreach (var transition in Transitions)
            transition.Condition.OnStateNetworkFixedUpdate();
    }

    public void Exit()
    {
        foreach (var action in Actions)
            action.OnStateExit();
        foreach (var transition in Transitions)
            transition.Condition.OnStateExit();
    }

    public void EvaluateTransitions()
    {
        foreach (var transition in Transitions)
        {
            bool cond = transition.Condition.OnStateCondition();
            if (cond)
            {
                if (transition.TrueState != null)
                    Brain.ChangeState(transition.TrueState);
            }
            else
            {
                if (transition.FalseState != null)
                    Brain.ChangeState(transition.FalseState);
            }
        }
    }
}
