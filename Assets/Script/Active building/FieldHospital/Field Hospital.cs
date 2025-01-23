using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldHospital : MonoBehaviour
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
    public float CurrentHealingSpeed = 0;
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
        patienManger.UpdateJobs(patienManger.activeHealingHospitalPatient);
    }
    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.FieldHospitalUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.FieldHospitalUpgradeUI);
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
        CurrentHealingSpeed = GetHealingSpeedbaseonvalue();

        // Apply contributions
        globalstat.IncreaseCurebed(CurrentActiveCurebed);
        globalstat.DecreaseDiscontent(currentDiscontentContribution);
        globalstat.IncreaseHealingSpeed(CurrentHealingSpeed);

        Debug.Log($"Applied abilities for Level {upgradeBuilding.currentLevel}");
    }

    void UpgradeAbilities()
    {
        // Calculate and replace old contributions with new ones
        int NewCurebedcontribution = GetActiveCurebedbaseonvalue();
        float newDiscontentContribution = GetDiscontentValueBasedOnLevel();
        float NewhealingspeedContribution = GetHealingSpeedbaseonvalue();

        globalstat.UpdateCurebedContribution(CurrentActiveCurebed, NewCurebedcontribution);
        globalstat.UpdateDiscontentContribution(currentDiscontentContribution, newDiscontentContribution);
        globalstat.UpdateHealingSpeedContribution(CurrentHealingSpeed, NewhealingspeedContribution);

        CurrentActiveCurebed = NewCurebedcontribution;
        currentDiscontentContribution = newDiscontentContribution;
        CurrentHealingSpeed = NewhealingspeedContribution;

        Debug.Log($"Upgraded to Level {upgradeBuilding.currentLevel}");
    }

    int GetActiveCurebedbaseonvalue()
    {
        switch (upgradeBuilding.currentLevel)
        {
            case 2: return 7; // Level 2
            default: return 3; // Level 1
        }
    }
    int GetHealingSpeedbaseonvalue()
    {
        switch (upgradeBuilding.currentLevel)
        {
            case 2: return 10; // Level 2
            default: return 0; // Level 1
        }
    }

    float GetDiscontentValueBasedOnLevel()
    {
        switch (upgradeBuilding.currentLevel)
        {
            case 2: return 25f; // Level 2
            default: return 20f; // Level 1
        }
    }
}
