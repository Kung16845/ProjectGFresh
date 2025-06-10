using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GrenadeImageDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public Image grenadeImage; // UI Image to display the grenade's icon

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

        UpdateGrenadeImage();
    }

    private void Update()
    {
        UpdateGrenadeImage();
    }

    private void UpdateGrenadeImage()
    {
        // Get the equipped grenade item from the inventory
        var grenadeItemData = uiInventory.listItemDataInventoryEquipment
            .FirstOrDefault(item => item.itemtype == Itemtype.Grenade);

        if (grenadeItemData != null)
        {
            // Find the UIItemData corresponding to the grenade item
            var uiItemData = inventoryItemPresent.listUIItemPrefab
                .FirstOrDefault(uiItem => uiItem.idItem == grenadeItemData.idItem);

            if (uiItemData != null)
            {
                // Update the grenade image with the sprite
                grenadeImage.sprite = uiItemData.itemIconImage.sprite;
                grenadeImage.enabled = true; // Ensure the image is visible
            }
            else
            {
                Debug.LogWarning($"UIItemData not found for item ID: {grenadeItemData.idItem}");
                grenadeImage.enabled = false; // Hide the image if no match
            }
        }
        else
        {
            grenadeImage.enabled = false; // Hide the image if no grenade is equipped
        }
    }
}
