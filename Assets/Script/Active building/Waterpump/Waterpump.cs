using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterpump : MonoBehaviour
{
    public BuildManager buildManager;
    public Building building;
    public TimeManager timeManager;
    public UpgradeBuilding upgradeBuilding;
    public DateTime dateTime;
    public UpgradeUi upgradeUi;
    public UImanger uImanger;
    public int currentday;
    public int FuelCost;

    private void Start()
    {
        uImanger = FindFirstObjectByType<UImanger>();
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();

        // Always get components on this GameObject to prevent cross-talk
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentday = dateTime.day;
        }
    }

    private void Update()
    {
        if (dateTime == null && timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        if (dateTime != null && currentday != dateTime.day)
        {
            if (upgradeBuilding != null && buildManager != null)
            {
                if (upgradeBuilding.currentLevel == 1)
                {
                    if (buildManager.fuel >= FuelCost)
                    {
                        buildManager.fuel -= FuelCost;
                        Activewater();
                    }
                    else
                    {
                        DeactiveWater();
                    }
                }
                else if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    Activewater();
                }
            }

            currentday = dateTime.day;  
        }
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.WaterPumpUI);
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    FuelCost = 0;
                    uImanger.DisableUIPanel(UImanger.UIPanel.WaterPumpUpgradeUi);
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
    }

    private void Activewater()
    {
        if (building != null && building.isfinsih && buildManager != null)
        {
            buildManager.iswateractive = true;
        }
    }

    private void DeactiveWater()
    {
        if (buildManager != null)
        {
            buildManager.iswateractive = false;
        }
    }
}
