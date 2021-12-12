using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float speed;
    public bool isFollowing, isReturning, isPatroling, isChasing;

    [Header("Nodes")]
    private Node startNode;
    private Node goalNode;

    [Header("Patrol")]
    public List<Node> patrollingNodes = new List<Node>();
    public int currentNode;
    private int patrollingDireciton;
    private List<Node> originalPatrollingNodes;

    [Header("Field of View")]
    public float viewRadius;
    public float viewAngle;
    public LayerMask detectableAgentMask;
    public LayerMask obstacleMask;
    public LayerMask nodeMask;
    public GameObject target;

    private void Start()
    {
        isPatroling = true;
        isReturning = false;
        isFollowing = false;
        isChasing = false;
        originalPatrollingNodes = patrollingNodes;
    }

    private void Update()
    {
        FieldOfView();
        Patrol();
    }

    void Patrol()
    {
        Vector3 direction;
        
        if(!isChasing)
            direction = patrollingNodes[currentNode].transform.position - transform.position;
        else direction = target.transform.position - transform.position;
        
        transform.forward = direction;
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        
        if (direction.magnitude <= 0.1f)
        {
            currentNode++;

            if (currentNode == patrollingNodes.Count)
            {
                    
                if (isReturning)
                {
                    Debug.Log("Patrullo");
                    //patrollingNodes = originalPatrollingNodes;
                    CheckLineOfSight();
                    if (patrollingNodes == originalPatrollingNodes)
                    {
                        isPatroling = true;
                        isReturning = false;
                    }
                }
                    
                if (isFollowing)
                {
                    if (InSight(transform.position, target.transform.position))
                    {
                        isChasing = true;
                    }
                    else
                    {
                        Debug.Log("Vuelvo");
                        patrollingNodes.Reverse();
                        isReturning = true;
                        isFollowing = false; 
                    }
                }
                currentNode = 0;
            }
        }
    }
    
    public List<Node> ConstructPathAStar(Node startingNode, Node goalNode)
    {
        if (startingNode == null && goalNode == null) 
            return default;

        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        cameFrom.Add(startingNode, null);
        costSoFar.Add(startingNode, 0);

        while (frontier.Count() > 0)
        {
            Node current = frontier.Get();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();
                Node nodeToAdd = current;
                while (nodeToAdd != null)
                {
                    path.Add(nodeToAdd);
                    nodeToAdd = cameFrom[nodeToAdd];
                }
                path.Reverse();
                return path;
            }

            foreach (var next in current.GetNeighbors())
            {
                int newCost = costSoFar[current] + next.cost;
                float priority = newCost + Heuristic(next.transform.position, goalNode.transform.position);
                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Put(next, priority);
                    costSoFar.Add(next, newCost);
                    cameFrom.Add(next, current);
                }
                else if (costSoFar.ContainsKey(next) && newCost < costSoFar[next])
                {
                    frontier.Put(next, priority);
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                }
            }
        }
        return default;
    }

    public List<Node> ConstructPathThetaStar(Node startingNode, Node goalNode)
    {
        var path = ConstructPathAStar(startingNode, goalNode);
        path.Reverse();

        int index = 0;

        while (index <= path.Count - 1)
        {
            int indexNextNext = index + 2;
            if (indexNextNext > path.Count - 1) break;
            if (InSight(path[index].transform.position, path[indexNextNext].transform.position))
                path.Remove(path[index + 1]);
            else index++;

        }

        return path;
    }
    
    float Heuristic(Vector3 a, Vector3 b)
    {
        if (InSight(a, b))
        {
            return Vector3.Distance(a, b);
        }
        return -1f;
    }
    
    void FieldOfView()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, detectableAgentMask);

        foreach (var item in targetsInViewRadius)
        {
            Vector3 dirToTarget = (item.transform.position - transform.position);
            
            if (Vector3.Angle(transform.forward, dirToTarget.normalized) < viewAngle / 2)
            {
                if(!Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, obstacleMask) && dirToTarget.magnitude < viewRadius)
                {
                    currentNode = 0;
                    int id = NpcManager.instance.GetClosestNode(this);
                    isChasing = true;
                    NpcManager.instance.NotifyNPCs(NpcManager.instance.nodes[id], "Follow");
                    Debug.DrawLine(transform.position, item.transform.position, Color.red);
                    Debug.Log("Detecto player");
                }
                else
                {
                    if(isChasing)
                        CheckLineOfSight();
                    isChasing = false;
                    Debug.DrawLine(transform.position, item.transform.position, Color.green);
                }
            }
        }
        
    }

    void CheckLineOfSight()
    {
        bool flag = false;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, nodeMask);

        foreach (var item in targetsInViewRadius)
        {
            Vector3 dirToTarget = (item.transform.position - transform.position);
            
            if (Vector3.Angle(transform.forward, dirToTarget.normalized) < viewAngle)
            {
                if(!Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, obstacleMask) && dirToTarget.magnitude < viewRadius /2)
                {
                    for(int i = 0; i < originalPatrollingNodes.Count; i++)
                    {
                        if (item.GetComponent<Node>() != null && item.GetComponent<Node>() == originalPatrollingNodes[i])
                        {
                            patrollingNodes = originalPatrollingNodes;
                            currentNode = i;
                            flag = true;
                        }
                    }
                }
            }
        }

        if (!flag)
        {
            patrollingNodes =
                ConstructPathAStar(NpcManager.instance.nodes[NpcManager.instance.GetClosestNode(this)],
                    originalPatrollingNodes[0]);
            currentNode = 0;
            isFollowing = false;
            isReturning = true;

            if (patrollingNodes.Count == 1)
            {
                patrollingNodes = originalPatrollingNodes;
            }
        }
    }

    public bool InSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        if (!Physics.Raycast(start, dir, dir.magnitude, obstacleMask)) return true;
        else return false;
    }
    
    private void OnDrawGizmos()
    {
        Vector3 lineA = DirFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 lineB = DirFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + lineA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + lineB * viewRadius);
    }
    
    Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
