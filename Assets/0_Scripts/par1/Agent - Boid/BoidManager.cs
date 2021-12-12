using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public static BoidManager instance;
    public List<BoidAgent> allBoids = new List<BoidAgent>();

    private void Awake()
    {
        if (instance == null) 
            instance = this;
        else
        {
            Debug.Log("An instance of " + this + " was already present in the scene. Deleting...");
            Destroy(gameObject);
        }
    }

    public void AddBoid(BoidAgent boid)
    {
        if (!allBoids.Contains(boid))
            allBoids.Add(boid);
    }

}
