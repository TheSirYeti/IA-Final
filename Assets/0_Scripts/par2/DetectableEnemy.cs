using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectableEnemy : MonoBehaviour
{
    [SerializeField]
    private Material _detectedMaterial;
    private Material _startingMaterial;
    private Renderer _rend;
    private void Start()
    {
        _rend = GetComponent<Renderer>();
        _startingMaterial = _rend.material;
    }

    public void Detect(bool d)
    {
        _rend.material = d ? _detectedMaterial : _startingMaterial;
    }
}
