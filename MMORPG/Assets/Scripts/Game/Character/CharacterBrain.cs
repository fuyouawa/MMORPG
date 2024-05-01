using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Malee.List;
using UnityEngine;



public class CharacterBrain : MonoBehaviour
{
    public Character Character;
    public List<CharacterState> States = new();
    public CharacterState CurrentState { get; private set; }


    public CharacterCondition[] GetAttachedConditions()
    {
        return GetComponents<CharacterCondition>();
    }
    public CharacterAction[] GetAttachedActions()
    {
        return GetComponents<CharacterAction>();
    }

    public string[] GetAllStateName()
    {
        return States.Select(s => s.StateName).ToArray();
    }

    public CharacterState GetState(string stateName)
    {
        return States.FirstOrDefault(x => x.StateName == stateName);
    }

    public void ChangeState(CharacterState state)
    {
        Debug.Assert(States.Contains(state));
        state.Brain = this;
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }

    private void StartState(CharacterState state)
    {
        Debug.Assert(States.Contains(state));
        state.Brain = this;
        CurrentState = state;
        CurrentState.Enter();
    }

    private void Awake()
    {
        if (States.Count == 0)
            throw new Exception($"({this})的状态机为空, 这是没有意义的!");
    }

    private void Start()
    {
        StartState(States[0]);
        StartCoroutine(NetworkFixedUpdate());
    }

    private void Update()
    {
        CurrentState?.Update();
        CurrentState?.EvaluateTransitions();
    }

    private void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }


    private IEnumerator NetworkFixedUpdate()
    {
        var deltaTime = Config.GameConfig.NetworkSyncDeltaTime;
        while (true)
        {
            CurrentState?.NetworkFixedUpdate();
            yield return new WaitForSeconds(deltaTime);
        }
    }
}
