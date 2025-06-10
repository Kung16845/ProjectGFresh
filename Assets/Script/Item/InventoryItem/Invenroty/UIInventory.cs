using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.SceneManagement;
public class UIInventory : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Image spriteHeadNpc;
    public NpcManager npcManager;
    public NpcClass npcSelecting;
    public List<InventorySlots> listInventorySlotsUI = new List<InventorySlots>();
    public event Action<List<ItemWeapon>> OnWeaponsChanged;
    public event Action<ItemVest> OnVestChanged;
    public event Action<ItemBackpack> OnBackpackChanged;
    public List<ItemData> listItemDataInventoryEquipment;
    public List<ItemData> listItemDataInventorySlot;
    public Transform transformBoxes;
    public InventoryItemPresent inventoryItemPresent;
    [Header("TextMeshProUGUI")]
    public TextMeshProUGUI levelEnduranceText;
    public TextMeshProUGUI levelCombatText;
    public TextMeshProUGUI levelSpeedText;
    public TextMeshProUGUI specialistNpcText;
    public TextMeshProUGUI nameNpcText;
    public int SlotHasincreased;
    public int currentNumCategory;
    public LootingSystem currentLootingSystem;
    public TradesystemScript currentTradesystem;
    
    public void RemoveItemData(ItemClass itemClass, SlotType slotType)
    {
        ItemData itemData = null;

        if (slotType == SlotType.SlotBag || slotType == SlotType.SlotBoxes)
        {
            // Remove from inventory list
            itemData = listItemDataInventorySlot.FirstOrDefault(item => item.idItem == itemClass.idItem);

            if (itemData != null)
            {
                itemData.count--;
                if (itemData.count <= 0)
                {
                    listItemDataInventorySlot.Remove(itemData);
                }
            }
        }
        else
        {
            // Remove from equipment list
            itemData = listItemDataInventoryEquipment.FirstOrDefault(item => item.idItem == itemClass.idItem);

            if (itemData != null)
            {
                itemData.count--;
                if (itemData.count <= 0)
                {
                    listItemDataInventoryEquipment.Remove(itemData);
                }
            }
        }
    }


    public void AddItemlistInvenrotySlots(ItemClass itemClass)
    {
        // Find an existing entry for this item
        ItemData itemData = listItemDataInventorySlot.FirstOrDefault(item => item.idItem == itemClass.idItem);

        if (listItemDataInventorySlot.Count < npcSelecting.countInventorySlot)
        {
            if (itemData != null)
            {
                int totalQuantity = itemData.count + itemClass.quantityItem;

                if (totalQuantity <= itemData.maxCount)
                {
                    // Fits in the same slot
                    itemData.count = totalQuantity;
                }
                else
                {
                    // Exceeds maxCount, split the excess
                    itemData.count = itemData.maxCount;
                    int excess = totalQuantity - itemData.maxCount;

                    // Create a new entry for the excess
                    while (excess > 0)
                    {
                        int newSlotQuantity = Math.Min(excess, itemData.maxCount);
                        ItemData newItemData = new ItemData
                        {
                            idItem = itemClass.idItem,
                            nameItem = itemClass.nameItem,
                            count = newSlotQuantity,
                            maxCount = itemData.maxCount,
                            itemtype = itemData.itemtype
                        };

                        listItemDataInventorySlot.Add(newItemData);
                        excess -= newSlotQuantity;
                    }
                }
            }
            else
            {
                // No existing entry, add a new one
                listItemDataInventorySlot.Add(new ItemData
                {
                    idItem = itemClass.idItem,
                    nameItem = itemClass.nameItem,
                    count = itemClass.quantityItem,
                    maxCount = itemClass.maxCountItem,
                    itemtype = itemClass.itemtype
                });
            }
        }
        else
        {
            Debug.LogWarning("Inventory is full. Cannot add more items.");
        }
    }
    public void BindInventorySlotsToData()
    {
        // Step 1: Clear all children in the inventory slots UI
        ClearAllChildInvenrotySlot();

        // Step 2: Ensure that the number of UI slots matches the inventory data
        for (int i = 0; i < listInventorySlotsUI.Count; i++)
        {
            // If there's a corresponding item in the inventory data, bind it
            if (i < listItemDataInventorySlot.Count)
            {
                var itemData = listItemDataInventorySlot[i];
                CreateUIItem(itemData, listInventorySlotsUI[i]);
            }
        }

        Debug.Log($"Inventory slots and data bound successfully. Total slots: {listInventorySlotsUI.Count}, Total items: {listItemDataInventorySlot.Count}");
    }
    public void SetCostumeNpcExpentdition(NpcClass npcClass, GameObject npcOBJ)
    {
        HeadCoutume headCoutume = npcManager.listHeadCoutume.FirstOrDefault(coutume => coutume.idHead == npcClass.idHead);
        BodyCoutume bodyCoutume = npcManager.listBodyCoutume.FirstOrDefault(coutume => coutume.idBody == npcClass.idBody);
        FeedCoutume feedCoutume = npcManager.listFeedCoutume.FirstOrDefault(coutume => coutume.idFeed == npcClass.idFeed);

        NpcCoutume npcCoutume = npcOBJ.GetComponent<NpcCoutume>();

        npcCoutume.SetCostume(headCoutume, bodyCoutume, feedCoutume);
    }
    public void SetValuableUIInventory()
    {

        inventoryItemPresent = GameManager.Instance.inventoryItemPresent;
        inventoryItemPresent.targetObject = this.gameObject;

        npcManager = GameManager.Instance.npcManager;
        npcManager.uIInventory = this;
        npcManager.levelCombatText = levelCombatText;
        npcManager.levelEnduranceText = levelEnduranceText;
        npcManager.levelSpeedText = levelSpeedText;
        npcManager.specialistNpcText = specialistNpcText;

        SetSlotToInventory();
        dropdown.AddOptions(npcManager.SetNpcOptionDropDown());
        dropdown.onValueChanged.AddListener(npcManager.OnDropdownValueChanged);

        Debug.Log("This run");
        npcManager.OnDropdownValueChanged(0);
        inventoryItemPresent.RefreshUIBox();
    }
    public void SetSlotToInventory()
    {
        inventoryItemPresent.listInvenrotySlots.Clear();

        inventoryItemPresent.invenrotySlotSpecialMilitaryLock = listInventorySlotsUI.ElementAt(13);
        inventoryItemPresent.invenrotySlotSpecialScavengerLock = listInventorySlotsUI.ElementAt(16);

        for (int i = 0; i < 12; i++)
        {
            inventoryItemPresent.listInvenrotySlots.Add(listInventorySlotsUI.ElementAt(i));
        }
        if(transformBoxes != null)
            inventoryItemPresent.transformsBoxes = transformBoxes;
    }
    public void RefreshUIBoxCategory(int numCategory)
    {    
        currentNumCategory = numCategory;
        if(numCategory == -1)
        {
            inventoryItemPresent.RefreshUIBox();
            return;
        }
        inventoryItemPresent.ClearUIBoxes();
        
        Itemtype itemtypeCategory = (Itemtype)numCategory;
        
        foreach (ItemData itemData in inventoryItemPresent.listItemsDataBox)
        {
            if (itemData.itemtype == itemtypeCategory)
            {
                inventoryItemPresent.CreateUIItemInBoxes(itemData);

            }
        }

    }
    public virtual void RefreshUIInventory()
    {   
        
        // 1. Clear all existing UI items
        ClearAllChildInvenrotySlot();
        // 2. Combine items with the same idItem in listItemDataInventorySlot
        CombineAndSplitItems(listItemDataInventorySlot);
        CombineAndSplitItems(listItemDataInventoryEquipment);

        // 3. Default inventory slots based on NPC (no backpack)
        npcSelecting.countInventorySlot = 6; // Default inventory slots

        // 4. Handle equipment items, including backpacks, weapons, and tools
        for (int i = 12; i < listInventorySlotsUI.Count; i++)
        {
            InventorySlots eqSlot = listInventorySlotsUI[i];
            SlotType slotType = eqSlot.slotTypeInventory;

            // Identify items based on the slot type
            if (slotType == SlotType.SlotWeapon || slotType == SlotType.SlotTool)
            {
                // Find corresponding items
                var matchingItems = listItemDataInventoryEquipment
                    .Where(item => GetSlotTypeForItemType(item.itemtype) == slotType)
                    .ToList();

                // Assign items to the correct slot
                if (slotType == SlotType.SlotWeapon)
                {
                    if (matchingItems.Count > 0)
                    {
                        if (i == 12 && matchingItems.Count >= 1)
                        {
                            CreateUIItem(matchingItems[0], eqSlot); // Primary Weapon Slot
                        }
                        else if (i == 13 && matchingItems.Count >= 2)
                        {
                            CreateUIItem(matchingItems[1], eqSlot); // Secondary Weapon Slot
                        }
                    }
                }
                else if (slotType == SlotType.SlotTool)
                {
                    if (matchingItems.Count > 0)
                    {
                        if (i == 15 && matchingItems.Count >= 1)
                        {
                            CreateUIItem(matchingItems[0], eqSlot); // Primary Tool Slot
                        }
                        else if (i == 16 && matchingItems.Count >= 2)
                        {
                            CreateUIItem(matchingItems[1], eqSlot); // Secondary Tool Slot
                        }
                    }
                }
            }
            else
            {
                // Find other equipment items (e.g., backpack)
                ItemData eqItem = listItemDataInventoryEquipment.FirstOrDefault(item => GetSlotTypeForItemType(item.itemtype) == slotType);
                if (eqItem != null)
                {
                    CreateUIItem(eqItem, eqSlot);

                    // Adjust inventory slots for backpacks
                    if (eqItem.itemtype == Itemtype.Backpack)
                    {
                        ItemBackpack backpack = inventoryItemPresent.listUIItemPrefab
                            .First(ui => ui.idItem == eqItem.idItem)
                            .GetComponent<ItemBackpack>();

                        if (backpack != null)
                        {
                            npcSelecting.countInventorySlot += backpack.slotIncreasing;
                            SlotHasincreased = backpack.slotIncreasing;
                        }
                    }
                }
            }
        }

        // 5. Remove items beyond the allowed slots
        if (listItemDataInventorySlot.Count > npcSelecting.countInventorySlot)
        {
            int excessItemCount = listItemDataInventorySlot.Count - npcSelecting.countInventorySlot;

            // Remove excess items starting from the last slot
            for (int i = listItemDataInventorySlot.Count - 1; i >= npcSelecting.countInventorySlot; i--)
            {
                Debug.Log($"Removing item {listItemDataInventorySlot[i].nameItem} from slot {i} due to slot constraints.");
                listItemDataInventorySlot.RemoveAt(i);
            }
        }

        // 6. Unlock and adjust slots based on the current inventory slot count
        inventoryItemPresent.UnlockSlotInventory(npcSelecting.countInventorySlot, npcSelecting.roleNpc, listItemDataInventoryEquipment);

        // 7. Bind inventory items to UI slots
        int maxBagSlots = 12;
        for (int i = 0; i < maxBagSlots; i++)
        {
            if (i < listItemDataInventorySlot.Count)
            {
                // Place this item in slot i
                CreateUIItem(listItemDataInventorySlot[i], listInventorySlotsUI[i]);
            }
        }
        // 8. If this is UIInventoryEX, also bind car slots directly from listItemDataCarInventorySlot
        UIInventoryEX exUI = this as UIInventoryEX;
        if (exUI != null && exUI.listItemDataCarInventorySlot != null && exUI.listInvenrotyCarSlotsUI != null)
        {
            exUI.BindCarSlotsToData(); // Ensures car slots are also updated
        }
    }
    public void CombineAndSplitItems(List<ItemData> items)
    {
        Dictionary<int, int> itemCountMap = new Dictionary<int, int>();
        List<ItemData> updatedItems = new List<ItemData>();

        // Combine items by idItem
        foreach (var item in items)
        {
            if (itemCountMap.ContainsKey(item.idItem))
            {
                itemCountMap[item.idItem] += item.count;
            }
            else
            {
                itemCountMap[item.idItem] = item.count;
            }
        }

        // Split items if count exceeds maxCount
        foreach (var kvp in itemCountMap)
        {
            int itemId = kvp.Key;
            int totalQuantity = kvp.Value;

            // Get the template item to copy other properties
            ItemData templateItem = items.First(item => item.idItem == itemId);

            while (totalQuantity > 0)
            {
                int splitCount = Mathf.Min(totalQuantity, templateItem.maxCount);
                updatedItems.Add(new ItemData
                {
                    idItem = templateItem.idItem,
                    nameItem = templateItem.nameItem,
                    count = splitCount,
                    maxCount = templateItem.maxCount,
                    itemtype = templateItem.itemtype
                });

                totalQuantity -= splitCount;
            }
        }

        // Update the original list in place
        items.Clear();
        items.AddRange(updatedItems);
    }

    private SlotType GetSlotTypeForItemType(Itemtype itemType)
    {
        switch (itemType)
        {
            case Itemtype.Weapon:
                return SlotType.SlotWeapon;
            case Itemtype.Vest:
                return SlotType.SlotVest;
            case Itemtype.Backpack:
                return SlotType.SlotBackpack;
            case Itemtype.Tool:
                return SlotType.SlotTool;
            case Itemtype.Grenade:
                return SlotType.SlotGrenade;
            // Add other mappings as needed
            default:
                return SlotType.SlotLock; // Indicating no valid slot
        }
    }
    public StatAmplifier statAmplifier;
    public void SelectNpcDefenseScene()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        statAmplifier = FindFirstObjectByType<StatAmplifier>();
        StatManager statManager = FindFirstObjectByType<StatManager>();

        // Update StatAmplifier properties
        statAmplifier.SetStatAmplifer(npcSelecting);

        // Initialize amplifiers
        statAmplifier.InitializeAmplifiers();

        // Notify StatManager of the change
        statManager.OnStatAmplifierChanged();

        // Update player's current stamina based on new max stamina
        player.currentStamina = statManager.maxStamina;

        // If you have a weapon equipped, ensure it updates the base stats
        Weapon weapon = player.GetComponent<Weapon>();

        if (weapon != null)
        {
            weapon.OnStatsChanged();
        }
        npcSelecting.isWorking = true;;
        SetCostumeNpcExpentdition(npcSelecting, player.gameObject);
    }
    public void ClearItemDataInAllInventorySlotToListDataBoxes()
    {
        Debug.Log("ClearItemDataInAllInventorySlotToListDataBoxes");

        foreach (InventorySlots slotsItem in listInventorySlotsUI)
        {
            ItemClass itemClass = slotsItem.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                // Convert ItemClass to ItemData
                ItemData itemData = inventoryItemPresent.ConventItemClassToItemData(itemClass);

                // Check if the item ID maps to a resource
                if (BuildManager.Instance != null)
                {
                    BuildManager.Instance.AddResource(itemData.idItem, itemData.count);
                }
                inventoryItemPresent.AddItem(itemData);
                // Destroy the item GameObject
                Destroy(itemClass.gameObject);
            }
        }
    }

    public void ClearAllChildInvenrotySlot()
    {
        foreach (InventorySlots slotsItem in listInventorySlotsUI)
        {
            // Create a temporary list of children to avoid modifying the collection while iterating.
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in slotsItem.transform)
            {
                children.Add(child.gameObject);
            }

            // Destroy all children game objects
            foreach (GameObject child in children)
            {
                Debug.Log("Clearing item: " + child.name);
                Destroy(child);
            }
        }
    }

    public virtual void ConventAllUIItemInListInventorySlotToListItemData(List<ItemData> listSlotItemDatas)
    {
        for (int i = 0; i < 12; i++)
        {
            ItemClass itemClass = listInventorySlotsUI.ElementAt(i).GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                //  Debug.Log("Item Class quantityItem : " + itemClass.quantityItem);
                ItemData itemData = inventoryItemPresent.ConventItemClassToItemData(itemClass);

                // Debug.Log(itemData.count);
                listSlotItemDatas.Add(itemData);
            }
        }
    }
    public virtual void ConventAllUIItemInListInventorySlotToListEquipmentItemData(List<ItemData> listEqicmentItemDatas)
    {
        for (int i = 12; i < 19; i++)
        {
            ItemClass itemClass = listInventorySlotsUI.ElementAt(i).GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                ItemData itemData = inventoryItemPresent.ConventItemClassToItemData(itemClass);
                listEqicmentItemDatas.Add(itemData);
            }
        }
    }
    public virtual void ConventDataUIToItemData()
    {
        listItemDataInventorySlot.Clear();
        listItemDataInventoryEquipment.Clear();
        ConventAllUIItemInListInventorySlotToListItemData(listItemDataInventorySlot);
        ConventAllUIItemInListInventorySlotToListEquipmentItemData(listItemDataInventoryEquipment);

        // Weapon logic
        List<ItemData> weaponItemDataList = listItemDataInventoryEquipment
        .Where(item => item.itemtype == Itemtype.Weapon)
        .ToList();

        if (weaponItemDataList.Count > 0)
        {
            List<ItemWeapon> itemWeapons = new List<ItemWeapon>();

            foreach (var weaponItemData in weaponItemDataList)
            {
                UIItemData uiItemData = inventoryItemPresent.listUIItemPrefab
                    .FirstOrDefault(uiItem => uiItem.idItem == weaponItemData.idItem);

                if (uiItemData != null)
                {
                    ItemWeapon itemWeapon = uiItemData.GetComponent<ItemWeapon>();
                    itemWeapons.Add(itemWeapon);
                }
            }

            OnWeaponsChanged?.Invoke(itemWeapons);
        }
        else
        {
            OnWeaponsChanged?.Invoke(null);
        }

        // Vest logic
        ItemData vestItemData = listItemDataInventoryEquipment.FirstOrDefault(item => item.itemtype == Itemtype.Vest);
        if (vestItemData != null)
        {
            UIItemData uiItemData = inventoryItemPresent.listUIItemPrefab
                .FirstOrDefault(uiItem => uiItem.idItem == vestItemData.idItem);

            if (uiItemData != null)
            {
                ItemVest itemVest = uiItemData.GetComponent<ItemVest>();
                OnVestChanged?.Invoke(itemVest);
            }
            else
            {
                OnVestChanged?.Invoke(null); // No vest
            }
        }
        else
        {
            OnVestChanged?.Invoke(null);
        }
         ItemData backpackItemData = listItemDataInventoryEquipment.FirstOrDefault(item => item.itemtype == Itemtype.Backpack);
        if (backpackItemData != null)
        {
            UIItemData uiItemData = inventoryItemPresent.listUIItemPrefab
                .FirstOrDefault(uiItem => uiItem.idItem == backpackItemData.idItem);

            if (uiItemData != null)
            {
                ItemBackpack itemBackpack = uiItemData.GetComponent<ItemBackpack>();
                npcSelecting.countInventorySlot += itemBackpack.slotIncreasing;
                SlotHasincreased = itemBackpack.slotIncreasing;
                OnBackpackChanged?.Invoke(itemBackpack);
            }
            else
            {
                OnBackpackChanged?.Invoke(null); // No backpack
            }
        }
        else
        {
            OnBackpackChanged?.Invoke(null);
        }
    }
    public GameObject CreateUIItem(ItemData itemData, InventorySlots invenrotySlots)
    {
        // Get the UI prefab matching the item's ID
        GameObject itemUIPrefab = inventoryItemPresent.listUIItemPrefab
            .FirstOrDefault(idItem => idItem.idItem == itemData.idItem)?.gameObject;

        if (itemUIPrefab == null)
        {
            Debug.LogWarning($"UI prefab not found for item ID: {itemData.idItem}");
            return null;
        }

        // Instantiate the UI item as a child of the given slot
        GameObject itemUICreate = Instantiate(itemUIPrefab, invenrotySlots.transform, true);

        // Set up item class and data
        UIItemData uIItemData = itemUICreate.GetComponent<UIItemData>();
        ItemClass itemClass = itemUICreate.GetComponent<ItemClass>();

        if (itemClass != null)
        {
            itemClass.quantityItem = itemData.count;
            itemClass.maxCountItem = itemData.maxCount;
        }

        if (uIItemData != null)
        {
            uIItemData.slotTypeParent = invenrotySlots.slotTypeInventory;
            uIItemData.UpdateDataUI(itemClass);
        }

        // Return the created UI object
        return itemUICreate;
    }

    private void InstallNpcCostumeOnPlayer(GameObject playerObject, NpcClass npcClass)
    {
        // Get the NpcCoutume component from the player
        NpcCoutume playerCoutume = playerObject.GetComponent<NpcCoutume>();
        if (playerCoutume != null)
        {
            // Get the NpcManager instance
            if (npcManager == null)
            {
                npcManager = GameManager.Instance.npcManager;
            }

            // Retrieve the costume data based on the selected NPC
            HeadCoutume headCoutume = npcManager.listHeadCoutume.FirstOrDefault(coutume => coutume.idHead == npcClass.idHead);
            BodyCoutume bodyCoutume = npcManager.listBodyCoutume.FirstOrDefault(coutume => coutume.idBody == npcClass.idBody);
            FeedCoutume feedCoutume = npcManager.listFeedCoutume.FirstOrDefault(coutume => coutume.idFeed == npcClass.idFeed);

            // Apply the costume to the player's NpcCoutume
            playerCoutume.SetCostume(headCoutume, bodyCoutume, feedCoutume);
        }
        else
        {
            Debug.LogWarning("Player does not have a NpcCoutume component.");
        }
    }
    public void HighlightItemsInSlotsUI(Ammotype ammoType)
    {
        foreach (InventorySlots slot in listInventorySlotsUI)
        {
            UIItemData uiItemData = slot.GetComponentInChildren<UIItemData>();
            if (uiItemData != null)
            {
                // Retrieve the item data
                ItemData itemData = listItemDataInventorySlot.FirstOrDefault(item => item.idItem == uiItemData.idItem);

                if (itemData != null && itemData.itemtype == Itemtype.Ammo)
                {
                    // Get the ammo type for this item
                    if (inventoryItemPresent.ammoItemIdToAmmoType.TryGetValue(itemData.idItem, out Ammotype itemAmmoType))
                    {
                        if (itemAmmoType == ammoType)
                        {
                            // Highlight the item
                            Image itemImage = uiItemData.itemIconImage;
                            if (itemImage != null)
                            {
                                itemImage.color = Color.yellow; // Highlight color
                            }
                        }
                    }
                }
            }
        }
    }

    public void ResetHighlightInSlotsUI()
    {
        foreach (InventorySlots slot in listInventorySlotsUI)
        {
            UIItemData uiItemData = slot.GetComponentInChildren<UIItemData>();
            if (uiItemData != null)
            {
                Image itemImage = uiItemData.itemIconImage;
                if (itemImage != null)
                {
                    itemImage.color = Color.white; // Reset to original color
                }
            }
        }
    }
}
