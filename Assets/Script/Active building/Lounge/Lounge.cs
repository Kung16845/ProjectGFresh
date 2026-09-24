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

    private bool abilitiesApplied = false;

    private void Start()
    {
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
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
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.LoungeUI);
                
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
                {
                    uImanger.DisableUIPanel(UImanger.UIPanel.LoungeUpgradeButton);
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
        currentBedContribution = GetBedValueBasedOnLevel();
        currentDiscontentContribution = GetDiscontentValueBasedOnLevel();

        if (globalstat != null)
        {
            globalstat.AddBedsFromBuilding(currentBedContribution);
            globalstat.DecreaseDiscontent(currentDiscontentContribution);
        }

        int level = upgradeBuilding != null ? upgradeBuilding.currentLevel : 1;
        Debug.Log($"Applied abilities for Level {level}");
    }

    private void UpgradeAbilities()
    {
        int newBedContribution = GetBedValueBasedOnLevel();
        float newDiscontentContribution = GetDiscontentValueBasedOnLevel();

        if (globalstat != null)
        {
            globalstat.UpdateBuildingBedContribution(currentBedContribution, newBedContribution);
            globalstat.UpdateDiscontentContribution(currentDiscontentContribution, newDiscontentContribution);
        }

        currentBedContribution = newBedContribution;
        currentDiscontentContribution = newDiscontentContribution;

        int level = upgradeBuilding != null ? upgradeBuilding.currentLevel : 1;
        Debug.Log($"Upgraded to Level {level}");
    }

    public int GetBedValueBasedOnLevel(int level = -1)
    {
        int targetLevel = (level == -1 && upgradeBuilding != null) ? upgradeBuilding.currentLevel : (level == -1 ? 1 : level);
        switch (targetLevel)
        {
            case 2: return 3;
            case 3: return 5;
            default: return 1;
        }
    }

    public float GetDiscontentValueBasedOnLevel(int level = -1)
    {
        int targetLevel = (level == -1 && upgradeBuilding != null) ? upgradeBuilding.currentLevel : (level == -1 ? 1 : level);
        switch (targetLevel)
        {
            case 2: return 25f;
            case 3: return 35f;
            default: return 15f;
        }
    }
}
