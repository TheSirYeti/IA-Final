using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class IdleState : IState
{
    private HunterBehaviour _hunter;
    private FiniteStateMachine _fsm;

    public IdleState(HunterBehaviour hunter, FiniteStateMachine fsm)
    {
        _hunter = hunter;
        _fsm = fsm;
    }
    public void OnStart()
    {
        Debug.Log("Resting...");
    }

    public void OnUpdate()
    {
        _hunter.energy += Time.fixedDeltaTime * 2;
        if (_hunter.energy >= _hunter.maxEnergy)        //Si ya recupero la energia, que vuelva a patrullar.
        {
            _hunter.energy = _hunter.maxEnergy;
            _fsm.ChangeState(HunterState.PATROL);
        }
    }

    public void OnExit()
    {
        Debug.Log("Stopped Resting.");
    }
}
