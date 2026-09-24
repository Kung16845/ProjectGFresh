using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watchtower : MonoBehaviour
{
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public int currentDay;
    public Building building;
    public UpgradeBuilding upgradeBuilding;
    public Globalstat globalstat;
    
    private float currentRiskContribution = 0;
    private float currentNpcchange = 0;
    private bool isApplied = false;

    private void Start()
    {
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();

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
        currentNpcchange = GetNpcchangeValueBasedOnLevel();

        if (globalstat != null)
        {
            globalstat.DecreaseRiskofExipidition(currentRiskContribution);
            globalstat.IncreaseNpcchange(currentNpcchange);
        }

        isApplied = true;
    }

    private float GetRiskValueBasedOnLevel()
    {
        return 40f;
    }

    private float GetNpcchangeValueBasedOnLevel()
    {
        return 7f;
    }
}
