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

    private int _previousLevel = 1;

    private void Start()
    {
        uImanger = FindFirstObjectByType<UImanger>();
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();

        // Always get components directly from this GameObject
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day - 1;
        }

        if (upgradeBuilding != null)
        {
            _previousLevel = upgradeBuilding.currentLevel;
        }

        UpdateFoodGainPerDay();
        UpdateCurrentYield();
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.SmallGardenUI);
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    uImanger.DisableUIPanel(UImanger.UIPanel.SmallGardenUpgradeButton);
                }
            }
        }
    }

    public void AssignUpgradeData()
    {
        if (upgradeUi == null)
        {
            upgradeUi = FindFirstObjectByType<UpgradeUi>();
        }
        if (upgradeUi != null && upgradeBuilding != null)
        {
            upgradeUi.Initialize(upgradeBuilding);
        }
        UpdateFoodGainPerDay();
        UpdateCurrentYield();
    }

    private void Update()
    {
        if (upgradeBuilding != null && upgradeBuilding.currentLevel != _previousLevel)
        {
            _previousLevel = upgradeBuilding.currentLevel;
            UpdateFoodGainPerDay();
            UpdateCurrentYield();
        }

        FoodGain();
    }

    private void FoodGain()
    {
        if (dateTime == null && timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        if (dateTime != null && dateTime.day != currentDay && building != null && building.isfinsih)
        {
            int dailyFood = foodGainPerDay;

            if (yieldduration > 0)
            {
                dailyFood += 3;
                yieldduration--;
            }

            if (buildManager != null)
            {
                buildManager.food += dailyFood;
            }

            currentDay = dateTime.day;
            UpdateCurrentYield();
        }
    }

    public void UpdateFoodGainPerDay()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 1:
                    foodGainPerDay = 2;
                    break;
                case 2:
                    foodGainPerDay = 4;
                    break;
                default:
                    foodGainPerDay = 2;
                    break;
            }
        }
        else
        {
            foodGainPerDay = 2;
        }
    }

    public void UpdateCurrentYield()
    {
        currentYield = yieldduration > 0 ? foodGainPerDay + 3 : foodGainPerDay;
    }
}
