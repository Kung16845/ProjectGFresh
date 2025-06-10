using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class VestImageDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public Image VestImage; // UI Image to display the grenade's icon

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

        UpdateVestImage();
    }

    private void Update()
    {
        UpdateVestImage();
    }

    private void UpdateVestImage()
    {
        // Get the equipped grenade item from the inventory
        var VestItemdata = uiInventory.listItemDataInventoryEquipment
            .FirstOrDefault(item => item.itemtype == Itemtype.Vest);

        if (VestItemdata != null)
        {
            // Find the UIItemData corresponding to the grenade item
            var uiItemData = inventoryItemPresent.listUIItemPrefab
                .FirstOrDefault(uiItem => uiItem.idItem == VestItemdata.idItem);

            if (uiItemData != null)
            {
                // Update the grenade image with the sprite
                VestImage.sprite = uiItemData.itemIconImage.sprite;
                VestImage.enabled = true; // Ensure the image is visible
            }
            else
            {
                Debug.LogWarning($"UIItemData not found for item ID: {VestItemdata.idItem}");
                VestImage.enabled = false; // Hide the image if no match
            }
        }
        else
        {
            VestImage.enabled = false; // Hide the image if no grenade is equipped
        }
    }
}
