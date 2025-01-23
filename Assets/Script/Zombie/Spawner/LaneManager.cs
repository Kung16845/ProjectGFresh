using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance { get; private set; }

    [HideInInspector]
    public List<Lane> allLanes = new List<Lane>();

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

    // Method to register lanes
    public void RegisterLanes(List<Lane> lanes)
    {
        allLanes = lanes;
    }

    // Optionally, you can add methods to add or remove lanes individually
}
