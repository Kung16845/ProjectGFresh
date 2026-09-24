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
    private bool abilitiesApplied = false;

    private void Start()
    {
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        patienManger = GameManager.Instance != null ? GameManager.Instance.patienManger : FindFirstObjectByType<PatienManger>();
        uImanger = FindFirstObjectByType<UImanger>();

        // Always get components on this GameObject
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        if (upgradeBuilding != null)
        {
            previousLevel = upgradeBuilding.currentLevel;
        }
    }

    private void Update()
    {
        if (building != null && building.isfinsih)
        {
            if (!abilitiesApplied)
            {
                ApplyAbilities();
                abilitiesApplied = true;
                if (upgradeBuilding != null) previousLevel = upgradeBuilding.currentLevel;
            }
            else if (upgradeBuilding != null && upgradeBuilding.currentLevel != previousLevel)
            {
                UpgradeAbilities();
                previousLevel = upgradeBuilding.currentLevel;
            }
        }

        if (patienManger != null && patienManger.activeHealingHospitalPatient != null)
        {
            patienManger.UpdateJobs(patienManger.activeHealingHospitalPatient);
        }
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.FieldHospitalUI);
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    uImanger.DisableUIPanel(UImanger.UIPanel.FieldHospitalUpgradeUI);
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

    private void ApplyAbilities()
    {
        CurrentActiveCurebed = GetActiveCurebedbaseonvalue();
        currentDiscontentContribution = GetDiscontentValueBasedOnLevel();
        CurrentHealingSpeed = GetHealingSpeedbaseonvalue();

        if (globalstat != null)
        {
            globalstat.IncreaseCurebed(CurrentActiveCurebed);
            globalstat.DecreaseDiscontent(currentDiscontentContribution);
            globalstat.IncreaseHealingSpeed(CurrentHealingSpeed);
        }

        int level = upgradeBuilding != null ? upgradeBuilding.currentLevel : 1;
        Debug.Log($"Applied abilities for Level {level}");
    }

    private void UpgradeAbilities()
    {
        int newCurebedContribution = GetActiveCurebedbaseonvalue();
        float newDiscontentContribution = GetDiscontentValueBasedOnLevel();
        float newHealingSpeedContribution = GetHealingSpeedbaseonvalue();

        if (globalstat != null)
        {
            globalstat.UpdateCurebedContribution(CurrentActiveCurebed, newCurebedContribution);
            globalstat.UpdateDiscontentContribution(currentDiscontentContribution, newDiscontentContribution);
            globalstat.UpdateHealingSpeedContribution(CurrentHealingSpeed, newHealingSpeedContribution);
        }

        CurrentActiveCurebed = newCurebedContribution;
        currentDiscontentContribution = newDiscontentContribution;
        CurrentHealingSpeed = newHealingSpeedContribution;

        int level = upgradeBuilding != null ? upgradeBuilding.currentLevel : 1;
        Debug.Log($"Upgraded to Level {level}");
    }

    private int GetActiveCurebedbaseonvalue()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2: return 7;
                default: return 3;
            }
        }
        return 3;
    }

    private int GetHealingSpeedbaseonvalue()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2: return 10;
                default: return 0;
            }
        }
        return 0;
    }

    private float GetDiscontentValueBasedOnLevel()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2: return 25f;
                default: return 20f;
            }
        }
        return 20f;
    }
}
