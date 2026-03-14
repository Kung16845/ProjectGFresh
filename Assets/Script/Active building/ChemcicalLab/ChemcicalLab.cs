using System.Collections.Generic;
using UnityEngine;

public class ChemcicalLab : BaseBuilding
{
    private CraftManager craftManager;
    public InventoryItemPresent inventoryItemPresent;

    public List<CraftingItem> craftingItemsLevel1;
    public List<CraftingItem> craftingItemsLevel2;

    protected override void Start()
    {
        base.Start();
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

    public CraftingResult AddCraftingJob(CraftingItem craftingItem)
    {
        return craftManager.AddCraftingJob(craftingItem, CraftingSource.ChemicalLab);
    }
}
