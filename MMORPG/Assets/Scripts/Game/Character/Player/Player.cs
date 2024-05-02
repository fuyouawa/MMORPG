using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MMORPG;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Character Character;
    public List<PlayerState> States = new();
    public List<PlayerTransition> AnyStateTransitions = new();
    public GameInputControls InputControls { get; private set; }
    public PlayerState CurrentState { get; private set; }

    public PlayerAction[] GetAttachActions()
    {
        return GetComponents<PlayerAction>();
    }

    public void ChangeState(PlayerState state)
    {
        Debug.Assert(States.Contains(state));
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
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
        InitStates();
    }

    private void Start()
    {
        if (States.Count > 0)
        {
            ChangeState(States[0]);
        }
        StartCoroutine(NetworkFixedUpdate());
    }
        
    private void Update()
    {
        foreach (var transition in AnyStateTransitions)
        {
            if (transition.Evaluate())
                break;
        }
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
            state.Initialize(this);

        foreach (var transition in AnyStateTransitions)
            transition.Initialize(this);
    }
}
