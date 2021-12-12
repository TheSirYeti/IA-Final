using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using Random = System.Random;

public class BoidAgent : MonoBehaviour
{
    [Header("Targets")]
    public GameObject fleeTarget;
    public GameObject seekTarget;

    [Header("Data values")]
    private Vector3 _velocity;
    public float maxSpeed;
    public float maxForce;
    public float viewDistance;
    public float separationDistance;

    [Header("Arrive")]
    public float arriveRadius;
    public float eatRadius;
    
    [Header("Evade")]
    public float evadeRadius;

    [Header("Weights")]
    public float separationWeightValue;
    public float alignWeightValue;
    public float cohesionWeightValue;

    [Header("Map Bounds Values")] 
    public float mapXbounds;
    public float mapZbounds;

    private void Start()
    {
        BoidManager.instance.AddBoid(this);
        
        ApplyForce(new Vector3(UnityEngine.Random.Range(0, maxSpeed), 0, UnityEngine.Random.Range(0, maxSpeed)).normalized);
    }

    void Update()
    {
        CheckMapBounds();
        TakeAction();

        _velocity.y = 0;
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity.normalized;

    }

    void TakeAction()
    {
        if(Vector3.Distance(fleeTarget.transform.position, transform.position) < evadeRadius)
            Evade();
        else if(seekTarget != null && Vector3.Distance(seekTarget.transform.position, transform.position) < arriveRadius)
            Arrive();
        else ApplyForce(Separation() * separationWeightValue + Align() * alignWeightValue + Cohesion() * cohesionWeightValue);
    }

    void Arrive()
    {
        if (seekTarget != null)
        {
            Vector3 desired = seekTarget.transform.position - transform.position;
            if (desired.magnitude < arriveRadius)
            {
                if (desired.magnitude < eatRadius)
                {
                    EventManager.Trigger("DisablePreviousFood");
                }
                float speed = maxSpeed * (desired.magnitude / arriveRadius);
                desired.Normalize();
                desired *= speed;
            }
            else
            {
                desired.Normalize();
                desired *= maxSpeed;
            }

            Vector3 steering = desired - _velocity;
            steering = Vector3.ClampMagnitude(steering, maxForce);

            ApplyForce(steering);
        }
    }

    void Evade()
    {
        Vector3 desired = fleeTarget.transform.position  - transform.position;
        desired.Normalize();
        desired *= maxSpeed; 
        desired *= -1;

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        ApplyForce(steering);
    }

    Vector3 Cohesion()
    {
        Vector3 desired = new Vector3();
        int nearbyBoids = 0;

        foreach (var boid in BoidManager.instance.allBoids)
        {
            if (boid != this && Vector3.Distance(boid.transform.position, transform.position) < viewDistance)
            {
                desired += boid.transform.position;
                nearbyBoids++;
            }
        }
        if (nearbyBoids == 0) return desired;
        desired /= nearbyBoids;
        desired = desired - transform.position;
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        return steering;
    }

    Vector3 Align()
    {
        Vector3 desired = new Vector3();
        int nearbyBoids = 0;
        foreach (var boid in BoidManager.instance.allBoids)
        {
            if (boid != this && Vector3.Distance(boid.transform.position, transform.position) < viewDistance)
            {
                desired += boid._velocity;
                nearbyBoids++;
            }
        }
        if (nearbyBoids == 0) 
            return Vector3.zero;
        
        desired = desired / nearbyBoids;
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steering = Vector3.ClampMagnitude(desired - _velocity, maxForce);

        return steering;
    }

    Vector3 Separation()
    {
        Vector3 desired = new Vector3();
        int nearbyBoids = 0;

        foreach (var item in BoidManager.instance.allBoids)
        {
            Vector3 dist = item.transform.position - transform.position;

            if (item != this && dist.magnitude < separationDistance)
            {
                desired.x += dist.x;
                desired.z += dist.z;
                nearbyBoids++;
            }
        }
        if (nearbyBoids == 0) return desired;
        desired /= nearbyBoids;
        desired.Normalize();
        desired *= maxSpeed;
        desired = -desired;

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        return steering;
    }
    
    
    void ApplyForce(Vector3 force)
    {
        _velocity += force;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }
    
    void CheckMapBounds()
    {
        if (transform.position.z > mapZbounds) 
            transform.position = new Vector3(transform.position.x, transform.position.y, -mapZbounds);
        
        if (transform.position.z < -mapZbounds) 
            transform.position = new Vector3(transform.position.x, transform.position.y, mapZbounds);
        
        if (transform.position.x < -mapXbounds) 
            transform.position = new Vector3(mapXbounds, transform.position.y, transform.position.z);
        
        if (transform.position.x > mapXbounds) 
            transform.position = new Vector3(-mapXbounds, transform.position.y, transform.position.z);
    }
}

