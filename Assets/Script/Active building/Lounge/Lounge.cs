using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lounge : MonoBehaviour
{
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public Building building;
    public UpgradeBuilding upgradeBuilding;
    public Globalstat globalstat;
    public UImanger uImanger;
    public UpgradeUi upgradeUi;

    public float currentDiscontentContribution = 0f;
    public int currentBedContribution = 0;
    private int previousLevel = 0;

    private bool abilitiesApplied = false; // Ensure abilities apply only once

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        globalstat = FindObjectOfType<Globalstat>();
        buildManager = FindObjectOfType<BuildManager>();
        building = FindObjectOfType<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();
        uImanger = FindObjectOfType<UImanger>();
        dateTime = timeManager.dateTime;

        previousLevel = upgradeBuilding.currentLevel; // Sync level on start
    }

    void Update()
    {
        if (building.isfinsih && !abilitiesApplied)
        {
            ApplyAbilities(); // Apply abilities once when finished
            abilitiesApplied = true; // Mark as applied to avoid duplication
        }

        // Detect level changes during runtime
        if (building.isfinsih && upgradeBuilding.currentLevel != previousLevel)
        {
            UpgradeAbilities();
            previousLevel = upgradeBuilding.currentLevel;
        }
    }
     void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.LoungeUI);
            
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.LoungeUpgradeButton);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }
    void ApplyAbilities()
    {
        // Get the current contributions based on the level
        currentBedContribution = GetBedValueBasedOnLevel();
        currentDiscontentContribution = GetDiscontentValueBasedOnLevel();

        // Apply contributions
        globalstat.AddBedsFromBuilding(currentBedContribution);
        globalstat.DecreaseDiscontent(currentDiscontentContribution);

        Debug.Log($"Applied abilities for Level {upgradeBuilding.currentLevel}");
    }

    void UpgradeAbilities()
    {
        // Calculate and replace old contributions with new ones
        int newBedContribution = GetBedValueBasedOnLevel();
        float newDiscontentContribution = GetDiscontentValueBasedOnLevel();

        globalstat.UpdateBuildingBedContribution(currentBedContribution, newBedContribution);
        globalstat.UpdateDiscontentContribution(currentDiscontentContribution, newDiscontentContribution);

        currentBedContribution = newBedContribution;
        currentDiscontentContribution = newDiscontentContribution;

        Debug.Log($"Upgraded to Level {upgradeBuilding.currentLevel}");
    }

    public int GetBedValueBasedOnLevel(int level = -1)
    {
        int targetLevel = (level == -1) ? upgradeBuilding.currentLevel : level;
        switch (targetLevel)
        {
            case 2: return 3; // Level 2
            case 3: return 5; // Level 3
            default: return 1; // Level 1
        }
    }

    public float GetDiscontentValueBasedOnLevel(int level = -1)
    {
        int targetLevel = (level == -1) ? upgradeBuilding.currentLevel : level;
        switch (targetLevel)
        {
            case 2: return 25f; // Level 2
            case 3: return 35f; // Level 3
            default: return 15f; // Level 1
        }
    }

}





