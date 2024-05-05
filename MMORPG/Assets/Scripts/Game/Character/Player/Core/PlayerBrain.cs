using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MMORPG;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    public Character Character;

    [InfoBox("Empty state machine is meaningless", InfoMessageType.Warning, "IsEmptyStates")]
    [InfoBox("The state machine name cannot be the same!", InfoMessageType.Error, "HasRepeatStateName")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Name")]
    public PlayerState[] States;
    public PlayerState CurrentState { get; private set; }
    public GameInputControls InputControls { get; private set; }

    public Vector2 CurrentMovementDirection { get; private set; }

    public PlayerAbility[] GetAttachAbilities()
    {
        var total = new List<PlayerAbility>();
        total.AddRange(GetComponents<PlayerAbility>());
        foreach (var node in Character.AdditionalAbilityNodes)
        {
            total.AddRange(node.GetComponents<PlayerAbility>());
        }
        return total.ToArray();
    }
    
#if UNITY_EDITOR
    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        States?.ForEach(x => x.Brain = this);
    }

    private bool HasRepeatStateName => States.GroupBy(x => x.Name).Any(g => g.Count() > 1);

    private bool IsEmptyStates => States.Length == 0;
#endif

    public void ChangeState(PlayerState state)
    {
        Debug.Assert(state != null);
        Debug.Assert(States.Contains(state));
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }

    public PlayerState GetState(string stateName)
    {
        return Array.Find(States, x => x.Name == stateName);
    }


    // private void StartState(PlayerState state)
    // {
    //     Debug.Assert(States.Contains(state));
    //     CurrentState = state;
    //     CurrentState.Enter();
    // }

    private void Awake()
    {
        InputControls = new();
        if (States.Length == 0) return;
        InitStates();
    }

    private void Start()
    {
        if (States.Length == 0) return;
        Debug.Log(States[0].Name);
        ChangeState(States[0]);
        StartCoroutine(NetworkFixedUpdate());
    }
        
    private void Update()
    {
        if (States.Length == 0) return;
        UpdateInputValues();
        CurrentState?.Update();
        CurrentState?.EvaluateTransitions();
    }
    
    private void FixedUpdate()
    {
        if (States.Length == 0) return;
        CurrentState?.FixedUpdate();
    }
    
    
    private IEnumerator NetworkFixedUpdate()
    {
        var deltaTime = 0.1f;
        // var deltaTime = Config.GameConfig.NetworkSyncDeltaTime;
        while (true)
        {
            CurrentState?.NetworkFixedUpdate();
            yield return new WaitForSeconds(deltaTime);
        }
    }

    private void OnEnable()
    {
        InputControls.Enable();
    }

    private void OnDisable()
    {
        InputControls.Disable();
    }

    private void InitStates()
    {
        foreach (var state in States)
        {
            state.Initialize(this);
            state.OnTransitionEvaluated += (transition, condition) =>
            {
                ChangeState(condition ? transition.TrueState : transition.FalseState);
            };
        }
    }


    private void UpdateInputValues()
    {
        CurrentMovementDirection = InputControls.Player.Move.ReadValue<Vector2>();
    }
}
