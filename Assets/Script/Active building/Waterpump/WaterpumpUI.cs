using System.Collections;
using UnityEngine;
using TMPro;

public class WaterpumpUI : MonoBehaviour
{
    private Waterpump waterpump;
    public TextMeshProUGUI WaterpumpStat;
    public TextMeshProUGUI UpgradeBenefit;

    void Start()
    {
        waterpump = FindObjectOfType<Waterpump>();
        UpdateUI();
    }

    void OnEnable()
    {
        waterpump = FindObjectOfType<Waterpump>();
        UpdateUI();
    }

    public void InitializeUpgradeData()
    {
        waterpump.AssignUpgradeData();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (waterpump != null)
        {
            // Update fuel cost display
            int fuelCost = waterpump.FuelCost;
            WaterpumpStat.text = $"Fuel Cost: {fuelCost} per day";

            // Update upgrade benefit or fully upgraded status
            if (waterpump.upgradeBuilding.currentLevel < waterpump.upgradeBuilding.maxLevel)
            {
                int nextLevel = waterpump.upgradeBuilding.currentLevel + 1;
                UpgradeBenefit.text = $"Upgrade Benefit:No longer Consume fuel every day.";
            }
            else
            {
                UpgradeBenefit.text = "Fully Upgraded";
            }
        }
        else
        {
            WaterpumpStat.text = "Waterpump not found";
            UpgradeBenefit.text = string.Empty;
        }
    }
}
