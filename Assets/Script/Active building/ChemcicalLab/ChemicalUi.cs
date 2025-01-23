using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class ChemicalUi : MonoBehaviour
{
    public ChemcicalLab chemcicalLab;
    public TextMeshProUGUI Craftingslot;
    public TextMeshProUGUI ActionSpeed;

    // UI elements for selected item details
    public Image selectedItemImage;
    public TextMeshProUGUI selectedItemNameText;
    public TextMeshProUGUI selectedItemCraftingTimeText;

    public TextMeshProUGUI selectedItemammoneeded;
    public TextMeshProUGUI selectedItemPlankneeded;
    public TextMeshProUGUI selectedItemSteelneeded;
    public TextMeshProUGUI selectedItemFuelneeded;
    public TextMeshProUGUI selectedItemFoodneeded;
    public Transform recipeItemsParent;
    public GameObject recipeItemUIPrefab;
    public BuildManager buildManager;
    public InventoryItemPresent inventoryItemPresent;

    public GameObject craftingItemUIPrefab; // Assign in Inspector
    public Transform craftingItemsParent; // Assign in Inspector

    public CraftingItem selectedCraftingItem;
    public Transform craftingJobsParent; // Parent object to hold crafting job UI elements
    public GameObject craftingJobUIPrefab;
    public Globalstat globalstat;
    public CraftManager craftManager;

    

    void Start()
    {
        craftManager = FindObjectOfType<CraftManager>();
        globalstat = FindObjectOfType<Globalstat>();
        chemcicalLab = FindObjectOfType<ChemcicalLab>();
        AutoAssignCraftingItemData();
    }
    void AutoAssignCraftingItemData()
    {
        // For Level 1 Items
        foreach (CraftingItem craftingItem in chemcicalLab.craftingItemsLevel1)
        {
            AutoAssignCraftingItemProperties(craftingItem);
        }

        // For Level 2 Items
        foreach (CraftingItem craftingItem in chemcicalLab.craftingItemsLevel2)
        {
            AutoAssignCraftingItemProperties(craftingItem);
        }
    }
    public void InitializeUpgradeData()
    {
        chemcicalLab.AssignUpgradeData();
    }
    void AutoAssignCraftingItemProperties(CraftingItem craftingItem)
    {
        // Assign properties from InventoryItemPresent or UIItemData
        UIItemData uiItemData = inventoryItemPresent.listUIItemPrefab.Find(uiItem => uiItem.idItem == craftingItem.itemID);

        if (uiItemData != null)
        {
            craftingItem.itemName = uiItemData.nameItem;
            craftingItem.itemIcon = uiItemData.itemIconImage.sprite;

            // If you need rarity or other properties
            ItemClass itemClass = uiItemData.GetComponent<ItemClass>();
            if (itemClass != null)
            {
                craftingItem.rarity = itemClass.rarityItem;
            }
        }
        else
        {
            Debug.LogWarning($"UIItemData not found for itemID: {craftingItem.itemID}");
        }

        // Auto-assign for each RecipeItem
        foreach (RecipeItem recipeItem in craftingItem.recipeItems)
        {
            AutoAssignRecipeItemProperties(recipeItem);
        }
    }

    void AutoAssignRecipeItemProperties(RecipeItem recipeItem)
    {
        UIItemData uiItemData = inventoryItemPresent.listUIItemPrefab.Find(uiItem => uiItem.idItem == recipeItem.itemID);

        if (uiItemData != null)
        {
            recipeItem.itemName = uiItemData.nameItem;
            recipeItem.itemIcon = uiItemData.itemIconImage.sprite;
        }
        else
        {
            Debug.LogWarning($"UIItemData not found for itemID: {recipeItem.itemID}");
        }
    }
   public void DisplayCraftingItems()
    {
        // Clear existing crafting item UI elements
        foreach (Transform child in craftingItemsParent)
        {
            Destroy(child.gameObject);
        }

        // Get the chemcicalLab's current level
        int chemcicalLabLevel = chemcicalLab.upgradeBuilding.currentLevel;

        // Get the crafting items based on the level
        List<CraftingItem> availableCraftingItems = new List<CraftingItem>();

        // Level 1 items are always included
        availableCraftingItems.AddRange(chemcicalLab.craftingItemsLevel1);

        // If level >= 2, include level 2 items
        if (chemcicalLabLevel >= 2)
        {
            availableCraftingItems.AddRange(chemcicalLab.craftingItemsLevel2);
        }

        // Order the list by rarity and then by name
        var orderedCraftingItems = availableCraftingItems.OrderBy(item => item.rarity)
                                                        .ThenBy(item => item.itemName)
                                                        .ToList();

        // Instantiate the crafting item UI prefabs
        foreach (CraftingItem craftingItem in orderedCraftingItems)
        {
            GameObject newCraftingItemUI = Instantiate(craftingItemUIPrefab, craftingItemsParent);
            ChemicalItemUI chemicalUi = newCraftingItemUI.GetComponent<ChemicalItemUI>();
            if (chemicalUi != null)
            {
                chemicalUi.Initialize(craftingItem, this);
            }
        }
    }
    public void DisplaySelectedItemDetails(CraftingItem selectedItem)
    {
        selectedCraftingItem = selectedItem;

        if (selectedItem != null)
        {
            if (selectedItemImage != null)
                selectedItemImage.sprite = selectedItem.itemIcon;

            if (selectedItemNameText != null)
                selectedItemNameText.text = $"{selectedItem.itemName} X {selectedItem.amountProduced}";

            if (selectedItemCraftingTimeText != null)
                selectedItemCraftingTimeText.text = $"{(selectedItem.craftingTime / 1000f):F1} hr";

            // Ammo Needed
            UpdateTextWithColor(selectedItemammoneeded, selectedItem.Ammoneeded, buildManager.ammo);

            // Plank Needed
            UpdateTextWithColor(selectedItemPlankneeded, selectedItem.Plankneeded, buildManager.plank);

            // Steel Needed
            UpdateTextWithColor(selectedItemSteelneeded, selectedItem.Steelneeded, buildManager.steel);

            // Fuel Needed
            UpdateTextWithColor(selectedItemFuelneeded, selectedItem.Fuelneeded, buildManager.fuel);

            // Food Needed
            UpdateTextWithColor(selectedItemFoodneeded, selectedItem.Foodneeded, buildManager.food);
        }
        else
        {
            ClearUIElements();
        }

        // Clear existing recipe items
        foreach (Transform child in recipeItemsParent)
        {
            Destroy(child.gameObject);
        }

        if (selectedItem != null)
        {
            // Display recipe items
            foreach (RecipeItem recipeItem in selectedItem.recipeItems)
            {
                GameObject recipeItemUIObject = Instantiate(recipeItemUIPrefab, recipeItemsParent);
                RecipeItemUI recipeItemUIScript = recipeItemUIObject.GetComponent<RecipeItemUI>();

                int amountHave = inventoryItemPresent.GetItemCountByID(recipeItem.itemID);

                recipeItemUIScript.Initialize(recipeItem, amountHave);
            }
        }
    }

    private void UpdateTextWithColor(TextMeshProUGUI textElement, int neededAmount, int availableAmount)
    {
        if (textElement != null)
        {
            textElement.text = $"{neededAmount}/{availableAmount}";

            // Change color: Yellow if the player doesn't have enough, default (white) if sufficient
            if (availableAmount < neededAmount)
            {
                textElement.color = Color.yellow; // Not enough resources
            }
            else
            {
                textElement.color = Color.white; // Sufficient resources
            }
        }
    }
    private void ClearUIElements()
    {
        if (selectedItemImage != null)
            selectedItemImage.sprite = null;

        if (selectedItemNameText != null)
            selectedItemNameText.text = "";

        if (selectedItemCraftingTimeText != null)
            selectedItemCraftingTimeText.text = "";

        if (selectedItemammoneeded != null)
            selectedItemammoneeded.text = "0";

        if (selectedItemPlankneeded != null)
            selectedItemPlankneeded.text = "0";

        if (selectedItemSteelneeded != null)
            selectedItemSteelneeded.text = "0";

        if (selectedItemFuelneeded != null)
            selectedItemFuelneeded.text = "0";

        if (selectedItemFoodneeded != null)
            selectedItemFoodneeded.text = "0";
    }
    public void DisplayActiveCraftingJobs()
    {
        // Clear existing crafting job UI elements
        foreach (Transform child in craftingJobsParent)
        {
            Destroy(child.gameObject);
        }

        // Display current crafting jobs
        foreach (CraftingJob job in craftManager.ChemicalactiveJobs)
        {
            GameObject jobUIObject = Instantiate(craftingJobUIPrefab, craftingJobsParent);
            CraftingJobUI jobUIScript = jobUIObject.GetComponent<CraftingJobUI>();
            if (jobUIScript != null)
            {
                jobUIScript.Initialize(job);
            }
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            this.gameObject.SetActive(false);
        }
    }
}
