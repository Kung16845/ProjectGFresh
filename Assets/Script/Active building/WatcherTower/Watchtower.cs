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
    
    private float currentRiskContribution = 0; // Track the bed contribution for this building
    private float currentNpcchange = 0;
    private bool isApplied = false;         // Ensure we apply once per stage

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        globalstat = FindObjectOfType<Globalstat>();
        buildManager = FindObjectOfType<BuildManager>();
        building = FindObjectOfType<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        dateTime = timeManager.dateTime;
        currentDay = dateTime.day;
        isApplied = false;
    }

    void Update()
    {
        if (!isApplied && building.isfinsih)
        {
            ApplyRiskContribution(); // Apply initial bed contribution
        }
    }

    void ApplyRiskContribution()
    {
        currentRiskContribution = GetRiskValueBasedOnLevel();
        currentNpcchange = GetNpcchangeValueBasedOnLevel();
        globalstat.DecreaseRiskofExipidition(currentRiskContribution);
        globalstat.IncreaseNpcchange(currentNpcchange);
        isApplied = true; // Ensure this runs only once after the building finishes
    }

    // void UpgradeBeaconContribution()
    // {
    //     float NewriskContribution = GetRiskValueBasedOnLevel();
    //     float NewNpcchangevalue = GetNpcchangeValueBasedOnLevel();
    //     // Replace the old contribution with the new one
    //     globalstat.UpdateRiskofExipiditionContribution(currentRiskContribution, NewriskContribution);
    //     globalstat.UpdateNpcchangeContribution(currentNpcchange, NewNpcchangevalue);
    //     currentRiskContribution = NewriskContribution; // Store the new contribution
    //     currentNpcchange = NewNpcchangevalue;
    // }

    float GetRiskValueBasedOnLevel()
    {
        return 40; // Default to 0 if no upgrade building is found
    }

    float GetNpcchangeValueBasedOnLevel()
    {
        return 7; // Default to 0 if no upgrade building is found
    }
}
