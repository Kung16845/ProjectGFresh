using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnWave
{
    public List<LaneSpawnConfig> laneSpawnConfigs; // List of lane configurations
    public float timeUntilNextWave; // Time after this wave ends before the next wave starts
}

[System.Serializable]
public class LaneSpawnConfig
{
    public int laneID;
    public List<ZombieSpawnQueue> zombieSpawnQueue; // Queue of zombies to spawn in this lane
}

[System.Serializable]
public class ZombieSpawnQueue
{
    public GameObject zombiePrefab;
    public int quantity;
    public float spawnInterval;
    public int zombieTier;
    public MutationType mutationType; 
    // You can add more parameters if needed
}
