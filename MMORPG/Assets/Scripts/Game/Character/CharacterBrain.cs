using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SubjectNerd.Utilities;
using UnityEngine;

public class CharacterBrain : MonoBehaviour
{
    public Character Character;
    [SerializeField]
    [DropdownRuntime("GetAllStateName", "StartState")]
    private string _startStateName;
    [SerializeField]
    [Reorderable]
    private List<CharacterState> _states = new();

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
        return _states.Select(s => s.StateName).ToArray();
    }

    public CharacterState GetState(string stateName)
    {
        return _states.Find(s => s.StateName == stateName);
    }

    public void ChangeState(CharacterState state)
    {
        Debug.Assert(_states.Contains(state));
        state.Brain = this;
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }

    private void StartState(CharacterState state)
    {
        Debug.Assert(_states.Contains(state));
        state.Brain = this;
        CurrentState = state;
        CurrentState.Enter();
    }

    private void Awake()
    {
        if (_states.Count == 0)
            throw new Exception($"({this})的状态机为空, 这是没有意义的!");
        var startState = GetState(_startStateName)
                         ?? throw new Exception($"({this})还没有设置开始状态!");
        StartState(startState);
    }

    private void Start()
    {
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
