using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class FoodBehaviour : MonoBehaviour
{
    public List<GameObject> foodPositions;
    public float timeToWait;

    private void Start()
    {
        StartCoroutine(SpawnRandomFood());
        EventManager.Subscribe("DisablePreviousFood", DisablePreviousFood);
    }

    IEnumerator SpawnRandomFood()
    {
        while (true)
        {
            DisablePreviousFood(null);
            int rand = UnityEngine.Random.Range(0, foodPositions.Count);
            foodPositions[rand].SetActive(true);
            foreach (BoidAgent boid in BoidManager.instance.allBoids)
            {
                boid.seekTarget = foodPositions[rand];
            }

            yield return new WaitForSeconds(10);
        }
    }

    void DisablePreviousFood(object[] parameters)
    {
        foreach (GameObject g in foodPositions)
        {
            g.SetActive(false);
        }

        foreach (BoidAgent boid in BoidManager.instance.allBoids)
        {
            boid.seekTarget = null;
        }
    }
}
