using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenBuilding : MonoBehaviour
{
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public Building building;
    public UImanger uImanger;
    public int foodGainPerDay;
    public UpgradeUi upgradeUi;
    public int currentDay;
    public UpgradeBuilding upgradeBuilding;
    public int currentYield;
    public int yieldduration;

    void Start()
    {
        uImanger = FindObjectOfType<UImanger>();
        timeManager = FindObjectOfType<TimeManager>();
        buildManager = FindObjectOfType<BuildManager>();
        building = FindObjectOfType<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();
        dateTime = timeManager.dateTime;
        currentDay = dateTime.day - 1;
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.SmallGardenUI);
            
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.SmallGardenUpgradeButton);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
        UpdateFoodGainPerDay(); // Make sure foodGainPerDay is updated after upgrading
    }

    void Update()
    {
        UpdateFoodGainPerDay(); // Ensure foodGainPerDay is updated based on the building's level
        FoodGain();
        UpdateCurrentYield(); // Ensure currentYield is updated with each frame
    }

    void FoodGain()
    {
        // Check if the day has changed
        if (dateTime.day != currentDay && building.isfinsih)
        {
            if (yieldduration > 0)
            {
                foodGainPerDay += 3; // Add bonus during yield duration
                yieldduration--; // Decrease yield duration only when the day changes
            }

            buildManager.food += foodGainPerDay; // Add food gained to the total food count
            currentDay = dateTime.day; // Update the current day to reflect the new day

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
                    foodGainPerDay = 2; // Level 1 food gain
                    break;
                case 2:
                    foodGainPerDay = 4; // Level 2 food gain
                    break;
                default:
                    foodGainPerDay = 2; // Default to Level 1
                    break;
            }
        }
    }

    // Update the current yield based on whether there's a bonus applied during yield duration
    void UpdateCurrentYield()
    {
        // If the yield bonus is active, increase the current yield by 3
        if (yieldduration > 0)
        {
            currentYield = foodGainPerDay + 3; // Add bonus during yield duration
        }
        else
        {
            currentYield = foodGainPerDay; // No bonuses
        }
    }
}
