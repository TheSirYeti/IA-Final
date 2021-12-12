using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    IState _currentState = new NullState();
    Dictionary<HunterState, IState> _allStates = new Dictionary<HunterState, IState>();


    public void OnUpdate()
    {
        _currentState.OnUpdate();
    }

    public void AddState(HunterState id, IState state)
    {
        if (_allStates.ContainsKey(id)) return;

        _allStates.Add(id, state);
    }

    public void ChangeState(HunterState id)
    {
        if (!_allStates.ContainsKey(id)) return;
        _currentState.OnExit();
        _currentState = _allStates[id]; 
        _currentState.OnStart();
    }
}

public enum HunterState
{
    IDLE,
    PATROL,
    CHASE
}
