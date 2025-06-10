using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostSystem : MonoBehaviour
{
    public TimeManager timeManager;
    public DateTime dateTime;
    public Globalstat globalstat;
    public InventoryItemPresent inventoryItemPresent;
    public BuildManager buildManager;

    public List<OutpostReward> outpostRewards = new List<OutpostReward>();
    public List<OutpostData> outpostData = new List<OutpostData>(); // New list for outpost icons and locations
    public int currentDay;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        buildManager = FindObjectOfType<BuildManager>();
        dateTime = timeManager.dateTime;
        currentDay = dateTime.day; // Initialize currentDay

    }

    void Update()
    {
        if (dateTime.day != currentDay) // Check if a new day has started
        {
            GiveOutpostReward();
            currentDay = dateTime.day; // Update to the current day
        }
    }

    public void AddOutpostReward(Location location, int amount)
    {
        // Look for the corresponding icon for the outpost location
        OutpostData outpostInfo = outpostData.Find(outpost => outpost.location == location);
        
        if (outpostInfo != null)
        {
            outpostRewards.Add(new OutpostReward
            {
                location = location,
                amountGive = amount,
                itemName = location.ToString(),
                locationIcon = outpostInfo.locationIcon // Assign the icon from outpostData
            });
        }
        else
        {
            Debug.LogWarning($"Outpost data for location {location} not found.");
        }
    }
    public void RemoveOutpostReward(Location location)
    {
        // Find the outpost reward that matches the given location
        OutpostReward rewardToRemove = outpostRewards.Find(reward => reward.location == location);
        
        if (rewardToRemove != null)
        {
            outpostRewards.Remove(rewardToRemove); // Remove the outpost reward from the list
            Debug.Log($"Outpost reward for {location} has been removed.");
        }
        else
        {
            Debug.LogWarning($"No outpost reward found for location {location}.");
        }
    }

    void GiveOutpostReward()
    {
        foreach (var reward in outpostRewards)
        {
            SupplyType supplyType = GetSupplyTypeFromLocation(reward.location);
            if (supplyType != SupplyType.None) // Assuming SupplyType.None is a valid type for no supply
            {
                buildManager.AddSupply(supplyType, reward.amountGive);
                Debug.Log($"Given {reward.amountGive} {supplyType} from {reward.location}");
            }
        }
    }

    SupplyType GetSupplyTypeFromLocation(Location location)
    {
        switch (location)
        {
            case Location.Mall:
                return SupplyType.Food;
            case Location.Military:
                return SupplyType.Ammo;
            case Location.GasStation:
                return SupplyType.Fuel;
            case Location.WoodFactory:
                return SupplyType.Plank;
            case Location.SteelFactory:
                return SupplyType.Steel;
            default:
                Debug.LogWarning($"Unknown location: {location}");
                return SupplyType.None;
        }
    }
}

[System.Serializable]
public class OutpostReward
{
    public Location location;
    public int amountGive;
    public string itemName;
    public Sprite locationIcon; // Icon for the outpost
}

[System.Serializable]
public class OutpostData
{
    public Location location;
    public Sprite locationIcon; // Icon for each location
}

public enum Location
{
    Mall,
    Military,
    GasStation,
    WoodFactory,
    SteelFactory
}
