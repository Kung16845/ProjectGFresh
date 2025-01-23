using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawned : MonoBehaviour
{

    public float duration;
    public float treatValue = 50;
    Coroutine spawnCoroutine;
    public List<GameObject> prefabMonster;
    public Transform transformBarrier;
    private void Start()
    {   
        FindClosestBarrier();
        StartCoroutine(SpawnMon());
    }
    public void FindClosestBarrier()
    {
        Barrier[] barriers = FindObjectsOfType<Barrier>();
        float closestDistance = Mathf.Infinity;
        Transform closestBarrierTransform = null;

        foreach (Barrier b in barriers)
        {
            float distanceToBarrier = Vector2.Distance(transform.position, b.transform.position);
            if (distanceToBarrier < closestDistance)
            {
                closestDistance = distanceToBarrier;
                closestBarrierTransform = b.transform;
            }
        }

        if (closestBarrierTransform != null)
        {
            transformBarrier = closestBarrierTransform;
            
        }
    }

    public void StopSpawnEnemy()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    public IEnumerator SpawnMon()
    {
        yield return new WaitForSeconds(duration);
        SpawnedMonster();
        spawnCoroutine = StartCoroutine(SpawnMon());
    }

    public void SpawnedMonster()
    {
        Vector2 toBarrier = transformBarrier.position - transform.position;
        Vector3 transformSpawnMonster = new Vector3();
        if (Mathf.Abs(toBarrier.x) > Mathf.Abs(toBarrier.y))
        {   
            transformSpawnMonster = new Vector3(this.transform.position.x, Random.Range(-4.0f, 4.0f));
        }
        else
        {
            transformSpawnMonster = new Vector3(Random.Range(-7.0f, 7.0f),this.transform.position.y);
        }
        
        // float[] probabilities = { 0.5f, 0.45f, 0.05f };
        // float randomValue = Random.value;
        // int n = 0;

        // // Determine which monster to spawn based on the random value.
        // if (randomValue < probabilities[0])
        // {
        //     n = 0; // First monster type (50% chance)
        // }
        // else if (randomValue < probabilities[0] + probabilities[1])
        // {
        //     n = 1; // Second monster type (40% chance)
        // }
        // else
        // {
        //     n = 2; // Third monster type (10% chance)
        // }   

        GameObject monster = Instantiate(prefabMonster[0], transformSpawnMonster, this.transform.rotation);

  

    }

}
