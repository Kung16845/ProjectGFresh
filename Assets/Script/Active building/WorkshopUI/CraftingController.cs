using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingController : MonoBehaviour
{
    public Workshop workshop;
    public WorkshopUI workshopUI; // Reference to WorkshopUI
    public InventoryItemPresent inventoryItemPresent; // Reference to inventory
    public Transform craftingSlotsParent; // Parent object containing crafting slots
    public GameObject craftingSlotPrefab; // Prefab for crafting slot
    public Globalstat globalstat;
    public int maxCraftingSlots = 3; // Max number of crafting slots
    public Button confirmCraftingButton;
    public List<GameObject> activeCraftingSlots = new List<GameObject>();
    

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
        workshop = FindObjectOfType<Workshop>();
        if (workshopUI.selectedCraftingItem == null)
        {
            Debug.LogWarning("No crafting item selected.");
            return;
        }

        CraftingItem selectedItem = workshopUI.selectedCraftingItem;
        workshopUI.DisplaySelectedItemDetails(selectedItem);
        // Try to add the crafting job to the workshop
        CraftingResult result = workshop.AddCraftingJob(selectedItem);
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
            confirmCraftingButton.interactable = globalstat.CraftingSlot > 0;
        }
        else
             confirmCraftingButton.interactable = false;
    }
    void Update()
    {
        UpdateCraftingSlotUI(); 
    }
}
