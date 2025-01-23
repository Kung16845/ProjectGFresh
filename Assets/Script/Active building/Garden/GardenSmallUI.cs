using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GardenSmallUI : MonoBehaviour
{
    private GardenBuilding gardenBuilding;
    public TextMeshProUGUI Foodgain;
    public TextMeshProUGUI Isyield;
    public GameObject YieldButton;
    public InventoryItemPresent inventoryItemPresent;

    void Start()
    {
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
    }

    void OnEnable()
    {
        gardenBuilding = FindObjectOfType<GardenBuilding>();
        UpdateYieldButtonAndText();
        UpdateFoodGainText(); // Update the food gain text on panel activation
    }

    public void AssignUpgradeDataUI()
    {
        gardenBuilding.AssignUpgradeData();
    }

    void UpdateYieldButtonAndText()
    {
        // Check if there are enough seeds (i.e., count >= 2) in the inventory to increase yield
        int seedCount = GetSeedCount();

        if (gardenBuilding.yieldduration > 0)
        {
            YieldButton.SetActive(false);
            Isyield.text = "Yield is increased! Remaining time: " + gardenBuilding.yieldduration + " days.";
        }
        else if(seedCount >= 2 && gardenBuilding.yieldduration == 0)
        {
            YieldButton.SetActive(true);
            Isyield.text = "Yield has not increased.";
        }
        else
        {
            YieldButton.SetActive(false);
            Isyield.text = "Yield has not increased. Require 2 Seeds.";
        } 

    }

    // Function to get the number of seeds available in the inventory
    int GetSeedCount()
    {
        int seedCount = 0;
        foreach (ItemData item in inventoryItemPresent.listItemsDataBox)
        {
            if (item.idItem == 1020119) // Assuming 1020119 is the ID for seeds
            {
                seedCount = item.count;
                break;
            }
        }
        return seedCount;
    }

    public void IncreaseYield()
    {
        int seedCount = GetSeedCount();

        if (seedCount >= 2)
        {
            gardenBuilding.yieldduration = 3; // Set yield duration to 3 days
            // Optionally, reduce seed count after use:
            UseSeeds(2); // Assuming 2 seeds are used for increasing the yield
            UpdateYieldButtonAndText(); // Re-check the seed count and update the UI
            UpdateFoodGainText(); // Update food gain text when yield is increased
        }
    }

    // Use seeds after increasing yield
    void UseSeeds(int amount)
    {
        foreach (ItemData item in inventoryItemPresent.listItemsDataBox)
        {
            if (item.idItem == 1020119 && item.count >= amount)
            {
                item.count -= amount;
                break;
            }
        }
    }

    // Update the Isyield text each frame based on the duration
    void Update()
    {
        UpdateYieldButtonAndText(); // Update the UI to show remaining duration
        UpdateFoodGainText(); // Update food gain text dynamically
    }

    // Update the Foodgain text based on the current food gain and yield bonus
    void UpdateFoodGainText()
    {
        Foodgain.text = "Food gain per day: " + gardenBuilding.currentYield;
    }
}
