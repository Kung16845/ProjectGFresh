using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public int currentDay;
    public Building building;
    public UpgradeBuilding upgradeBuilding;
    public Globalstat globalstat;
    public UImanger uImanger;
    public UpgradeUi upgradeUi;
    
    private float currentRiskContribution = 0;
    private int currentOutpostlimit = 0;
    private float currentRewardSpeed = 0;
    private float currentNpcchange = 0;
    private bool isApplied = false;  
    private bool UpgradeApply = false;

    private void Start()
    {
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        uImanger = FindFirstObjectByType<UImanger>();

        // Always get components directly from this GameObject
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day;
        }

        isApplied = false;
    }
    
    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.BeaconUI);
                
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel && !UpgradeApply)
                {
                    uImanger.DisableUIPanel(UImanger.UIPanel.BeaconUpgradeUI);
                    UpgradeBeaconContribution();
                    UpgradeApply = true;
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

    private void Update()
    {
        if (!isApplied && building != null && building.isfinsih)
        {
            ApplyRiskContribution();
        }
    }

    private void ApplyRiskContribution()
    {
        currentRiskContribution = GetRiskValueBasedOnLevel();
        currentOutpostlimit = GetOutpostValueBasedOnLevel();
        currentRewardSpeed = GetRewardValueBasedOnLevel();
        currentNpcchange = GetNpcchangeValueBasedOnLevel();

        if (globalstat != null)
        {
            globalstat.DecreaseRiskofExipidition(currentRiskContribution);
            globalstat.IncreaseOutpostlimit(currentOutpostlimit);
            globalstat.IncreaseRewardspeed(currentRewardSpeed);
            globalstat.IncreaseNpcchange(currentNpcchange);
        }

        isApplied = true;
    }

    private void UpgradeBeaconContribution()
    {
        float newRiskContribution = GetRiskValueBasedOnLevel();
        int newOutpostContribution = GetOutpostValueBasedOnLevel();
        float newRewardContribution = GetRewardValueBasedOnLevel();
        float newNpcChangeValue = GetNpcchangeValueBasedOnLevel();

        if (globalstat != null)
        {
            globalstat.UpdateoOutpostContribution(currentOutpostlimit, newOutpostContribution);
            globalstat.UpdateRiskofExipiditionContribution(currentRiskContribution, newRiskContribution);
            globalstat.UpdateRewardSpeedContribution(currentRewardSpeed, newRewardContribution);
            globalstat.UpdateNpcchangeContribution(currentNpcchange, newNpcChangeValue);
        }

        currentOutpostlimit = newOutpostContribution;
        currentRiskContribution = newRiskContribution;
        currentRewardSpeed = newRewardContribution;
        currentNpcchange = newNpcChangeValue;
    }

    public float GetRiskValueBasedOnLevel()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2:
                    return 15f;
                default:
                    return 10f;
            }
        }
        return 10f;
    }

    public float GetRewardValueBasedOnLevel()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2:
                    return 25f;
                default:
                    return 0f;
            }
        }
        return 0f;
    }

    public float GetNpcchangeValueBasedOnLevel()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2:
                    return 7f;
                default:
                    return 5f;
            }
        }
        return 5f;
    }

    public int GetOutpostValueBasedOnLevel()
    {
        if (upgradeBuilding != null)
        {
            switch (upgradeBuilding.currentLevel)
            {
                case 2: return 3;
                default: return 2;
            }
        }
        return 2;
    }
}
