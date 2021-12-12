using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<NPC> npcs;
    public List<Node> nodes;

    public static EnemyManager instance;

    private void Awake()
    {
        instance = this;
    }

    public int GetClosestNode(Transform t)
    {
        int id = -1;
        float minDistance = Mathf.Infinity;
        for(int i = 0; i < nodes.Count; i++)
        {
            float dist = Vector3.Distance(t.position, nodes[i].transform.position);
            if (dist < minDistance)
            {
                id = i;
                minDistance = dist;
            }
        }

        return id;
    }
}
