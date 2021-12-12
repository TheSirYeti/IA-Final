using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public List<Node> ConstructPathAStar(Node startingNode, Node goalNode)
    {
        if (startingNode == null && goalNode == null) return default;

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

    public IEnumerator PaintAStar(Node startingNode, Node goalNode, float time)
    {
        if (startingNode == null && goalNode == null) yield break;

        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        cameFrom.Add(startingNode, null);
        costSoFar.Add(startingNode, 0);

        while (frontier.Count() > 0)
        {
            Node current = frontier.Get();
            GameManager.instance.ChangeGameObjectColor(current.gameObject, Color.blue);
            yield return new WaitForSeconds(time);
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
                foreach (var item in path)
                {
                    GameManager.instance.ChangeGameObjectColor(item.gameObject, Color.cyan);
                }
                yield break;

            }

            foreach (var next in current.GetNeighbors())
            {
                int newCost = costSoFar[current] + next.cost;
                float priority = newCost + Heuristic(next.transform.position, goalNode.transform.position);
                if (!costSoFar.ContainsKey(next) && !next.blocked)
                {
                    frontier.Put(next, priority);
                    costSoFar.Add(next, newCost);
                    cameFrom.Add(next, current);
                    GameManager.instance.ChangeGameObjectColor(next.gameObject, Color.gray);
                }
                else if (costSoFar.ContainsKey(next) && newCost < costSoFar[next] && !next.blocked) 
                {
                    frontier.Put(next, priority);
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                    GameManager.instance.ChangeGameObjectColor(next.gameObject, Color.red);
                }
            }
            GameManager.instance.ChangeGameObjectColor(current.gameObject, Color.gray);
        }
    }


}
