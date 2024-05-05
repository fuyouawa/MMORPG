using System;
using System.Linq;
using Malee.List;
using UnityEngine;

[Serializable]
public class PlayerState
{
    public string Name;
    [Reorderable(sortable = false)]
    public PlayerActionArray Actions = new();
    [Reorderable(sortable = false)]
    public PlayerTransitionArray Transitions = new();

    public void Initialize(PlayerBrain brain)
    {
        foreach (var transition in Transitions)
        {
            transition.Initialize(brain);
        }

        foreach (var action in Actions)
        {
            action.Brain = brain;
        }
    }

    public void Enter()
    {
        foreach (var action in Actions)
        {
            action.OnStateEnter();
        }
    }

    public void Update()
    {
        foreach (var action in Actions)
        {
            action.OnStateUpdate();
        }
    }

    public void FixedUpdate()
    {
        foreach (var action in Actions)
        {
            action.OnStateFixedUpdate();
        }
    }

    public void NetworkFixedUpdate()
    {
        foreach (var action in Actions)
        {
            action.OnStateNetworkFixedUpdate();
        }
    }

    public void Exit()
    {
        foreach (var action in Actions)
        {
            action.OnStateExit();
        }
    }

    public void EvaluateTransitions()
    {
        foreach (var transition in Transitions)
        {
            if (transition.Evaluate())
                break;
        }
    }
}

public class StateConditionAttribute : Attribute { }
