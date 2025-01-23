using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WeaponAmmoTracker : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI reserveAmmoText;

    [Header("References")]
    public Weapon weapon; // Reference to the Weapon script
    public UIInventory uiInventory; // Reference to the inventory system

    private Dictionary<CaliberType, int> ammoInventory;

    private void Start()
    {
        weapon = FindObjectOfType<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("Weapon reference not assigned!");
            return;
        }

        if (uiInventory == null)
        {
            uiInventory = FindObjectOfType<UIInventory>();
            if (uiInventory == null)
            {
                Debug.LogError("UIInventory not found in the scene!");
                return;
            }
        }
    }

    private void Update()
    {
        UpdateAmmoInventory();
        UpdateAmmoUI();
    }

   private void UpdateAmmoInventory()
    {
        ammoInventory = new Dictionary<CaliberType, int>();

        foreach (var item in uiInventory.listItemDataInventorySlot)
        {

            if (item.itemtype == Itemtype.Ammo)
            {
                CaliberType caliberType = ConvertAmmoIDToCaliberType(item.idItem);
                if (!ammoInventory.ContainsKey(caliberType))
                {
                    ammoInventory[caliberType] = 0;
                }
                ammoInventory[caliberType] += item.count;
            }
        }
    }

    private void UpdateAmmoUI()
    {
        // Update current ammo text
        currentAmmoText.text = weapon.currentAmmo.ToString();
        int reserveAmmo = 0;
        if (ammoInventory != null && ammoInventory.TryGetValue(weapon.caliberType, out reserveAmmo))
        {
            reserveAmmoText.text = reserveAmmo.ToString();
        }
        else
        {
            reserveAmmoText.text = "0";
        }
    }

    private CaliberType ConvertAmmoIDToCaliberType(int ammoID)
    {
        // Map ammo IDs to their respective caliber types
        switch (ammoID)
        {
            case 1020125: return CaliberType.High;
            case 1020126: return CaliberType.Shotgun;
            case 1020124: return CaliberType.Low;
            case 1020127: return CaliberType.Medium;
            default: return CaliberType.Low; // Default to Low if no match
        }
    }
}
