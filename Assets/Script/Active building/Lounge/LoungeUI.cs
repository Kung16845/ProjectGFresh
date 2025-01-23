using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoungeUI : MonoBehaviour
{
    private Lounge lounge;
    public TextMeshProUGUI CurrentBuff;
    public TextMeshProUGUI CurrentBed;
    public TextMeshProUGUI UpgradeBenefits;
    public Button Booststatduringnight;
    public Globalstat globalstst;
    public InventoryItemPresent inventoryItemPresent;
    public TimeManager timeManager;

    void Start()
    {
        lounge = FindObjectOfType<Lounge>();   
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
    }

    void OnEnable()
    {
        timeManager = FindObjectOfType<TimeManager>();
        lounge = FindObjectOfType<Lounge>();
        UpdateUI();
    }

    public void AssignUpgradeDataUI()
    {
        lounge.AssignUpgradeData();
    }
    public void ActiveEntertainmentBonus()
    {
        globalstst.ActionSpeed += 20;
        globalstst.Craftingspeed += 20;
        globalstst.expiditionspeed += 10;
    }
    void Update()
    {
    }
     void UpdateUI()
    {
        if (lounge != null)
        {
            // Update current bed and discontent contributions
            CurrentBed.text = $"Bed Count: {lounge.currentBedContribution}";
            CurrentBuff.text = $"Morale Increase: {lounge.currentDiscontentContribution}";

            // Update upgrade benefit or fully upgraded status
            if (lounge.upgradeBuilding.currentLevel < lounge.upgradeBuilding.maxLevel)
            {
                int nextLevel = lounge.upgradeBuilding.currentLevel + 1;
                int nextBedContribution = lounge.GetBedValueBasedOnLevel(nextLevel);
                float nextDiscontentContribution = lounge.GetDiscontentValueBasedOnLevel(nextLevel);

                UpgradeBenefits.text = $"Next Upgrade: {nextBedContribution} Beds, {nextDiscontentContribution} Morale Increase";
            }
            else
            {
                UpgradeBenefits.text = "Fully Upgraded";
            }
        }
        else
        {
            CurrentBed.text = "Lounge not found";
            CurrentBuff.text = string.Empty;
            UpgradeBenefits.text = string.Empty;
        }
    }
}
