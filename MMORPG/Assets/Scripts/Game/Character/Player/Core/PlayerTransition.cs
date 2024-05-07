using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PlayerTransition
{
#if UNITY_EDITOR
    public string Label = "TODO";
#endif

    [InfoBox("StateConditions cannot be empty!", InfoMessageType.Error, "IsEmptyStateConditions")]
    [TableList(AlwaysExpanded = true)]
    public PlayerStateCondition[] StateConditions;

    [InfoBox("At least one of the \"TrueState\" and \"FalseState\" is not empty!", InfoMessageType.Error, "IsTrueOrFalseStateEmpty")]

    [InfoBox("Invalid state!", InfoMessageType.Error, "CheckTrueStateNameInvalid")]
    [TitleGroup("Branch")]
    [LabelText("TrueState")]
    [ValueDropdown("GetAllStateNames")]
    public string TrueStateName = string.Empty;

    [InfoBox("Invalid state!", InfoMessageType.Error, "CheckFalseStateNameInvalid")]
    [TitleGroup("Branch")]
    [LabelText("FalseState")]
    [ValueDropdown("GetAllStateNames")]
    public string FalseStateName = string.Empty;

    public PlayerState TrueState { get; private set; }
    public PlayerState FalseState { get; private set; }

    public PlayerState OwnerState { get; set; }

    public event Action<bool> OnEvaluated;

    public void Initialize(PlayerState state)
    {
        OwnerState = state;
        Debug.Assert(StateConditions.Length > 0);
        foreach (var condition in StateConditions)
        {
            condition.Initialize(this);
        }
        if (TrueStateName != string.Empty)
        {
            TrueState = OwnerState.Brain.GetState(TrueStateName);
            Debug.Assert(TrueState != null, "TrueState != null");
        }
        
        if (FalseStateName != string.Empty)
        {
            FalseState = OwnerState.Brain.GetState(FalseStateName);
            Debug.Assert(FalseState != null, "FalseState != null");
        }
    }


    public void Evaluate()
    {
        bool res = true;
        foreach (var binder in StateConditions)
        {
            res &= binder.Invoke();
            if (!res) break;
        }
        if (res)
        {
            if (TrueState != null)
            {
                OnEvaluated?.Invoke(true);
            }
        }
        else
        {
            if (FalseState != null)
            {
                OnEvaluated?.Invoke(false);
            }
        }
    }


#if UNITY_EDITOR
    private bool IsEmptyStateConditions => StateConditions == null || StateConditions.Length == 0;
    private bool IsTrueOrFalseStateEmpty => TrueStateName == string.Empty && FalseStateName == string.Empty;

    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        StateConditions?.ForEach(x => x.OwnerTransition = this);
    }

    private IEnumerable GetAllStateNames()
    {
        var total = new ValueDropdownList<string> { { "None State", string.Empty } };

        if (OwnerState?.Brain?.States != null)
        {
            total.AddRange((
                from state in OwnerState.Brain.States
                select state.Name == string.Empty ? "EMPTY" : state.Name
            ).Select((x, i) => new ValueDropdownItem<string>($"{i} - {x}", x)));
        }

        return total;
    }

    // private string GetLabelName()
    // {
    //     if (StateConditions == null || StateConditions.Length == 0)
    //         return "TODO";
    //
    //     return StateConditions.Select(x =>
    //         (x.Not ? "!" : "") +
    //         (x.MethodName == string.Empty ? "NONE" : x.MethodName)
    //     ).StringJoin(" && ");
    // }

    private bool CheckTrueStateNameInvalid()
    {
        if (TrueStateName == string.Empty || OwnerState?.Brain == null)
            return false;
        return OwnerState.Brain.GetState(TrueStateName) == null;
    }
    private bool CheckFalseStateNameInvalid()
    {
        if (FalseStateName == string.Empty || OwnerState?.Brain == null)
            return false;
        return OwnerState.Brain.GetState(FalseStateName) == null;
    }
#endif
}
