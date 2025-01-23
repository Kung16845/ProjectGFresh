using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumGarden : MonoBehaviour
{
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public Building building;
    public UImanger uImanger;
    public UpgradeUi upgradeUi;
    public UpgradeBuilding upgradeBuilding;
    public InventoryItemPresent inventoryItemPresent;
    public int foodGainPerDay;
    public int currentDay;
    public int yieldduration;
    public int currentYield;

    // New variables for herbal planting option
    public bool isHerbalPlanted = false; // Whether the player chose herbal instead of normal
    public int herbalItemID = 1; // The ID for the herbal item, replace with the correct ID
    public int herbalDailyAmount = 5; // Amount of herbal items produced per day when herbal is chosen

    void Start()
    {
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        timeManager = FindObjectOfType<TimeManager>();
        buildManager = FindObjectOfType<BuildManager>();
        building = FindObjectOfType<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();
        uImanger = FindObjectOfType<UImanger>();

        dateTime = timeManager.dateTime;
        currentDay = dateTime.day;
    }

    void Update()
    {
        UpdateFoodGainPerDay();
        FoodGain();
        UpdateCurrentYield();
    }

    // This function is called when player clicks on the building
    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            // Toggle UI panel for MediumGarden (similar to SmallGardenUI)
            // Assuming you have a UIPanel enum and a corresponding panel for MediumGarden:
            uImanger.ToggleUIPanel(UImanger.UIPanel.MediumGardenUI);

            // If max level reached, disable upgrade button
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.MediumGardenUpgradeButton);
            }

            // Here you could also present an option to choose herbal or normal
            // This choice can be made via UI buttons and set the isHerbalPlanted flag accordingly.
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
        UpdateFoodGainPerDay(); // ensure correct gain after upgrading
    }

    // Called once per frame, checks if day changed and gives yield
    void FoodGain()
    {
        // Check if the day has changed
        if (dateTime.day != currentDay && building.isfinsih)
        {
            int finalDailyYield = foodGainPerDay;

            // If yield duration bonus is active, add 3 to daily yield and decrease duration
            if (yieldduration > 0)
            {
                finalDailyYield += 3;
                yieldduration--;
            }

            // If herbal is chosen and level is at least 2, give herbal items instead of normal food
            if (isHerbalPlanted && upgradeBuilding.currentLevel >= 2)
            {
                if(yieldduration > 0)
                    inventoryItemPresent.AddItemByID(1020105, 9);
                else    
                    inventoryItemPresent.AddItemByID(1020105,5);
            }
            else
            {
                // Normal yield (food)
                buildManager.food += finalDailyYield;
            }

            // Update current day
            currentDay = dateTime.day;

            Debug.Log("Current Day: " + currentDay);
            Debug.Log("Yield Duration: " + yieldduration);
        }
    }

    // Update the food gain per day based on the current building upgrade level
    void UpdateFoodGainPerDay()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 1:
                    foodGainPerDay = 3; // Level 1 food gain
                    break;
                case 2:
                    foodGainPerDay = 5; // Level 2 food gain
                    break;
                default:
                    foodGainPerDay = 3; // Default to Level 1 if no match
                    break;
            }
        }
    }

    // Update the current yield shown to the player
    void UpdateCurrentYield()
    {
        // If yield bonus is active, add 3 to the base yield
        int baseYield = foodGainPerDay;
        if (yieldduration > 0)
        {
            baseYield += 3;
        }

        // If herbal is chosen and level is at least 2, the current yield should reflect herbal output (5 items)
        if (isHerbalPlanted && upgradeBuilding.currentLevel >= 2)
        {
            currentYield = herbalDailyAmount; // Always 5 if herbal is chosen at lvl 2
        }
        else
        {
            currentYield = baseYield; // normal yield
        }

        Debug.Log("Current Yield: " + currentYield);
        Debug.Log("Yield Duration: " + yieldduration);
    }

    public void IncreaseYield()
    {
        // Implement or leave as is, depending on the original mechanics
        // This function can be triggered by UI or other events
    }

}
