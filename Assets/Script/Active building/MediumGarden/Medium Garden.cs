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

    public bool isHerbalPlanted = false;
    public int herbalItemID = 1;
    public int herbalDailyAmount = 5;

    private int _previousLevel = 1;

    private void Start()
    {
        inventoryItemPresent = FindFirstObjectByType<InventoryItemPresent>();
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        uImanger = FindFirstObjectByType<UImanger>();

        // Always get components directly from this GameObject
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day;
        }

        if (upgradeBuilding != null)
        {
            _previousLevel = upgradeBuilding.currentLevel;
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

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.MediumGardenUI);
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    uImanger.DisableUIPanel(UImanger.UIPanel.MediumGardenUpgradeButton);
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

    private void FoodGain()
    {
        if (dateTime == null && timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        if (dateTime != null && dateTime.day != currentDay && building != null && building.isfinsih)
        {
            int finalDailyYield = foodGainPerDay;

            if (yieldduration > 0)
            {
                finalDailyYield += 3;
                yieldduration--;
            }

            if (isHerbalPlanted && upgradeBuilding != null && upgradeBuilding.currentLevel >= 2)
            {
                if (inventoryItemPresent != null)
                {
                    int herbalCount = yieldduration > 0 ? 9 : 5;
                    inventoryItemPresent.AddItemByID(1020105, herbalCount);
                }
            }
            else
            {
                if (buildManager != null)
                {
                    buildManager.food += finalDailyYield;
                }
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
                    foodGainPerDay = 3;
                    break;
                case 2:
                    foodGainPerDay = 5;
                    break;
                default:
                    foodGainPerDay = 3;
                    break;
            }
        }
        else
        {
            foodGainPerDay = 3;
        }
    }

    public void UpdateCurrentYield()
    {
        int baseYield = foodGainPerDay;
        if (yieldduration > 0)
        {
            baseYield += 3;
        }

        if (isHerbalPlanted && upgradeBuilding != null && upgradeBuilding.currentLevel >= 2)
        {
            currentYield = herbalDailyAmount;
        }
        else
        {
            currentYield = baseYield;
        }
    }

    public void IncreaseYield()
    {
    }
}
