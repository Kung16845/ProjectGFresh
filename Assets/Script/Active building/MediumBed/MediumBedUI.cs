using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MediumBedUI : MonoBehaviour
{
    private MediumBed mediumBed;
    public TextMeshProUGUI BedCount;
    public TextMeshProUGUI UpgradeBenefits;
    void OnEnable()
    {
        mediumBed = FindObjectOfType<MediumBed>();
        UpdateUI();
    }

    public void InitializeUpgradeData()
    {
        mediumBed.AssignUpgradeData();
    }
    void UpdateUI()
    {
        if (mediumBed != null)
        {
            // Update fuel cost display
            int bed = mediumBed.currentBedContribution;
            BedCount.text = $"Bed Count: {bed} ";

            // Update upgrade benefit or fully upgraded status
            if (mediumBed.upgradeBuilding.currentLevel < mediumBed.upgradeBuilding.maxLevel)
            {
                int nextLevel = mediumBed.upgradeBuilding.currentLevel + 1;
               UpgradeBenefits.text = $"Upgrade Benefit:Provide 7 Bed.";
            }
            else
            {
               UpgradeBenefits.text = "Fully Upgraded";
            }
        }
        else
        {
            BedCount.text = "mediumBed not found";
           UpgradeBenefits.text = string.Empty;
        }
    }
}
