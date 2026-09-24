using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public List<CraftingJob> activeCraftingJobs = new List<CraftingJob>();
    public List<CraftingJob> ChemicalactiveJobs = new List<CraftingJob>();
    public List<CraftingJob> MedicineactiveJobs = new List<CraftingJob>();
    public List<CraftingJob> MoonshienactiveJobs = new List<CraftingJob>();

    public InventoryItemPresent inventoryItemPresent;
    public BuildManager buildManager;
    public Globalstat globalstat;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            globalstat = GameManager.Instance.globalstat;
            inventoryItemPresent = GameManager.Instance.inventoryItemPresent;
            buildManager = GameManager.Instance.buildManager;
        }

        if (globalstat == null) globalstat = FindFirstObjectByType<Globalstat>();
        if (inventoryItemPresent == null) inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        if (buildManager == null) buildManager = BuildManager.Instance ?? FindFirstObjectByType<BuildManager>();

        UpdateUsedSlots();
    }

    public CraftingResult AddCraftingJob(CraftingItem craftingItem, CraftingSource source)
    {
        if (craftingItem == null || globalstat == null || inventoryItemPresent == null)
        {
            return CraftingResult.NoAvailableSlots;
        }

        switch (source)
        {
            case CraftingSource.Workshop:
                if (globalstat.CraftingSlot <= 0)
                {
                    Debug.LogWarning("Crafting is not allowed in Workshop: No active crafting slots.");
                    return CraftingResult.NoAvailableSlots;
                }
                break;

            case CraftingSource.ChemicalLab:
                if (globalstat.ChemicalCraftingSlot <= 0)
                {
                    Debug.LogWarning("Crafting is not allowed in Chemical Lab: No active crafting slots.");
                    return CraftingResult.NoAvailableSlots;
                }
                break;

            case CraftingSource.Moonshine:
                if (globalstat.MoonshineCraftingSlot <= 0)
                {
                    Debug.LogWarning("Crafting is not allowed in Moonshine: No active crafting slots.");
                    return CraftingResult.NoAvailableSlots;
                }
                break;
            
            default:
                Debug.LogWarning("Unknown crafting source.");
                return CraftingResult.NoAvailableSlots;
        }

        // Check if the player has enough items for the recipe
        if (craftingItem.recipeItems != null)
        {
            for (int i = 0; i < craftingItem.recipeItems.Count; i++)
            {
                RecipeItem recipeItem = craftingItem.recipeItems[i];
                if (recipeItem == null) continue;

                int amountHave = inventoryItemPresent.GetItemCountByID(recipeItem.itemID);
                if (amountHave < recipeItem.amountNeeded)
                {
                    Debug.LogWarning($"Not enough {recipeItem.itemName}. Required: {recipeItem.amountNeeded}, Have: {amountHave}");
                    return CraftingResult.NotEnoughItems;
                }
            }
        }

        // Check if BuildManager has enough resources
        if (buildManager != null)
        {
            if (craftingItem.Ammoneeded > buildManager.ammo ||
                craftingItem.Fuelneeded > buildManager.fuel ||
                craftingItem.Steelneeded > buildManager.steel ||
                craftingItem.Plankneeded > buildManager.plank ||
                craftingItem.Foodneeded > buildManager.food)
            {
                Debug.LogWarning("Not enough resources.");
                return CraftingResult.NotEnoughItems;
            }
        }

        DeductResources(craftingItem);

        float actionSpeed = globalstat.ActionSpeed > 0 ? globalstat.ActionSpeed : 1f;
        CraftingJob newJob = new CraftingJob(craftingItem, actionSpeed, source);

        switch (source)
        {
            case CraftingSource.Workshop:
                activeCraftingJobs.Add(newJob);
                globalstat.CraftingSlot -= 1;
                break;

            case CraftingSource.ChemicalLab:
                ChemicalactiveJobs.Add(newJob);
                globalstat.ChemicalCraftingSlot -= 1;
                break;
            
            case CraftingSource.Moonshine:
                MoonshienactiveJobs.Add(newJob);
                globalstat.MoonshineCraftingSlot -= 1;
                break;

            default:
                Debug.LogWarning("Unknown Crafting Source.");
                return CraftingResult.NoAvailableSlots;
        }

        UpdateUsedSlots();
        return CraftingResult.Success;
    }

    public void UpdateCraftingJobs()
    {
        UpdateJobs(activeCraftingJobs);
        UpdateJobs(ChemicalactiveJobs);
        UpdateJobs(MoonshienactiveJobs);
        UpdateUsedSlots();
    }

    private void UpdateJobs(List<CraftingJob> jobList)
    {
        if (jobList == null) return;

        for (int i = jobList.Count - 1; i >= 0; i--)
        {
            CraftingJob job = jobList[i];
            if (job != null && !job.isComplete)
            {
                job.timeRemaining -= Time.deltaTime;

                if (job.timeRemaining <= 0f)
                {
                    job.timeRemaining = 0f;
                    job.isComplete = true;
                    CompleteCraftingJob(job);
                    jobList.RemoveAt(i);
                }
            }
        }
    }

    private void CompleteCraftingJob(CraftingJob job)
    {
        if (job == null || job.craftingItem == null || inventoryItemPresent == null) return;

        UIItemData uiItemData = inventoryItemPresent.GetUIItemPrefab(job.craftingItem.itemID);
        if (uiItemData == null)
        {
            Debug.LogError($"UIItemData not found for itemID: {job.craftingItem.itemID}");
            return;
        }

        ItemClass itemClass = uiItemData.GetComponent<ItemClass>();
        if (itemClass == null)
        {
            Debug.LogError($"ItemClass component not found for itemID: {job.craftingItem.itemID}");
            return;
        }

        int totalAmount = job.craftingItem.amountProduced;
        int maxStack = itemClass.maxCountItem > 0 ? itemClass.maxCountItem : 1;

        while (totalAmount > 0)
        {
            int amountToAdd = Mathf.Min(totalAmount, maxStack);
            ItemData craftedItemData = new ItemData
            {
                idItem = job.craftingItem.itemID,
                nameItem = itemClass.nameItem,
                count = amountToAdd,
                maxCount = itemClass.maxCountItem,
                itemtype = itemClass.itemtype,
                parantslotType = uiItemData.slotTypeParent,
            };

            inventoryItemPresent.AddItem(craftedItemData);
            totalAmount -= amountToAdd;
        }

        if (globalstat != null)
        {
            switch (job.source)
            {
                case CraftingSource.Workshop:
                    globalstat.CraftingSlot += 1;
                    break;
                case CraftingSource.ChemicalLab:
                    globalstat.ChemicalCraftingSlot += 1;
                    break;
                case CraftingSource.Moonshine:
                    globalstat.MoonshineCraftingSlot += 1;
                    break;
            }
        }

        UpdateUsedSlots();
    }

    public void UpdateUsedSlots()
    {
        if (globalstat == null) return;
        globalstat.usedCraftingSlot = activeCraftingJobs.Count;
        globalstat.usedChemicalCraftingSlot = ChemicalactiveJobs.Count;
        globalstat.usedMoonshineCraftingSlot = MoonshienactiveJobs.Count;
    }

    private bool HasRequiredResources(CraftingItem craftingItem)
    {
        if (craftingItem == null || inventoryItemPresent == null) return false;

        if (craftingItem.recipeItems != null)
        {
            for (int i = 0; i < craftingItem.recipeItems.Count; i++)
            {
                RecipeItem recipeItem = craftingItem.recipeItems[i];
                if (recipeItem == null) continue;

                if (inventoryItemPresent.GetItemCountByID(recipeItem.itemID) < recipeItem.amountNeeded)
                {
                    return false;
                }
            }
        }

        if (buildManager != null)
        {
            if (buildManager.ammo < craftingItem.Ammoneeded ||
                buildManager.fuel < craftingItem.Fuelneeded ||
                buildManager.steel < craftingItem.Steelneeded ||
                buildManager.plank < craftingItem.Plankneeded ||
                buildManager.food < craftingItem.Foodneeded)
            {
                return false;
            }
        }

        return true;
    }

    private void DeductResources(CraftingItem craftingItem)
    {
        if (craftingItem == null) return;

        if (craftingItem.recipeItems != null && inventoryItemPresent != null)
        {
            for (int i = 0; i < craftingItem.recipeItems.Count; i++)
            {
                RecipeItem recipeItem = craftingItem.recipeItems[i];
                if (recipeItem == null) continue;
                inventoryItemPresent.RemoveItem(new ItemData { idItem = recipeItem.itemID, count = recipeItem.amountNeeded });
            }
        }

        if (buildManager != null)
        {
            buildManager.ammo -= craftingItem.Ammoneeded;
            buildManager.fuel -= craftingItem.Fuelneeded;
            buildManager.steel -= craftingItem.Steelneeded;
            buildManager.plank -= craftingItem.Plankneeded;
            buildManager.food -= craftingItem.Foodneeded;
        }
    }
}
