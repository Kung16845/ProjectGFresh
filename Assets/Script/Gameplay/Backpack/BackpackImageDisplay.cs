using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class BackpackImageDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public Image BackpackDisplay; // UI Image to display the grenade's icon

    [Header("References")]
    public UIInventory uiInventory; // Reference to the inventory system
    public InventoryItemPresent inventoryItemPresent; // Reference to manage item data and UI updates

    private void Start()
    {
        // Find references if not assigned
        if (uiInventory == null)
        {
            uiInventory = FindObjectOfType<UIInventory>();
            if (uiInventory == null)
            {
                Debug.LogError("UIInventory not found!");
                return;
            }
        }

        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
            if (inventoryItemPresent == null)
            {
                Debug.LogError("InventoryItemPresent not found!");
                return;
            }
        }

        UpdateBackpackDisplay();
    }

    private void Update()
    {
        UpdateBackpackDisplay();
    }

    private void UpdateBackpackDisplay()
    {
        // Get the equipped grenade item from the inventory
        var BackpackItemdata = uiInventory.listItemDataInventoryEqicment
            .FirstOrDefault(item => item.itemtype == Itemtype.Backpack);

        if (BackpackItemdata != null)
        {
            // Find the UIItemData corresponding to the grenade item
            var uiItemData = inventoryItemPresent.listUIItemPrefab
                .FirstOrDefault(uiItem => uiItem.idItem == BackpackItemdata.idItem);

            if (uiItemData != null)
            {
                // Update the grenade image with the sprite
                BackpackDisplay.sprite = uiItemData.itemIconImage.sprite;
                BackpackDisplay.enabled = true; // Ensure the image is visible
            }
            else
            {
                Debug.LogWarning($"UIItemData not found for item ID: {BackpackItemdata.idItem}");
                BackpackDisplay.enabled = false; // Hide the image if no match
            }
        }
        else
        {
            BackpackDisplay.enabled = false; // Hide the image if no grenade is equipped
        }
    }
}
