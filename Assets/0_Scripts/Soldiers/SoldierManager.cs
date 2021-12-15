using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoldierManager : MonoBehaviour
{
    public static SoldierManager instance;
    public List<Pathfinder> allFollowers = new List<Pathfinder>();

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

    public void AddFollower(Pathfinder follower)
    {
        if (!allFollowers.Contains(follower))
            allFollowers.Add(follower);
    }
}
