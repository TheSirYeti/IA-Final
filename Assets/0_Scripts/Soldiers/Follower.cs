using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Follower : Pathfinder
{
    [Header("Targets")]
    public GameObject seekTarget;

    [Header("Data values")]
    public float maxSpeed;
    public float maxForce;
    public float viewDistance;
    public float arriveDistance;
    public float separationDistance;

    [Header("Weights")]
    public float separationWeightValue;
    public float alignWeightValue;
    public float cohesionWeightValue;
    public float avoidWeightValue;

    [Header("Field of View")] 
    public LayerMask leaderMask;
    public LayerMask wallMask;
    public LayerMask obstacleMask;

    [Header("Logic Bools")] 
    public bool isFollowing;
    public bool isRerouting;
    public bool isAvoiding;
    
    private void Start()
    {
        SoldierManager.instance.AddFollower(this);
    }

    void Update()
    {
        Vector3 dirToTarget = (seekTarget.transform.position - transform.position);
        if (Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, wallMask) && !isPathfinding)
        {
            isRerouting = true;
            isFollowing = false;
            Gizmos.color = Color.blue;
        }
        else if(!isPathfinding && !isRerouting)
        {
            isFollowing = true;
            Gizmos.color = Color.green;
        }

        if (Physics.OverlapSphere(transform.position, viewDistance / avoidWeightValue, obstacleMask).Length > 0)
        {
            isAvoiding = true;
        }
        else
        {
            isAvoiding = false;
        }

        TakeAction();
    }

    void TakeAction()
    {
        if (isFollowing && !isAvoiding)
        {
            ApplyForce(Separation() * separationWeightValue + Cohesion() * cohesionWeightValue + Align() * alignWeightValue);
            Arrive();
            
            velocity.y = 0;
            transform.position += velocity * Time.deltaTime;
            transform.forward = velocity.normalized;
        }

        if (isRerouting)
        {
            currentNode = 0;
            int closeNode = NodeManager.instance.GetClosestNode(transform);
            int endNode = NodeManager.instance.GetClosestNode(seekTarget.transform);
                            
            if (closeNode != -1 && endNode != -1)
            {
                nodePath = ConstructPathThetaStar(NodeManager.instance.nodes[closeNode], NodeManager.instance.nodes[endNode]);
            }

            isRerouting = false;
            isPathfinding = true;
        }
        
        if(isPathfinding && !isAvoiding)
        {
            Vector3 dirToTarget = (seekTarget.transform.position - transform.position);
            if (Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, wallMask) || !Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, leaderMask))
            {
                Gizmos.color = Color.red;
                Vector3 direction = nodePath[currentNode].transform.position - transform.position;
                transform.forward = direction;
                transform.position += transform.forward * speed * Time.fixedDeltaTime;
            
                if (direction.magnitude <= 0.1f)
                {
                    currentNode++;
                    if (currentNode >= nodePath.Count)
                    {
                        currentNode = 0;
                        isFollowing = true;
                        isPathfinding = false;
                    }
                }
            }
            else
            {
                isFollowing = true;
                isPathfinding = false;
            }
        }

        if (isAvoiding)
        {
            Vector3 avoidSteer = ObstacleAvoidance();
            Vector3 steer = avoidSteer == Vector3.zero ? transform.position + transform.forward * 3 : avoidSteer;
            steer.y = 0;
            ApplyForce(Seek(steer));
            
            velocity.y = 0;
            transform.position += velocity * Time.deltaTime;
            transform.forward = velocity.normalized;
        }
    }
    
    Vector3 ObstacleAvoidance()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, viewDistance, wallMask))
        {
            Vector3 dirToTarget = (hit.transform.position - transform.position);
            float angle = Vector3.SignedAngle(transform.position + transform.forward, transform.position + dirToTarget.normalized, transform.up);
            int dir = angle > 0 ? -1 : 1;

            return transform.position + (dirToTarget * -1) * dir;
        }

        return Vector3.zero;
    }
    
    void Arrive()
    {
        if (seekTarget != null)
        {
            Vector3 desired = seekTarget.transform.position - transform.position;
            if (desired.magnitude < arriveDistance)
            {
                float speed = maxSpeed * (desired.magnitude / arriveDistance);
                desired.Normalize();
                desired *= speed;
            }
            else
            {
                desired.Normalize();
                desired *= maxSpeed;
            }

            Vector3 steering = desired - velocity;
            steering = Vector3.ClampMagnitude(steering, maxForce);

            ApplyForce(steering);
        }
    }
    Vector3 Cohesion()
    {
        Vector3 desired = new Vector3();
        int nearbyBoids = 0;

        foreach (var follower in SoldierManager.instance.allFollowers)
        {
            if (follower != this && Vector3.Distance(follower.transform.position, transform.position) < viewDistance)
            {
                desired += follower.transform.position;
                nearbyBoids++;
            }
        }
        if (nearbyBoids == 0) return desired;
        desired /= nearbyBoids;
        desired = desired - transform.position;
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        return steering;
    }

    Vector3 Align()
    {
        Vector3 desired = new Vector3();
        int nearbyBoids = 0;
        foreach (var follower in SoldierManager.instance.allFollowers)
        {
            if (follower != this && Vector3.Distance(follower.transform.position, transform.position) < viewDistance)
            {
                desired += follower.velocity;
                nearbyBoids++;
            }
        }
        if (nearbyBoids == 0) 
            return Vector3.zero;
        
        desired = desired / nearbyBoids;
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steering = Vector3.ClampMagnitude(desired - velocity, maxForce);

        return steering;
    }

    Vector3 Separation()
    {
        Vector3 desired = new Vector3();
        int nearbyBoids = 0;

        foreach (var follower in SoldierManager.instance.allFollowers)
        {
            Vector3 dist = follower.transform.position - transform.position;

            if (follower != this && dist.magnitude < separationDistance)
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

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        return steering;
    }
    
    Vector3 Seek(Vector3 target)
    {
        Vector3 desired = (target - transform.position).normalized * maxSpeed;
        Vector3 steering = Vector3.ClampMagnitude(desired - velocity, maxForce);
        return steering;
    }
    
    void ApplyForce(Vector3 force)
    {
        velocity += force;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, seekTarget.transform.position);
    }
}
