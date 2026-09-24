using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBed : MonoBehaviour
{
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public int currentDay;
    public UpgradeBuilding upgradeBuilding;
    public Globalstat globalstat;
    public Building building;
    public UImanger uImanger;
    public UpgradeUi upgradeUi;
    
    public int currentBedContribution = 0;
    private bool isApplied = false;
    private int previousLevel = 1;

    private void Start()
    {
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        uImanger = FindFirstObjectByType<UImanger>();

        // Always get components directly from this GameObject to prevent cross-talk
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day;
        }

        if (upgradeBuilding != null)
        {
            previousLevel = upgradeBuilding.currentLevel;
        }

        isApplied = false;
    }

    private void Update()
    {
        if (building != null && building.isfinsih)
        {
            if (!isApplied)
            {
                ApplyBedContribution();
                if (upgradeBuilding != null) previousLevel = upgradeBuilding.currentLevel;
            }
            else if (upgradeBuilding != null && upgradeBuilding.currentLevel != previousLevel)
            {
                UpgradeBedContribution();
                previousLevel = upgradeBuilding.currentLevel;
            }
        }
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.SmallBedUI);
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    uImanger.DisableUIPanel(UImanger.UIPanel.SmallBedUpgradeButton);
                }
            }
        }
    }

    private void ApplyBedContribution()
    {
        currentBedContribution = GetBedValueBasedOnLevel();
        if (globalstat != null)
        {
            globalstat.AddBedsFromBuilding(currentBedContribution);
        }
        isApplied = true;
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

    private void UpgradeBedContribution()
    {
        int newBedContribution = GetBedValueBasedOnLevel();

        if (globalstat != null)
        {
            globalstat.UpdateBuildingBedContribution(currentBedContribution, newBedContribution);
        }

        currentBedContribution = newBedContribution;
    }

    public int GetBedValueBasedOnLevel()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2:
                    return 4; // Level 2 contribution
                default:
                    return 2; // Level 1 contribution
            }
        }
        return 2;
    }
}
