using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class WeaponImageDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public Image weaponImage1; // UI Image to display the first weapon's icon
    public Image weaponImage2; // UI Image to display the second weapon's icon

    [Header("References")]
    public WeaponManager weaponManager; // Reference to the WeaponManager script
    public UIInventory uiInventory; // Reference to the inventory system
    public InventoryItemPresent inventoryItemPresent; // Reference to manage item data and UI updates

    private void Start()
    {
        // Find references if not assigned
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
            if (weaponManager == null)
            {
                Debug.LogError("WeaponManager not found.");
                return;
            }
        }

        if (uiInventory == null)
        {
            uiInventory = FindObjectOfType<UIInventory>();
            if (uiInventory == null)
            {
                Debug.LogError("UIInventory not found.");
                return;
            }
        }

        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
            if (inventoryItemPresent == null)
            {
                Debug.LogError("InventoryItemPresent not found.");
                return;
            }
        }

        // Subscribe to the OnWeaponsChanged event
        uiInventory.OnWeaponsChanged += UpdateWeaponImages;

        // Initial update
        UpdateWeaponImages(weaponManager.equippedWeapons);
    }

    private void OnDestroy()
    {
        if (uiInventory != null)
        {
            uiInventory.OnWeaponsChanged -= UpdateWeaponImages;
        }
    }

    private void Update()
    {
        // Optionally, update the UI based on the current weapon selected
        UpdateCurrentWeaponHighlight();
    }

    private void UpdateWeaponImages(List<ItemWeapon> equippedWeapons)
    {
        // Clear images
        weaponImage1.enabled = false;
        weaponImage2.enabled = false;

        if (equippedWeapons != null && equippedWeapons.Count > 0)
        {
            for (int i = 0; i < equippedWeapons.Count; i++)
            {
                ItemWeapon itemWeapon = equippedWeapons[i];

                // Find the UIItemData corresponding to the weapon item
                var uiItemData = inventoryItemPresent.listUIItemPrefab
                    .FirstOrDefault(uiItem => uiItem.idItem == itemWeapon.idItem);

                if (uiItemData != null)
                {
                    if (i == 0)
                    {
                        // Update the weaponImage1
                        weaponImage1.sprite = uiItemData.itemIconImage.sprite;
                        weaponImage1.enabled = true;
                    }
                    else if (i == 1)
                    {
                        // Update the weaponImage2
                        weaponImage2.sprite = uiItemData.itemIconImage.sprite;
                        weaponImage2.enabled = true;
                    }
                    // If you support more weapons, add more conditions here
                }
            }
        }
    }

    private void UpdateCurrentWeaponHighlight()
    {
        // Highlight the currently selected weapon
        int currentWeaponIndex = weaponManager.currentWeaponIndex;

        // Reset colors to default
        weaponImage1.color = Color.white;
        weaponImage2.color = Color.white;

        // Dim the non-selected weapon to visually indicate which weapon is active
        Color dimColor = new Color(0.7f, 0.7f, 0.7f);

        if (currentWeaponIndex == 0)
        {
            // Weapon 1 is selected
            weaponImage1.color = Color.white;
            weaponImage2.color = dimColor;
        }
        else if (currentWeaponIndex == 1)
        {
            // Weapon 2 is selected
            weaponImage1.color = dimColor;
            weaponImage2.color = Color.white;
        }
    }
}
