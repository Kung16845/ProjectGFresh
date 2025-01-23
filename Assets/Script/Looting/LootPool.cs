using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootItem
{
    public ItemData item; // Reference to the item
    public float dropChance; // Drop chance percentage (0-100)
    public int minAmount = 1; // Minimum quantity of this item
    public int maxAmount = 1; // Maximum quantity of this item
}

public class LootPool : MonoBehaviour
{
    public List<LootItem> lootItems = new List<LootItem>();

    // Range for the number of items to drop per loot event
    [Header("Loot Quantity Settings")]
    [Tooltip("Minimum number of items to drop at once.")]
    public int minItemsToDrop = 1;
    [Tooltip("Maximum number of items to drop at once.")]
    public int maxItemsToDrop = 3;

    // Struct to return both the chosen item and the quantity
    public struct LootResult
    {
        public ItemData item;
        public int amount;
    }

    public List<LootResult> GetRandomLoot()
    {
        List<LootResult> results = new List<LootResult>();

        if (lootItems == null || lootItems.Count == 0)
            return results;

        // Determine how many items to drop this time
        int itemsToDrop = Random.Range(minItemsToDrop, maxItemsToDrop + 1);

        for (int i = 0; i < itemsToDrop; i++)
        {
            LootResult result = GetSingleRandomLoot();
            if (result.item != null)
            {
                // Avoid adding duplicate items unless intended
                // If duplicates are allowed, you can comment out the following lines
                if (!results.Exists(r => r.item.idItem == result.item.idItem))
                {
                    results.Add(result);
                }
                else
                {
                    // Optionally, combine quantities if duplicate is found
                    var existing = results.Find(r => r.item.idItem == result.item.idItem);
                    existing.amount += result.amount;
                    results.RemoveAll(r => r.item.idItem == result.item.idItem);
                    results.Add(existing);
                }
            }
        }

        return results;
    }

    private LootResult GetSingleRandomLoot()
    {
        float totalChance = 0f;
        foreach (var lootItem in lootItems)
        {
            totalChance += lootItem.dropChance;
        }

        if (totalChance <= 0f)
            return new LootResult { item = null, amount = 0 };

        float randomValue = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (var lootItem in lootItems)
        {
            cumulativeChance += lootItem.dropChance;
            if (randomValue <= cumulativeChance)
            {
                int amount = Random.Range(lootItem.minAmount, lootItem.maxAmount + 1);
                return new LootResult { item = lootItem.item, amount = amount };
            }
        }

        // Fallback in case no item is selected
        return new LootResult { item = null, amount = 0 };
    }
}
