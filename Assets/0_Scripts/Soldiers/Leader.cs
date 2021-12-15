using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : Pathfinder
{
    public Transform goalPosition;
    public LayerMask floorMask;
    private RaycastHit hit;
    public LayerMask obstacleMask;
    public LayerMask wallMask;
    public float maxForce;
    public float viewDistance;

    public bool isAvoiding;
    public bool isLineOfSight;
    private void Start()
    {
        SoldierManager.instance.AddFollower(this);
    }

    void Update()
    {
        if (Physics.OverlapSphere(transform.position, viewDistance / 3, obstacleMask).Length > 0)
        {
            isAvoiding = true;
        }
        else
        {
            isAvoiding = false;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            goalPosition.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            Vector3 dir = goalPosition.transform.position - transform.position;
            if (Physics.Raycast(transform.position, dir, dir.magnitude, wallMask))
            {
                if (nodePath == null)
                {
                    nodePath = new List<Node>();
                }
            
            
                int closeNode = NodeManager.instance.GetClosestNode(transform);
                int endNode = NodeManager.instance.GetClosestNode(goalPosition);
                if (closeNode != -1 && endNode != -1)
                {
                    nodePath = ConstructPathThetaStar(NodeManager.instance.nodes[closeNode], NodeManager.instance.nodes[endNode]);
                }
                isPathfinding = true;
                
                Debug.Log("PATH");
            }
            else
            {
                isLineOfSight = true;
                Debug.Log("VOY DIRECTO");
            }
        }

        if (isPathfinding && !isAvoiding)
        {
            FollowPath();
        }

        if (isLineOfSight)
        {
            Vector3 dir = goalPosition.transform.position - transform.position;
            transform.forward = dir;
            transform.position += transform.forward * speed * Time.fixedDeltaTime;
            Debug.Log("Me muevo");
            if (dir.magnitude <= 0.1f)
            {
                isLineOfSight = false;
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
        Debug.DrawLine(transform.position, transform.position + transform.forward * viewDistance, Color.red);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, viewDistance, obstacleMask))
        {
            Vector3 dirToTarget = (hit.transform.position - transform.position);
            Vector3 newDir = new Vector3(dirToTarget.x, 0, dirToTarget.z);
            float angle = Vector3.SignedAngle(transform.position + transform.forward, transform.position + newDir.normalized, transform.up);
            int dir = angle > 0 ? -1 : 1;

            return transform.position + transform.right * dir;
        }

        return Vector3.zero;
    }

    Vector3 Seek(Vector3 target)
    {
        Vector3 desired = (target - transform.position).normalized * speed;
        Vector3 steering = Vector3.ClampMagnitude(desired - velocity, maxForce);
        return steering;
    }

    void ApplyForce(Vector3 force)
    {
        velocity.y = 0;
        velocity = Vector3.ClampMagnitude(velocity + force, speed);
    }
}
