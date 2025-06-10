using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPresent : MonoBehaviour
{
    public static InventoryItemPresent Instance = new InventoryItemPresent();
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
    public GameObject targetObject; // Drag and drop the GameObject to toggle
    private float toggleCooldown = 0.5f; // Set cooldown interval
    private float nextToggleTime = 0f;

    private void Start()
    {
        canvas = FindAnyObjectByType<Canvas>();

    }
    private void Update()
    {
        // ตรวจสอบว่าปุ่ม I ถูกกดและว่า cooldown หมดลงแล้ว
        if (Input.GetKeyDown(KeyCode.I) && Time.time >= nextToggleTime)
        {
            // Toggle เปิด-ปิด GameObject
            targetObject.SetActive(!targetObject.activeSelf);

            // ตั้งเวลา cooldown สำหรับการกดครั้งถัดไป
            nextToggleTime = Time.time + toggleCooldown;
        }
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
        Dictionary<int, ItemData> itemMap = new Dictionary<int, ItemData>();

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

        GameObject uiItem = listUIItemPrefab.FirstOrDefault(idItem => idItem.idItem == itemData.idItem).gameObject;
        GameObject uIItemOBJ = Instantiate(uiItem, transformsBoxes, false);

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
        for (int i = 1; i <= numUnlock; i++)
        {
            InventorySlots slot = listInvenrotySlots.ElementAt(i - 1);
            slot.slotTypeInventory = SlotType.SlotBag;
        }

        // Define the item IDs that unlock the special slots
        int militaryItemID = 1020605; // Replace with your Military item ID
        int scavengerItemID = 1020604; // Replace with your Scavenger item ID

        // Check if the items are equipped
        bool hasMilitaryItem = listItemDataInventoryEqicment.Any(item => item.idItem == militaryItemID);
        bool hasScavengerItem = listItemDataInventoryEqicment.Any(item => item.idItem == scavengerItemID);

        // Unlock or lock the special military slot
        if (specialistRoleNpc == SpecialistRoleNpc.Military_training || hasMilitaryItem)
        {
            invenrotySlotSpecialMilitaryLock.slotTypeInventory = SlotType.SlotWeapon;
        }
        else
        {
            invenrotySlotSpecialMilitaryLock.slotTypeInventory = SlotType.SlotLock;
        }

        // Unlock or lock the special scavenger slot
        if (specialistRoleNpc == SpecialistRoleNpc.Scavenger || hasScavengerItem)
        {
            invenrotySlotSpecialScavengerLock.slotTypeInventory = SlotType.SlotTool;
        }
        else
        {
            invenrotySlotSpecialScavengerLock.slotTypeInventory = SlotType.SlotLock;
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
    public void AddItemByID(int itemID, int count) 
    {
        // Find the UIItemData associated with the given itemID
        UIItemData uiItemData = listUIItemPrefab.FirstOrDefault(item => item.idItem == itemID);
        ItemClass itemClass = uiItemData.GetComponent<ItemClass>();
        if (uiItemData == null)
        {
            Debug.LogWarning($"No UIItemData found for itemID: {itemID}");
            return;
        }

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
        int[] excludedItemIds = { 1020129, 1020130, 1020128, 1020131, 1020132 };
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

        if (itemDataInList.count - itemDataRemove.count >= 0)
        {
            itemDataInList.count -= itemDataRemove.count;
            if (itemDataInList.count == 0)
            {
                listItemsDataBox.Remove(itemDataInList);
            }
        }
        else
        {
            //ถ้าไปเท็มในกล่องไม่พอให้ทำอะไร
        }
        // RefreshUIBox();
    }

    public int GetItemCountByID(int itemID)
    {
        ItemData itemData = listItemsDataBox.Find(item => item.idItem == itemID);
        return itemData != null ? itemData.count : 0;
    }

    // Method to get item icon by ID
    public Sprite GetItemIconByID(int itemID)
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
    public bool HasItem(int itemID)
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
    public Dictionary<int, Ammotype> ammoItemIdToAmmoType = new Dictionary<int, Ammotype>
    {
        // Add mappings from ammo item IDs to their ammo types
        { 1020125, Ammotype.HighCaliber }, // Replace with actual ammo item IDs
        { 1020127, Ammotype.MediumCaliber },
        { 1020124, Ammotype.LowCaliber },
        { 1020126, Ammotype.Shotgun },
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
}
