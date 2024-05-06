using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PlayerState
{
    [Required("The status name cannot be empty!")]
    public string Name = "TODO";

    [InfoBox("Actions cannot be empty!", InfoMessageType.Error, "IsEmptyActions")]
    [TabGroup("Actions")]
    [TableList(AlwaysExpanded = true)]
    public PlayerAction[] Actions;

    [InfoBox("Transitions cannot be empty!", InfoMessageType.Error, "IsEmptyTransitions")]
    [TabGroup("Transitions")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Label")]
    public PlayerTransition[] Transitions;

    public delegate void TransitionEvaluatedHandler(PlayerTransition transition, bool condition);
    public event TransitionEvaluatedHandler OnTransitionEvaluated;

#if UNITY_EDITOR
    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        Transitions?.ForEach(x => x.OwnerState = this);
        Actions?.ForEach(x => x.OwnerState = this);
    }

    private bool IsEmptyActions => Actions == null || Actions.Length == 0;
    private bool IsEmptyTransitions => Transitions == null || Transitions.Length == 0;
#endif

    public PlayerBrain Brain { get; set; }

    public PlayerAbility[] GetAttachAbilities()
    {
        return Brain == null ? Array.Empty<PlayerAbility>() : Brain.GetAttachAbilities();
    }

    public void Initialize(PlayerBrain brain)
    {
        Brain = brain;
        foreach (var transition in Transitions)
        {
            transition.Initialize(this);
            transition.OnEvaluated += condition => OnTransitionEvaluated?.Invoke(transition, condition);
        }

        foreach (var action in Actions)
        {
            action.Initialize(this);
            action.Ability.Brain = Brain;
            action.Ability.OnStateInit();
        }
    }

    public void Enter()
    {
        foreach (var action in Actions)
        {
            action.Ability.OnStateEnter();
        }
    }

    public void Update()
    {
        foreach (var action in Actions)
        {
            action.Ability.OnStateUpdate();
        }
    }

    public void FixedUpdate()
    {
        foreach (var action in Actions)
        {
            action.Ability.OnStateFixedUpdate();
        }
    }

    public void NetworkFixedUpdate()
    {
        foreach (var action in Actions)
        {
            action.Ability.OnStateNetworkFixedUpdate();
        }
    }

    public void Exit()
    {
        foreach (var action in Actions)
        {
            action.Ability.OnStateExit();
        }
    }

    public void EvaluateTransitions()
    {
        foreach (var transition in Transitions)
        {
            transition.Evaluate();
        }
    }
}

public class StateConditionAttribute : Attribute { }
