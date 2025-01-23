using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoonshineCraftingController : MonoBehaviour
{
    public Moonshine moonshine;
    public MoonshinUI moonshineUI;
    public InventoryItemPresent inventoryItemPresent; // Reference to inventory
    public Transform craftingSlotsParent; // Parent object containing crafting slots
    public GameObject craftingSlotPrefab; // Prefab for crafting slot
    public int maxCraftingSlots = 3; // Max number of crafting slots
    public Button confirmCraftingButton;
    public Globalstat globalstat;
    private List<GameObject> activeCraftingSlots = new List<GameObject>();
    void Start()
    {
        globalstat = FindObjectOfType<Globalstat>();
        if (confirmCraftingButton != null)
        {
            confirmCraftingButton.onClick.AddListener(OnConfirmCraftingButtonClicked);
        }
        else
        {
            Debug.LogError("Confirm Crafting button not found.");
        }
    }

    void OnConfirmCraftingButtonClicked()
    {
        moonshine = FindObjectOfType<Moonshine>();
        if (moonshineUI.selectedCraftingItem == null)
        {
            Debug.LogWarning("No crafting item selected.");
            return;
        }

        CraftingItem selectedItem = moonshineUI.selectedCraftingItem;

        // Try to add the crafting job to the moonshine
        CraftingResult result = moonshine.AddCraftingJob(selectedItem);
        moonshineUI.DisplaySelectedItemDetails(selectedItem);
        switch (result)
        {
            case CraftingResult.Success:
                Debug.Log($"Started crafting: {selectedItem.itemName}");
                break;

            case CraftingResult.NoAvailableSlots:
                Debug.LogWarning("No available crafting slots.");
                // Optionally, display a message to the player
                break;

            case CraftingResult.NotEnoughItems:
                Debug.LogWarning("Not enough items to start crafting.");
                // Optionally, display a message to the player
                break;
        }
    }

    void AddCraftingSlot(CraftingItem craftingItem)
    {
        // Instantiate the crafting slot prefab
        GameObject craftingSlotObject = Instantiate(craftingSlotPrefab, craftingSlotsParent);
        CraftingSlot craftingSlot = craftingSlotObject.GetComponent<CraftingSlot>();
        if (craftingSlot != null)
        {
            craftingSlot.Initialize(craftingItem, inventoryItemPresent);
            activeCraftingSlots.Add(craftingSlotObject);
        }
        else
        {
            Debug.LogError("CraftingSlot component not found on crafting slot prefab.");
            Destroy(craftingSlotObject);
        }
    }

    public void RemoveCraftingSlot(GameObject slotObject)
    {
        if (activeCraftingSlots.Contains(slotObject))
        {
            activeCraftingSlots.Remove(slotObject);
            Destroy(slotObject);
        }
    }
     public void UpdateCraftingSlotUI()
    {
        if (confirmCraftingButton != null)
        {
            confirmCraftingButton.interactable = globalstat.MoonshineCraftingSlot > 0;
        }
        else
             confirmCraftingButton.interactable = false;
    }
    void Update()
    {
        UpdateCraftingSlotUI(); 
    }
}
