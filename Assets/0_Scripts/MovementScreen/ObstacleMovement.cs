using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public List<GameObject> waypoints;
    public float speed;
    private int currentWaypoint = 0;
    private void Update()
    {
        Vector3 direction = waypoints[currentWaypoint].transform.position - transform.position;
        transform.forward = direction;

        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        
        
        if (direction.magnitude <= 0.1f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Count)
            {
                currentWaypoint = 0;
            }
        }
        
    }
}
