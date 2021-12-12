using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Node[,] nodeGrid;
    public int width;
    public int height;
    public float scale;

    public GameObject nodePrefab;

    public List<Node> GetNeighborsBasedOnPosition(Vector2Int pos)
    {
        List<Node> neighbors = new List<Node>();

        if (InBounds(pos.x, pos.y + 1)) neighbors.Add(nodeGrid[pos.x, pos.y + 1]);
        if (InBounds(pos.x + 1, pos.y)) neighbors.Add(nodeGrid[pos.x + 1, pos.y]);
        if (InBounds(pos.x, pos.y - 1)) neighbors.Add(nodeGrid[pos.x, pos.y - 1]);
        if (InBounds(pos.x - 1, pos.y)) neighbors.Add(nodeGrid[pos.x - 1, pos.y]);


        return neighbors;
    }

    bool InBounds(int x, int y)
    {
        if (x < width && x >= 0 && y >= 0 && y < height) return true;
        else return false;
    }

    public void PaintGridWhite()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!nodeGrid[x, y].blocked) GameManager.instance.ChangeGameObjectColor(nodeGrid[x, y].gameObject, Color.white);
            }

        }
    }

}
