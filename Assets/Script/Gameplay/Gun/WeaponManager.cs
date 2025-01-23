using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public List<ItemWeapon> equippedWeapons = new List<ItemWeapon>();
    public Weapon weaponComponent; // Reference to the Weapon script
    private ActionController actionController;

    public int currentWeaponIndex = 0;
    private UIInventory uiInventory;

    // Store ammo for each weapon
    private Dictionary<int, int> weaponAmmoDict = new Dictionary<int, int>();

    void Start()
    {
        uiInventory = FindObjectOfType<UIInventory>();
        actionController = GetComponent<ActionController>();
        if (uiInventory != null)
        {
            uiInventory.OnWeaponsChanged += OnWeaponsChanged;
        }
        else
        {
            Debug.LogWarning("UIInventory not found.");
        }

        weaponComponent = GetComponent<Weapon>();
    }

    void OnWeaponsChanged(List<ItemWeapon> itemWeapons)
    {
        SaveCurrentWeaponAmmo();
        if (itemWeapons != null && itemWeapons.Count > 0)
        {
            equippedWeapons = itemWeapons;
            currentWeaponIndex = 0; // Reset to first weapon

            // Initialize ammo dict
            foreach (var itemWeapon in itemWeapons)
            {
                if (!weaponAmmoDict.ContainsKey(itemWeapon.idItem))
                {
                    weaponAmmoDict[itemWeapon.idItem] = itemWeapon.capacity; // Or some default value
                }
            }

            EquipWeapon(currentWeaponIndex);
        }
        else
        {
            equippedWeapons.Clear();
            weaponComponent.DisableWeapon();
        }
    }

    void Update()
    {
        if(actionController.canchangeweapond)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchWeapon(0);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchWeapon(1);
            }
        }
    }

    void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < equippedWeapons.Count)
        {
            // Save current weapon ammo
            SaveCurrentWeaponAmmo();

            currentWeaponIndex = weaponIndex;
            EquipWeapon(currentWeaponIndex);
        }
    }

    void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < equippedWeapons.Count)
        {
            var itemWeapon = equippedWeapons[weaponIndex];

            // Load ammo for this weapon
            if (weaponAmmoDict.TryGetValue(itemWeapon.idItem, out int ammo))
            {
                weaponComponent.currentAmmo = ammo;
            }
            else
            {
                weaponComponent.currentAmmo = itemWeapon.capacity;
                weaponAmmoDict[itemWeapon.idItem] = weaponComponent.currentAmmo;
            }

            weaponComponent.UpdateWeaponStats(itemWeapon);
        }
    }

    void SaveCurrentWeaponAmmo()
    {
        if (currentWeaponIndex >= 0 && currentWeaponIndex < equippedWeapons.Count)
        {
            var currentItemWeapon = equippedWeapons[currentWeaponIndex];
            weaponAmmoDict[currentItemWeapon.idItem] = weaponComponent.currentAmmo;
        }
    }

    private void OnDestroy()
    {
        if (uiInventory != null)
        {
            uiInventory.OnWeaponsChanged -= OnWeaponsChanged;
        }
    }
}
