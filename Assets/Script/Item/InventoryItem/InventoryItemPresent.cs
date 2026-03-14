using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPresent : MonoBehaviour
{
    public static InventoryItemPresent Instance = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public List<ItemData> listItemsDataBox = new List<ItemData>();
    public List<UIItemData> listUIItemPrefab;
    public List<InventorySlots> listInvenrotySlots = new List<InventorySlots>();
    public InventorySlots invenrotySlotSpecialMilitaryLock;
    public InventorySlots invenrotySlotSpecialScavengerLock;
    public Transform transformsBoxes;

    public Canvas canvas;

    private void Start()
    {
        canvas = FindAnyObjectByType<Canvas>();
    }

    public void RefreshUIBox()
    {
        ClearUIBoxes();
        CombineItemsNoSplit(listItemsDataBox);
        foreach (ItemData itemData in listItemsDataBox.OrderBy(item => item.idItem))
        {
            CreateUIItemInBoxes(itemData);
        }
        
    }
    private void CombineItemsNoSplit(List<ItemData> items)
    {
        Dictionary<string, ItemData> itemMap = new Dictionary<string, ItemData>();

        // Combine items by idItem
        foreach (var item in items)
        {
            if (itemMap.ContainsKey(item.idItem))
            {
                // Update the count in the existing item
                itemMap[item.idItem].count += item.count;
            }
            else
            {
                // Add a new item to the map (copying properties)
                itemMap[item.idItem] = new ItemData
                {
                    idItem = item.idItem,
                    nameItem = item.nameItem,
                    count = item.count,
                    maxCount = item.maxCount,
                    itemtype = item.itemtype,
                };
            }
        }

        // Update the original list in place
        items.Clear();
        items.AddRange(itemMap.Values);
    }

    public void CreateUIItemInBoxes(ItemData itemData)
    {
        UIItemData prefab = listUIItemPrefab.FirstOrDefault(idItem => idItem.idItem == itemData.idItem);
        if (prefab == null) return;
        GameObject uIItemOBJ = Instantiate(prefab.gameObject, transformsBoxes, false);

        UIItemData uIItemData = uIItemOBJ.GetComponent<UIItemData>();
        ItemClass itemClass = uIItemOBJ.GetComponent<ItemClass>();

        itemClass.quantityItem = itemData.count;
        itemClass.maxCountItem = itemData.maxCount;

        uIItemData.slotTypeParent = transformsBoxes.GetComponent<InventorySlots>().slotTypeInventory;
        uIItemData.UpdateDataUI(itemClass);
    }
    public void ClearUIBoxes()
    {
        foreach (Transform child in transformsBoxes)
        {
            Destroy(child.gameObject);
        }
    }
    public void UnlockSlotInventory(int numUnlock, SpecialistRoleNpc specialistRoleNpc, List<ItemData> listItemDataInventoryEqicment)
    {
        // Lock all slots initially
        foreach (InventorySlots slot in listInvenrotySlots)
        {
            slot.slotTypeInventory = SlotType.SlotLock;
        }

        // Unlock general inventory slots based on numUnlock
        int maxUnlock = Mathf.Min(numUnlock, listInvenrotySlots.Count);
        for (int i = 0; i < maxUnlock; i++)
        {
            listInvenrotySlots[i].slotTypeInventory = SlotType.SlotBag;
        }

        // Define the item IDs that unlock the special slots
        string militaryItemID = "1020605"; // Replace with your Military item ID
        string scavengerItemID = "1020604"; // Replace with your Scavenger item ID

        // Check if the items are equipped
        bool hasMilitaryItem = listItemDataInventoryEqicment.Any(item => item.idItem == militaryItemID);
        bool hasScavengerItem = listItemDataInventoryEqicment.Any(item => item.idItem == scavengerItemID);

        // Unlock or lock the special military slot
        if (invenrotySlotSpecialMilitaryLock != null)
        {
            invenrotySlotSpecialMilitaryLock.slotTypeInventory =
                (specialistRoleNpc == SpecialistRoleNpc.Military_training || hasMilitaryItem)
                    ? SlotType.SlotWeapon : SlotType.SlotLock;
        }

        // Unlock or lock the special scavenger slot
        if (invenrotySlotSpecialScavengerLock != null)
        {
            invenrotySlotSpecialScavengerLock.slotTypeInventory =
                (specialistRoleNpc == SpecialistRoleNpc.Scavenger || hasScavengerItem)
                    ? SlotType.SlotTool : SlotType.SlotLock;
        }
    }


    public void RefreshCarInventory()
    {
        // Ensure all car slots are cleared
        foreach (var slot in listInvenrotySlots.Where(s => s.slotTypeInventory == SlotType.SlotCar))
        {
            ClearSlot(slot);
        }

        // Populate car inventory slots
        foreach (var itemData in listItemsDataBox) // Assuming `listItemsDataBox` is the correct field
        {
            var carSlot = listInvenrotySlots.FirstOrDefault(s => s.slotTypeInventory == SlotType.SlotCar && s.transform.childCount == 0);
            if (carSlot != null)
            {
                CreateUIItemInSlot(itemData, carSlot);
            }
        }
    }

    // Clear all child elements from a slot
    private void ClearSlot(InventorySlots slot)
    {
        foreach (Transform child in slot.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Create a UI item in a specified slot
    private void CreateUIItemInSlot(ItemData itemData, InventorySlots slot)
    {
        var uiItemPrefab = listUIItemPrefab.FirstOrDefault(p => p.idItem == itemData.idItem)?.gameObject;
        if (uiItemPrefab != null)
        {
            var uiItem = Instantiate(uiItemPrefab, slot.transform);
            var itemClass = uiItem.GetComponent<ItemClass>();
            if (itemClass != null)
            {
                itemClass.quantityItem = itemData.count;
                itemClass.maxCountItem = itemData.maxCount;
            }

            var uiItemData = uiItem.GetComponent<UIItemData>();
            if (uiItemData != null)
            {
                uiItemData.slotTypeParent = slot.slotTypeInventory;
                uiItemData.UpdateDataUI(itemClass);
            }
        }
    }
    public void AddItemByID(string itemID, int count)
    {
        // Find the UIItemData associated with the given itemID
        UIItemData uiItemData = listUIItemPrefab.FirstOrDefault(item => item.idItem == itemID);
        if (uiItemData == null)
        {
            Debug.LogWarning($"No UIItemData found for itemID: {itemID}");
            return;
        }
        ItemClass itemClass = uiItemData.GetComponent<ItemClass>();

        // Construct a new ItemData object based on the UIItemData template
        ItemData newItemData = new ItemData
        {
            nameItem = uiItemData.nameItem,
            idItem = uiItemData.idItem,
            count = count,
            maxCount = itemClass.maxCountItem,
            itemtype = itemClass.itemtype,
        };

        // Use the existing AddItem method to handle addition logic
        AddItem(newItemData);
    }

    public void AddItem(ItemData itemDataAdd)
    {
        ItemData itemDataInList = this.listItemsDataBox.FirstOrDefault(item => item.idItem == itemDataAdd.idItem && item.count != item.maxCount);
        string[] excludedItemIds = { "1020129", "1020130", "1020128", "1020131", "1020132" };
        if (itemDataInList != null)
        {   
            if (excludedItemIds.Contains(itemDataAdd.idItem))
            {
                return;
            }
            else
                itemDataInList.count = itemDataInList.count + itemDataAdd.count;
        }
        else if(itemDataInList == null)
        {
            if (excludedItemIds.Contains(itemDataAdd.idItem))
            {
                return;
            }
            listItemsDataBox.Add(itemDataAdd);
        }
    }
    public void RemoveItem(ItemData itemDataRemove)
    {
        ItemData itemDataInList = listItemsDataBox.LastOrDefault(item => item.idItem == itemDataRemove.idItem);
        if (itemDataInList == null) return;

        if (itemDataInList.count - itemDataRemove.count >= 0)
        {
            itemDataInList.count -= itemDataRemove.count;
            if (itemDataInList.count == 0)
            {
                listItemsDataBox.Remove(itemDataInList);
            }
        }
    }

    public int GetItemCountByID(string itemID)
    {
        ItemData itemData = listItemsDataBox.Find(item => item.idItem == itemID);
        return itemData != null ? itemData.count : 0;
    }

    // Method to get item icon by ID
    public Sprite GetItemIconByID(string itemID)
    {
        UIItemData uiItemData = listUIItemPrefab.Find(uiItem => uiItem.idItem == itemID);
        if (uiItemData != null && uiItemData.itemIconImage != null)
        {
            return uiItemData.itemIconImage.sprite;
        }
        else
        {
            Debug.LogWarning($"Item icon not found for itemID: {itemID}");
            return null;
        }
    }
    public bool HasItem(string itemID)
    {
        return listItemsDataBox.Any(item => item.idItem == itemID);
    }

    public ItemData ConventItemClassToItemData(ItemClass itemClass)
    {
        ItemData newItemData = new ItemData();

        newItemData.nameItem = itemClass.nameItem;
        newItemData.idItem = itemClass.idItem;
        newItemData.count = itemClass.quantityItem;
        newItemData.maxCount = itemClass.maxCountItem;
        newItemData.itemtype = itemClass.itemtype;

        return newItemData;
    }
    public Dictionary<string, Ammotype> ammoItemIdToAmmoType = new Dictionary<string, Ammotype>
    {
        // Add mappings from ammo item IDs to their ammo types
        { "1020125", Ammotype.HighCaliber }, // Replace with actual ammo item IDs
        { "1020127", Ammotype.MediumCaliber },
        { "1020124", Ammotype.LowCaliber },
        { "1020126", Ammotype.Shotgun },
        // Continue for all ammo items
    };
    public void HighlightAmmoItems(Ammotype ammoType)
    {
        Debug.Log("HighlightItem");
        foreach (Transform child in transformsBoxes)
        {
            UIItemData uiItemData = child.GetComponent<UIItemData>();
            if (uiItemData != null)
            {
                ItemData itemData = listItemsDataBox.FirstOrDefault(item => item.idItem == uiItemData.idItem);

                if (itemData != null && itemData.itemtype == Itemtype.Ammo)
                {
                    // Get the ammo type for this item via the mapping
                    if (ammoItemIdToAmmoType.TryGetValue(itemData.idItem, out Ammotype itemAmmoType))
                    {
                        if (itemAmmoType == ammoType)
                        {
                            // Highlight the ammo item by changing its image color
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
    public void ResetAmmoHighlighting()
    {
        foreach (Transform child in transformsBoxes)
        {
            UIItemData uiItemData = child.GetComponent<UIItemData>();
            if (uiItemData != null)
            {
                Image itemImage = uiItemData.itemIconImage;
                if (itemImage != null)
                {
                    itemImage.color = Color.white; // Original color
                }
            }
        }
    }

    /// <summary>
    /// Shared helper: Add or update an item in a list, splitting into multiple entries if quantity exceeds maxCount.
    /// </summary>
    public static void AddOrUpdateItemInList(List<ItemData> list, ItemData sourceItem, int quantity)
    {
        var existingItem = list.FirstOrDefault(item => item.idItem == sourceItem.idItem);

        if (existingItem != null)
        {
            int totalQuantity = existingItem.count + quantity;

            if (totalQuantity <= existingItem.maxCount)
            {
                existingItem.count = totalQuantity;
            }
            else
            {
                existingItem.count = existingItem.maxCount;
                int excess = totalQuantity - existingItem.maxCount;

                while (excess > 0)
                {
                    int newSlotQuantity = Mathf.Min(excess, sourceItem.maxCount);
                    list.Add(new ItemData
                    {
                        idItem = sourceItem.idItem,
                        nameItem = sourceItem.nameItem,
                        count = newSlotQuantity,
                        maxCount = sourceItem.maxCount,
                        itemtype = sourceItem.itemtype,
                    });
                    excess -= newSlotQuantity;
                }
            }
        }
        else
        {
            while (quantity > 0)
            {
                int newSlotQuantity = Mathf.Min(quantity, sourceItem.maxCount);
                list.Add(new ItemData
                {
                    idItem = sourceItem.idItem,
                    nameItem = sourceItem.nameItem,
                    count = newSlotQuantity,
                    maxCount = sourceItem.maxCount,
                    itemtype = sourceItem.itemtype,
                });
                quantity -= newSlotQuantity;
            }
        }
    }

    /// <summary>
    /// Shared helper: Remove quantity of an item from a list.
    /// </summary>
    public static void RemoveItemFromList(List<ItemData> list, ItemData sourceItem, int quantity)
    {
        var originItem = list.FirstOrDefault(item => item.idItem == sourceItem.idItem && item.itemtype == sourceItem.itemtype);
        if (originItem != null)
        {
            originItem.count -= quantity;
            if (originItem.count <= 0)
            {
                list.Remove(originItem);
            }
        }
    }

    /// <summary>
    /// Shared helper: Resolve the item list for a given SlotType.
    /// </summary>
    public static List<ItemData> ResolveListForSlotType(SlotType slotType, UIInventory uIInventory)
    {
        TradesystemScript tradesystemScript = TradesystemScript.GetActiveTrade();

        switch (slotType)
        {
            case SlotType.SlotBag:
                return uIInventory.listItemDataInventorySlot;
            case SlotType.SlotCar:
                return ((UIInventoryEX)uIInventory).listItemDataCarInventorySlot;
            case SlotType.SlotBoxes:
                return uIInventory.inventoryItemPresent.listItemsDataBox;
            case SlotType.SlotWeapon:
            case SlotType.SlotVest:
            case SlotType.SlotTool:
            case SlotType.SlotBackpack:
            case SlotType.SlotGrenade:
                return uIInventory.listItemDataInventoryEquipment;
            case SlotType.SlotNpcItem:
                return tradesystemScript?.listInvenrotyNpcItem;
            case SlotType.SlotPlayerTrade:
                return tradesystemScript?.listPlayerItemWaitforTrade;
            case SlotType.SlotNpcTrade:
                return tradesystemScript?.listNpcItemWaitforTrade;
            default:
                return null;
        }
    }
}
