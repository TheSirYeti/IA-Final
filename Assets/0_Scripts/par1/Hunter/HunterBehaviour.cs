using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterBehaviour : MonoBehaviour
{
    private FiniteStateMachine _fsm;

    [Header("Pursuit Target")] 
    public GameObject target;
    public float detectionRadius;
    public float killRadius;
    public GameObject futurePosObject;
    public float futureTime;
    
    [Header("Stats")]
    public float energy;
    public float maxEnergy;
    private Vector3 _velocity;
    public float speed;
    public float chaseSpeed;
    public float maxSpeed;
    public float maxForce;

    [Header("Waypoints")] 
    public List<Transform> waypoints = new List<Transform>();
    public int currentWaypoint = 0;

    private void Start()
    {
        _fsm = new FiniteStateMachine();
        _fsm.AddState(HunterState.IDLE, new IdleState(this, _fsm));
        _fsm.AddState(HunterState.PATROL, new PatrolState(_fsm, this, waypoints));
        _fsm.AddState(HunterState.CHASE, new ChaseState(_fsm, this));
        _fsm.ChangeState(HunterState.PATROL);
        maxSpeed = speed;
    }

    public void CheckForBirds()
    {
        bool flag = false;
        
        foreach (BoidAgent boid in BoidManager.instance.allBoids)
        {
            if ((target == null || boid.gameObject != target.gameObject) && boid.gameObject.activeSelf) //Si no es nulo, no es el target y sigue vivo, entra.
            {
                float dist = Vector3.Distance(boid.transform.position, transform.position);
            
                if (dist <= detectionRadius)
                {
                    flag = true;
                    if (target == null || dist < Vector3.Distance(target.transform.position, transform.position))
                    {
                        target = boid.gameObject;
                    }
                }
            }
        }

        if (!flag) target = null;   //Si no detecto Boids, no hay target.
    }
    
    private void Update()
    {
        _fsm.OnUpdate();
    }
    
    public Vector3 GetVelocity()
    {
        return _velocity;
    }
    
    public void ApplyForce(Vector3 force)
    {
        _velocity += force;
        _velocity.y = 0;
    }
}
