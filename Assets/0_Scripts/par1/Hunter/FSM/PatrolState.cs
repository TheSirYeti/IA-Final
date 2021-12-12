using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private FiniteStateMachine _fsm;
    private HunterBehaviour _hunter;
    private List<Transform> _waypoints;
    private int _currentWaypoint = 0;
    private bool waypointDirection = true;
    private int exitState;

    public PatrolState(FiniteStateMachine fsm, HunterBehaviour hunter, List<Transform> waypoints)
    {
        _fsm = fsm;
        _hunter = hunter;
        _waypoints = waypoints;
    }
    
    public void OnStart()
    {
        Debug.Log("Patroling...");
    }

    public void OnUpdate()
    {
        _hunter.CheckForBirds();        //Vemos si hay Boids en el radio.
        if (_hunter.target != null)
        {
            exitState = 0;
            _fsm.ChangeState(HunterState.CHASE);        //Si lo hay, que comience a chasear.
        }
        if (_hunter.energy >= 0)
        {
            Vector3 dir = _waypoints[_currentWaypoint].transform.position - _hunter.transform.position;
            _hunter.transform.forward = dir;
            _hunter.transform.position += _hunter.transform.forward * _hunter.speed * Time.fixedDeltaTime;      //Recorremos el camino de Waypoints.

            if (dir.magnitude < 0.1f)
            {
                if (_currentWaypoint == _waypoints.Count -1)
                    waypointDirection = false;
                
                if(_currentWaypoint == 0)
                    waypointDirection = true;
                
                if (waypointDirection)
                    _currentWaypoint++;
                else _currentWaypoint--;
            }

            _hunter.energy -= Time.fixedDeltaTime;
        }
        else
        {
            exitState = 1;
            _fsm.ChangeState(HunterState.IDLE);
        }
    }

    public void OnExit()
    {
        switch (exitState)
        {
            case 0:
                Debug.Log("Found bird! Chasing!");
                _hunter.speed = _hunter.chaseSpeed;
                break;
            case 1:
                Debug.Log("Tired...");
                break;
        }
    }
}
