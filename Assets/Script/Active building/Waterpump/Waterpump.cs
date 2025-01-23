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
    void Start()
    {
        uImanger = FindObjectOfType<UImanger>();
        timeManager = FindObjectOfType<TimeManager>();
        buildManager = FindObjectOfType<BuildManager>();
        upgradeBuilding = FindObjectOfType<UpgradeBuilding>();
        building = GetComponent<Building>();
        dateTime = timeManager.dateTime;
        currentday = dateTime.day;
    }
    void Update()
    {
        if(currentday != dateTime.day)
        {
            if(upgradeBuilding.currentLevel == 1)
            {
                if(buildManager.fuel >= FuelCost)
                {
                    buildManager.fuel -= FuelCost;
                    Activewater();
                }
                else if(buildManager.fuel < FuelCost)
                {
                    DeactiveWater();
                }
            }
            else if(upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                Activewater();
            }
            currentday = dateTime.day;  
        }
    }
    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.WaterPumpUI);
            
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                FuelCost = 0;
                uImanger.DisableUIPanel(UImanger.UIPanel.WaterPumpUpgradeUi);
            }
        }
    }
    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }
    void Activewater()
    {
        if(building.isfinsih)
        {
            buildManager.iswateractive = true;
        }
    }
     void DeactiveWater()
    {
        buildManager.iswateractive = false;
    }
}
