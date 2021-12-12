using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPosManager : MonoBehaviour
{

    public LayerMask floorMask;
    public LayerMask wallMask;
    private RaycastHit hit;
    public Transform leader;

    void Update() {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            leader.position = hit.point + Vector3.up;
        }
    }
}
