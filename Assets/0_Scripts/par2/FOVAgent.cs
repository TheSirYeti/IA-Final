
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVAgent : MonoBehaviour
{
    [Header("Stats")]
    public float speed;

    [Header("Field of View")]
    public float viewRadius;
    public float viewAngle;
    public LayerMask detectableAgentMask;
    public LayerMask obstacleMask;


    List<DetectableEnemy> _detectedAgents = new List<DetectableEnemy>();


    private float _hMov;
    private float _vMov;
    private Vector3 _velocity;


    
    void Update()
    {
        Inputs();
        Move();
        FieldOfView();
    }

    void FieldOfView()
    {
        ClearDetectableEnemies();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, detectableAgentMask);

        foreach (var item in targetsInViewRadius)
        {
            Vector3 dirToTarget = (item.transform.position - transform.position);
            
            if (Vector3.Angle(transform.forward, dirToTarget.normalized) < viewAngle / 2)
            {
                if (InSight(transform.position, item.transform.position))
                {
                    item.GetComponent<DetectableEnemy>().Detect(true);
                    _detectedAgents.Add(item.GetComponent<DetectableEnemy>());
                    Debug.DrawLine(transform.position, item.transform.position, Color.red);
                }
                else
                {
                    Debug.DrawLine(transform.position, item.transform.position, Color.green);
                }
            }
        }
    }

    void ClearDetectableEnemies()
    {
        foreach (var item in _detectedAgents)
        {
            item.Detect(false);
        }
        _detectedAgents.Clear();
    }

    bool InSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        if (!Physics.Raycast(start, dir, dir.magnitude, obstacleMask)) return true;
        else return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 lineA = DirFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 lineB = DirFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + lineA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + lineB * viewRadius);

    }
    Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private void Move()
    {
        _velocity.Set(_hMov, 0, _vMov);
        _velocity.Normalize();
        if (_hMov != 0 || _vMov != 0) transform.forward = _velocity;

        transform.position += _velocity * speed * Time.deltaTime;


    }

    private void Inputs()
    {
        _hMov = Input.GetAxisRaw("Horizontal");
        _vMov = Input.GetAxisRaw("Vertical");
    }
}
