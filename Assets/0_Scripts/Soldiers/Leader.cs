using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : Pathfinder
{
    public Transform goalPosition;
    public LayerMask floorMask;
    private RaycastHit hit;
    void Update() {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            goalPosition.position = hit.point;
            int closeNode = NodeManager.instance.GetClosestNode(transform);
            int endNode = NodeManager.instance.GetClosestNode(goalPosition);
            ConstructPathThetaStar(NodeManager.instance.nodes[closeNode], NodeManager.instance.nodes[endNode]);
        }
    }
}
