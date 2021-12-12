using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private FiniteStateMachine _fsm;
    private HunterBehaviour _hunter;
    private int exitStatus;

    public ChaseState(FiniteStateMachine fsm, HunterBehaviour hunter)
    {
        _fsm = fsm;
        _hunter = hunter;
    }
    
    public void OnStart()
    {
        Debug.Log("Bird detected. Chasing!");
    }

    public void OnUpdate()
    {
        _hunter.CheckForBirds();                                                //vemos si siguen habiendo Boids en el rango de vision del Hunter.
        if (_hunter.target == null || !_hunter.target.gameObject.activeSelf)
        {
            exitStatus = 0;
            _fsm.ChangeState(HunterState.PATROL);           //Si no hay, a patrullar.
        }
        
        else if (Vector3.Distance(_hunter.transform.position, _hunter.target.transform.position) <= _hunter.killRadius)         //Si esta en el rango de matar, que mate.
        {
            exitStatus = 1;
            _hunter.target.gameObject.SetActive(false);
            _fsm.ChangeState(HunterState.PATROL);
        }
        
        else if (_hunter.energy > 0)
        {
            //BoidAgent tarAgent = _hunter.target.GetComponent<BoidAgent>();
            //if (tarAgent == null) return;
            //Vector3 futurePos = _hunter.target.transform.position + tarAgent.GetVelocity() * _hunter.futureTime;
            //_hunter.futurePosObject.transform.position = futurePos;
            
            Vector3 desired = _hunter.target.transform.position - _hunter.transform.position;
            desired.Normalize();
            desired *= _hunter.speed;

            Vector3 steering = desired - _hunter.GetVelocity();
            steering = Vector3.ClampMagnitude(steering, _hunter.maxForce);                  //Nos acercamos hacia el Boid a chasear.
            
            _hunter.ApplyForce(steering);
            _hunter.transform.position += _hunter.GetVelocity() * Time.deltaTime;
            _hunter.transform.forward = _hunter.GetVelocity().normalized;
            _hunter.energy -= Time.fixedDeltaTime * 1.5f;
        }
        else
        {
            exitStatus = 2;
            _fsm.ChangeState(HunterState.IDLE);         //Si no hay energia, a descanzar.
        }
    }

    public void OnExit()
    {
        switch (exitStatus)
        {
            case 0:
                Debug.Log("No birds in sight. Patrolling...");
                _hunter.speed = _hunter.maxSpeed;
                break;
            case 1:
                Debug.Log("Bird killed. Patrolling...");
                _hunter.speed = _hunter.maxSpeed;
                break;
            case 2:
                Debug.Log("Too tired. Resting...");
                break;
        }
    }
}
