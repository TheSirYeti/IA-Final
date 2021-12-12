using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class Node : MonoBehaviour
{
    public float viewRadius;
    public List<Node> _neighbors = new List<Node>();

    public int cost = 1;
    public bool blocked;

    private void Start()
    {
        foreach (Node node in NodeManager.instance.nodes)
        {
            if (Vector3.Distance(node.transform.position, this.transform.position) <= viewRadius 
                && !Physics.Raycast(transform.position, node.transform.position, viewRadius, NodeManager.instance.wallMask))
            {
                
                _neighbors.Add(node);
            }
        }
    }
    public List<Node> GetNeighbors()
    {
        return _neighbors;
    }

    void ChangeCost(int c)
    {
        if (c < 1) c = 1;
        cost = c;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
