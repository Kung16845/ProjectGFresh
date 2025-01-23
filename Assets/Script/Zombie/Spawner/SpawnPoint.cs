using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Lane lane;

    public Coroutine spawnCoroutine;
    private MainSpawner mainSpawner;

    public void Initialize(Lane lane)
    {
        this.lane = lane;
        mainSpawner = FindObjectOfType<MainSpawner>();
        if (mainSpawner == null)
        {
            Debug.LogError("MainSpawner not found in the scene!");
        }
    }

    // Starts spawning based on the queue
    public void StartSpawningQueue(List<ZombieSpawnQueue> spawnQueue, List<MutationType> selectedMutations, float mutationApplyRate)
    {
        if (spawnCoroutine == null)
        {
            Debug.Log($"Starting Spawn Zombie Queue: {spawnQueue.Count} zombies, MutationTypes: {string.Join(", ", selectedMutations)}, ApplyRate: {mutationApplyRate}");
            spawnCoroutine = StartCoroutine(SpawnZombieQueueCoroutine(spawnQueue, selectedMutations, mutationApplyRate));
        }
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnZombieQueueCoroutine(List<ZombieSpawnQueue> spawnQueue, List<MutationType> selectedMutations, float mutationApplyRate)
    {
        foreach (ZombieSpawnQueue spawnConfig in spawnQueue)
        {
            // Notify MainSpawner about zombies to spawn
            mainSpawner?.OnZombieQueueStarted(spawnConfig.quantity, spawnConfig.spawnInterval);

            int spawnedZombies = 0;
            while (spawnedZombies < spawnConfig.quantity)
            {
                Debug.Log(spawnedZombies);
                Debug.Log(spawnConfig.quantity);
                SpawnZombie(spawnConfig, selectedMutations, mutationApplyRate);
                spawnedZombies++;
                yield return new WaitForSeconds(spawnConfig.spawnInterval);
            }

            // Notify MainSpawner about zombies spawned
            mainSpawner?.OnZombieSpawned(spawnedZombies, spawnConfig.spawnInterval);
        }

        spawnCoroutine = null; // Reset coroutine so it can be started again if needed
    }

    private void SpawnZombie(ZombieSpawnQueue spawnConfig, List<MutationType> selectedMutations, float mutationApplyRate)
    {
        if (spawnConfig.zombiePrefab != null)
        {
            GameObject zombieObject = Instantiate(spawnConfig.zombiePrefab, lane.spawnPoint.position, Quaternion.identity);
            zombieObject.transform.SetParent(lane.spawnPoint.transform);

            Zombie zombie = zombieObject.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.SetLane(lane);
                zombie.SetTier(spawnConfig.zombieTier);

                // Determine if mutations should be applied
                if (Random.value <= mutationApplyRate)
                {
                    // Randomly select a mutation from selectedMutations
                    if (selectedMutations.Count > 0)
                    {
                        MutationType randomMutation = selectedMutations[Random.Range(0, selectedMutations.Count)];
                        zombie.SetMutationType(randomMutation);
                    }
                }
                else
                {
                    zombie.SetMutationType(MutationType.None);
                }

                // Optionally, you can notify MainSpawner directly here if needed
            }
        }
        else
        {
            Debug.LogWarning("SpawnPoint: zombiePrefab is null in spawnConfig.");
        }
    }

}
[System.Serializable]
public struct MutationSelectionRule
{
    public float minSkillPoint;                  // Minimum skill point to apply this rule
    public int mutationsToSelect;                // Number of mutations to select (1 or 2)
    public List<MutationType> availableMutations; // List of mutations available for selection
}