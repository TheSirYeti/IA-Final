using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{

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
    bool InSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        if (!Physics.Raycast(start, dir, dir.magnitude, NodeManager.instance.wallMask)) return true;
        else return false;
    }

    public List<Node> ConstructPathAStar(Node startingNode, Node goalNode)
    {
        if (startingNode == null || goalNode == null) return default;

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
                //path.Reverse();
                return path;
            }

            foreach (var next in current.GetNeighbors())
            {
                if (next.blocked) continue;
                int newCost = costSoFar[current] + next.cost;
                //Lo unico que cambia es la priority del frontier que le sumamos la heuristica
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
    
    float Heuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }
}
