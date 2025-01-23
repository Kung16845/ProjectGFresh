using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SmallBedUI : MonoBehaviour
{
    private SmallBed smallBed;
    public TextMeshProUGUI BedCount;
    public TextMeshProUGUI UpgradeBenefits;
    void OnEnable()
    {
        smallBed = FindObjectOfType<SmallBed>();
        UpdateUI();
    }

    public void InitializeUpgradeData()
    {
        smallBed.AssignUpgradeData();
    }
    void UpdateUI()
    {
        if (smallBed != null)
        {
            // Update fuel cost display
            int bed = smallBed.currentBedContribution;
            BedCount.text = $"Bed Count: {bed} ";

            // Update upgrade benefit or fully upgraded status
            if (smallBed.upgradeBuilding.currentLevel < smallBed.upgradeBuilding.maxLevel)
            {
                int nextLevel = smallBed.upgradeBuilding.currentLevel + 1;
               UpgradeBenefits.text = $"Upgrade Benefit:Provide 4 Bed.";
            }
            else
            {
               UpgradeBenefits.text = "Fully Upgraded";
            }
        }
        else
        {
            BedCount.text = "smallBed not found";
           UpgradeBenefits.text = string.Empty;
        }
    }
}
