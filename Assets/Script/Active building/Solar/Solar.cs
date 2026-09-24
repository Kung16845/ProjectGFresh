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
                    if (buildManager.steel >= steelCost)
                    {
                        buildManager.steel -= steelCost;
                        ActiveElecticities();
                    }
                    else
                    {
                        DeactiveElecticities();
                    }
                }
                else if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    ActiveElecticities();
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
                uImanger.ToggleUIPanel(UImanger.UIPanel.SolarUI);
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    steelCost = 0;
                    uImanger.DisableUIPanel(UImanger.UIPanel.SolarUpgradeUI);
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

    private void ActiveElecticities()
    {
        if (building != null && building.isfinsih && buildManager != null)
        {
            buildManager.iselecticitiesactive = true;
        }
    }

    private void DeactiveElecticities()
    {
        if (buildManager != null)
        {
            buildManager.iselecticitiesactive = false;
        }
    }
}
