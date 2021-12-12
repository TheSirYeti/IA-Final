using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Node _startingNode;
    private Node _goalNode;
    public float debugTime;
    private Pathfinding _pf;
    public Grid grid;
    
    List<Node> waypointsToFollow = new List<Node>();
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _pf = new Pathfinding();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            grid?.PaintGridWhite();
            ChangeGameObjectColor(_goalNode.gameObject, Color.green);
            if (_startingNode != null && _goalNode != null) StartCoroutine(_pf.PaintAStar(_startingNode, _goalNode, debugTime));
        }

    }

    public void SetStartingNode(Node n)
    {
        if (_startingNode != null) ChangeGameObjectColor(_startingNode.gameObject, Color.white);
        _startingNode = n;
        ChangeGameObjectColor(_startingNode.gameObject, Color.red);
    }
    public void SetGoalNode(Node n)
    {
        if (_goalNode != null) ChangeGameObjectColor(_goalNode.gameObject, Color.white);
        _goalNode = n;
        ChangeGameObjectColor(_goalNode.gameObject, Color.green);
    }

    public void ChangeGameObjectColor(GameObject go, Color color)
    {
        if (go == null) return;

        go.GetComponent<Renderer>().material.color = color;
    }
}
