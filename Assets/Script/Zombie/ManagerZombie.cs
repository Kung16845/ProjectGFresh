using System.Collections.Generic;
using UnityEngine;

public class ManagerZombie : MonoBehaviour
{
    public static ManagerZombie Instance { get; private set; }

    private List<Zombie> activeZombies = new List<Zombie>();

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // Optionally, don't destroy on load
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to register a zombie
    public void RegisterZombie(Zombie zombie)
    {
        if (!activeZombies.Contains(zombie))
        {
            activeZombies.Add(zombie);
        }
    }

    // Method to unregister a zombie
    public void UnregisterZombie(Zombie zombie)
    {
        if (activeZombies.Contains(zombie))
        {
            activeZombies.Remove(zombie);
        }
    }

    // Get list of all active zombies
    public List<Zombie> GetAllZombies()
    {
        return new List<Zombie>(activeZombies);
    }

    // Get zombies by state
    public List<Zombie> GetZombiesByState(ZombieState state)
    {
        return activeZombies.FindAll(zombie => zombie.CurrentState == state);
    }

    // Get zombies in a specific area (optional)
    public List<Zombie> GetZombiesInArea(Rect area)
    {
        return activeZombies.FindAll(zombie => area.Contains(zombie.transform.position));
    }
}
