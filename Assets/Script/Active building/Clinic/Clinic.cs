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
    private bool abilitiesApplied = false;

    private void Start()
    {
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        patienManger = GameManager.Instance != null ? GameManager.Instance.patienManger : FindFirstObjectByType<PatienManger>();
        uImanger = FindFirstObjectByType<UImanger>();

        // Always get components directly on this GameObject
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

        if (patienManger != null && patienManger.activeHealingClinicPatient != null)
        {
            patienManger.UpdateJobs(patienManger.activeHealingClinicPatient);
        }
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.ClinicUI);
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    uImanger.DisableUIPanel(UImanger.UIPanel.ClinicUpgradeUI);
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

        if (globalstat != null)
        {
            globalstat.IncreaseCurebed(CurrentActiveCurebed);
            globalstat.DecreaseDiscontent(currentDiscontentContribution);
        }

        int level = upgradeBuilding != null ? upgradeBuilding.currentLevel : 1;
        Debug.Log($"Applied abilities for Level {level}");
    }

    private void UpgradeAbilities()
    {
        int newCurebedContribution = GetActiveCurebedbaseonvalue();
        float newDiscontentContribution = GetDiscontentValueBasedOnLevel();

        if (globalstat != null)
        {
            globalstat.UpdateCurebedContribution(CurrentActiveCurebed, newCurebedContribution);
            globalstat.UpdateDiscontentContribution(currentDiscontentContribution, newDiscontentContribution);
        }

        CurrentActiveCurebed = newCurebedContribution;
        currentDiscontentContribution = newDiscontentContribution;

        int level = upgradeBuilding != null ? upgradeBuilding.currentLevel : 1;
        Debug.Log($"Upgraded to Level {level}");
    }

    private int GetActiveCurebedbaseonvalue()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2: return 2;
                case 3: return 4;
                default: return 1;
            }
        }
        return 1;
    }

    private float GetDiscontentValueBasedOnLevel()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2: return 10f;
                case 3: return 8f;
                default: return 5f;
            }
        }
        return 5f;
    }
}
