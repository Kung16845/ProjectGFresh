using System.Collections.Generic;
using UnityEngine;

public enum CraftingResult
{
    Success,
    NotEnoughItems,
    NoAvailableSlots
}

public class Workshop : BaseBuilding
{
    private UImanger uImanger;
    private UpgradeUi upgradeUi;
    private CraftManager craftManager;
    private bool isElectricityApplied;

    public List<CraftingItem> craftingItemsLevel1;
    public List<CraftingItem> craftingItemsLevel2;
    public InventoryItemPresent inventoryItemPresent;

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
        craftManager = FindObjectOfType<CraftManager>();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        isElectricityApplied = false;
    }

    protected override BuildingContribution GetContributionForLevel(int level)
    {
        switch (level)
        {
            case 2:
                return new BuildingContribution { craftingSlots = 5 };
            default:
                return new BuildingContribution { craftingSlots = 3 };
        }
    }

    protected override void Update()
    {
        base.Update();

        if (craftManager != null)
            craftManager.UpdateCraftingJobs();

        // Electricity toggle — not level-based, handled separately
        if (building.isfinsih)
            UpdateElectricityBonus();
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.WorkshopUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.WorkshopUpgradeUI);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }

    private void UpdateElectricityBonus()
    {
        bool hasElectricity = BuildManager.Instance.iselecticitiesactive;

        if (hasElectricity && !isElectricityApplied)
        {
            globalstat.CalculateActionSpeed(0.25f);
            isElectricityApplied = true;
        }
        else if (!hasElectricity && isElectricityApplied)
        {
            globalstat.CalculateActionSpeed(-0.25f);
            isElectricityApplied = false;
        }
    }

    public CraftingResult AddCraftingJob(CraftingItem craftingItem)
    {
        return craftManager.AddCraftingJob(craftingItem, CraftingSource.Workshop);
    }
}
