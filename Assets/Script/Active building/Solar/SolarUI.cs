using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SolarUI : MonoBehaviour
{
    private Solar solar;
    public TextMeshProUGUI solarStat;
    public TextMeshProUGUI UpgradeBenefit;

    void Start()
    {
        solar = FindObjectOfType<Solar>();
        UpdateUI();
    }

    void OnEnable()
    {
        solar = FindObjectOfType<Solar>();
        UpdateUI();
    }

    public void InitializeUpgradeData()
    {
        solar.AssignUpgradeData();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (solar != null)
        {
            // Update fuel cost display
            int steelCost = solar.steelCost;
            solarStat.text = $"Steel Cost: {steelCost} per day";

            // Update upgrade benefit or fully upgraded status
            if (solar.upgradeBuilding.currentLevel < solar.upgradeBuilding.maxLevel)
            {
                int nextLevel = solar.upgradeBuilding.currentLevel + 1;
                UpgradeBenefit.text = $"Upgrade Benefit:No longer Consume fuel every day.";
            }
            else
            {
                UpgradeBenefit.text = "Fully Upgraded";
            }
        }
        else
        {
            solarStat.text = "solar not found";
            UpgradeBenefit.text = string.Empty;
        }
    }
}
