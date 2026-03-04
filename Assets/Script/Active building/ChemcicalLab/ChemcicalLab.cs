using System.Collections.Generic;
using UnityEngine;

public class ChemcicalLab : BaseBuilding
{
    private UImanger uImanger;
    private UpgradeUi upgradeUi;
    private CraftManager craftManager;
    public InventoryItemPresent inventoryItemPresent;

    public List<CraftingItem> craftingItemsLevel1;
    public List<CraftingItem> craftingItemsLevel2;

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
        craftManager = FindObjectOfType<CraftManager>();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
    }

    protected override BuildingContribution GetContributionForLevel(int level)
    {
        switch (level)
        {
            case 2:
                return new BuildingContribution { chemicalSlots = 5 };
            default:
                return new BuildingContribution { chemicalSlots = 3 };
        }
    }

    protected override void Update()
    {
        base.Update();
        if (craftManager != null)
            craftManager.UpdateCraftingJobs();
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.ChemcicalLabWorkshopUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.ChemcicalLabButtonUpgradeUI);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }

    public CraftingResult AddCraftingJob(CraftingItem craftingItem)
    {
        return craftManager.AddCraftingJob(craftingItem, CraftingSource.ChemicalLab);
    }
}
