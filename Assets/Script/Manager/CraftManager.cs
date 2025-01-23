using System.Collections;
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
    public Globalstat globalstat; // Reference to Globalstat

    void Awake()
    {
        globalstat = FindObjectOfType<Globalstat>();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        buildManager = FindObjectOfType<BuildManager>();

        // Initialize used slots based on the current job lists
        UpdateUsedSlots();
    }

    public CraftingResult AddCraftingJob(CraftingItem craftingItem, CraftingSource source)
    {
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
                    Debug.LogWarning("Crafting is not allowed in Chemical Lab: No active crafting slots.");
                    return CraftingResult.NoAvailableSlots;
                }
                break;
            
            default:
                Debug.LogWarning("Unknown crafting source.");
                return CraftingResult.NoAvailableSlots;
        }

        // Check if the player has enough items for the recipe
        foreach (RecipeItem recipeItem in craftingItem.recipeItems)
        {
            int amountHave = inventoryItemPresent.GetItemCountByID(recipeItem.itemID);
            if (amountHave < recipeItem.amountNeeded)
            {
                Debug.LogWarning($"Not enough {recipeItem.itemName}. Required: {recipeItem.amountNeeded}, Have: {amountHave}");
                return CraftingResult.NotEnoughItems;
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

        // Deduct resources
        DeductResources(craftingItem);

        // Use ActionSpeed to calculate crafting time
        float actionSpeed = globalstat.ActionSpeed > 0 ? globalstat.ActionSpeed : 1f;
        CraftingJob newJob = new CraftingJob(craftingItem, actionSpeed, source);

        // Add the job to the specified list based on source
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

        // Update used slots after adding the job
        UpdateUsedSlots();

        return CraftingResult.Success;
    }

    public void UpdateCraftingJobs()
    {
        UpdateJobs(activeCraftingJobs);
        UpdateJobs(ChemicalactiveJobs);
        UpdateJobs(MoonshienactiveJobs);
        // Update used slots after processing jobs
        UpdateUsedSlots();
    }

    private void UpdateJobs(List<CraftingJob> jobList)
    {
        for (int i = jobList.Count - 1; i >= 0; i--)
        {
            CraftingJob job = jobList[i];
            if (!job.isComplete)
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
        // Handle completed crafting job
        UIItemData uiItemData = inventoryItemPresent.listUIItemPrefab.Find(uiItem => uiItem.idItem == job.craftingItem.itemID);
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
        int maxStack = itemClass.maxCountItem;

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
            Debug.Log($"Crafting complete: {job.craftingItem.itemName}, Amount Added: {amountToAdd}");
            totalAmount -= amountToAdd;
        }

        // Increase the appropriate crafting slot based on the job source
        switch (job.source)
        {
            case CraftingSource.Workshop:
                globalstat.CraftingSlot += 1; // Free up a Workshop slot
                break;
            case CraftingSource.ChemicalLab:
                globalstat.ChemicalCraftingSlot += 1; // Free up a Chemical Lab slot
                break;
            case CraftingSource.Moonshine:
                globalstat.MoonshineCraftingSlot += 1;
                break;
            default:
                Debug.LogWarning("Unknown crafting source when completing job.");
                break;
        }

        // Update used slots after completing the job
        UpdateUsedSlots();
    }
    void Update()
    {
        UpdateUsedSlots();
    }
    private void UpdateUsedSlots()
    {
        // Update the globalstat used slot values
        globalstat.usedCraftingSlot = activeCraftingJobs.Count;
        globalstat.usedChemicalCraftingSlot = ChemicalactiveJobs.Count;
        globalstat.usedMoonshineCraftingSlot = MoonshienactiveJobs.Count;
    }

    private bool HasRequiredResources(CraftingItem craftingItem)
    {
        foreach (RecipeItem recipeItem in craftingItem.recipeItems)
        {
            if (inventoryItemPresent.GetItemCountByID(recipeItem.itemID) < recipeItem.amountNeeded)
            {
                Debug.LogWarning($"Not enough {recipeItem.itemName}.");
                return false;
            }
        }

        if (buildManager.ammo < craftingItem.Ammoneeded || buildManager.fuel < craftingItem.Fuelneeded ||
            buildManager.steel < craftingItem.Steelneeded || buildManager.plank < craftingItem.Plankneeded ||
            buildManager.food < craftingItem.Foodneeded)
        {
            Debug.LogWarning("Not enough resources.");
            return false;
        }

        return true;
    }

    private void DeductResources(CraftingItem craftingItem)
    {
        foreach (RecipeItem recipeItem in craftingItem.recipeItems)
        {
            inventoryItemPresent.RemoveItem(new ItemData { idItem = recipeItem.itemID, count = recipeItem.amountNeeded });
        }

        buildManager.ammo -= craftingItem.Ammoneeded;
        buildManager.fuel -= craftingItem.Fuelneeded;
        buildManager.steel -= craftingItem.Steelneeded;
        buildManager.plank -= craftingItem.Plankneeded;
        buildManager.food -= craftingItem.Foodneeded;
    }
}

