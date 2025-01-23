using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solar : MonoBehaviour
{
    public BuildManager buildManager;
    public Building building;
    public TimeManager timeManager;
    public UpgradeBuilding upgradeBuilding;
    public DateTime dateTime;
    public UImanger uImanger;
    public UpgradeUi upgradeUi;
    public int currentday;
    public int steelCost;
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
                if(buildManager.steel >= steelCost)
                {
                    buildManager.steel -= steelCost;
                    ActiveElecticities();
                }
                else if(buildManager.steel < steelCost)
                {
                    DeactiveElecticities();
                }
            }
            else if(upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                ActiveElecticities();
            }
            currentday = dateTime.day;  
        }
    }
    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.SolarUI);
            
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                steelCost = 0;
                uImanger.DisableUIPanel(UImanger.UIPanel.SolarUpgradeUI);
            }
        }
    }
    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }
    void ActiveElecticities()
    {
        if(building.isfinsih)
        {
            buildManager.iselecticitiesactive = true;
        }
    }
    void DeactiveElecticities()
    {
        buildManager.iselecticitiesactive = false;
    }
}
