using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clinic : MonoBehaviour
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
    private PatienManger patienManger;
    public float Healingrate;

    private float currentDiscontentContribution = 0f;
    public int CurrentActiveCurebed = 0;
    private int previousLevel = 0;

    private bool abilitiesApplied = false; // Ensure abilities apply only once

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        globalstat = FindObjectOfType<Globalstat>();
        buildManager = FindObjectOfType<BuildManager>();
        building = FindObjectOfType<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();
        patienManger = FindObjectOfType<PatienManger>();
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
        patienManger.UpdateJobs(patienManger.activeHealingClinicPatient);
    }
    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.ClinicUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.ClinicUpgradeUI);
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
        CurrentActiveCurebed = GetActiveCurebedbaseonvalue();
        currentDiscontentContribution = GetDiscontentValueBasedOnLevel();

        // Apply contributions
        globalstat.IncreaseCurebed(CurrentActiveCurebed);
        globalstat.DecreaseDiscontent(currentDiscontentContribution);

        Debug.Log($"Applied abilities for Level {upgradeBuilding.currentLevel}");
    }

    void UpgradeAbilities()
    {
        // Calculate and replace old contributions with new ones
        int NewCurebedcontribution = GetActiveCurebedbaseonvalue();
        float newDiscontentContribution = GetDiscontentValueBasedOnLevel();

        globalstat.UpdateCurebedContribution(CurrentActiveCurebed, NewCurebedcontribution);
        globalstat.UpdateDiscontentContribution(currentDiscontentContribution, newDiscontentContribution);

        CurrentActiveCurebed = NewCurebedcontribution;
        currentDiscontentContribution = newDiscontentContribution;

        Debug.Log($"Upgraded to Level {upgradeBuilding.currentLevel}");
    }

    int GetActiveCurebedbaseonvalue()
    {
        switch (upgradeBuilding.currentLevel)
        {
            case 2: return 2; // Level 2
            case 3: return 4; // Level 3
            default: return 1; // Level 1
        }
    }

    float GetDiscontentValueBasedOnLevel()
    {
        switch (upgradeBuilding.currentLevel)
        {
            case 2: return 10f; // Level 2
            case 3: return 8f; // Level 3
            default: return 5f; // Level 1
        }
    }
}
