using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPresent : MonoBehaviour
{
    public static InventoryItemPresent Instance { get; private set; }

    [Header("Inventory Data")]
    public List<ItemData> listItemsDataBox = new List<ItemData>();
    public List<UIItemData> listUIItemPrefab = new List<UIItemData>();
    public List<InventorySlots> listInvenrotySlots = new List<InventorySlots>();
    public InventorySlots invenrotySlotSpecialMilitaryLock;
    public InventorySlots invenrotySlotSpecialScavengerLock;
    public Transform transformsBoxes;

    [Header("UI Toggle Settings")]
    public Canvas canvas;
    public GameObject targetObject;
    [SerializeField] private float toggleCooldown = 0.5f;
    private float nextToggleTime = 0f;

    [Header("Ammo Mapping")]
    public Dictionary<int, Ammotype> ammoItemIdToAmmoType = new Dictionary<int, Ammotype>
    {
        { 1020125, Ammotype.HighCaliber },
        { 1020127, Ammotype.MediumCaliber },
        { 1020124, Ammotype.LowCaliber },
        { 1020126, Ammotype.Shotgun }
    };

    // Cached lookups and reusable containers (Zero-GC)
    private readonly Dictionary<int, UIItemData> _prefabLookup = new Dictionary<int, UIItemData>();
    private readonly Dictionary<int, ItemData> _combineDictionary = new Dictionary<int, ItemData>();
    private static readonly HashSet<int> ExcludedItemIds = new HashSet<int>
    {
        1020129, 1020130, 1020128, 1020131, 1020132
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePrefabLookup();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && Time.time >= nextToggleTime)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(!targetObject.activeSelf);
                nextToggleTime = Time.time + toggleCooldown;
            }
        }
    }

    public void InitializePrefabLookup()
    {
        _prefabLookup.Clear();
        if (listUIItemPrefab == null) return;

        for (int i = 0; i < listUIItemPrefab.Count; i++)
        {
            UIItemData item = listUIItemPrefab[i];
            if (item != null && !_prefabLookup.ContainsKey(item.idItem))
            {
                _prefabLookup.Add(item.idItem, item);
            }
        }
    }

    public UIItemData GetUIItemPrefab(int idItem)
    {
        if (_prefabLookup.TryGetValue(idItem, out UIItemData prefab))
        {
            return prefab;
        }

        // Fallback scan if lookup was not initialized or updated dynamically
        if (listUIItemPrefab != null)
        {
            for (int i = 0; i < listUIItemPrefab.Count; i++)
            {
                if (listUIItemPrefab[i] != null && listUIItemPrefab[i].idItem == idItem)
                {
                    _prefabLookup[idItem] = listUIItemPrefab[i];
                    return listUIItemPrefab[i];
                }
            }
        }

        return null;
    }

    public void RefreshUIBox()
    {
        if (transformsBoxes == null) return;

        ClearUIBoxes();
        CombineItemsNoSplit(listItemsDataBox);

        listItemsDataBox.Sort((a, b) => a.idItem.CompareTo(b.idItem));

        for (int i = 0; i < listItemsDataBox.Count; i++)
        {
            CreateUIItemInBoxes(listItemsDataBox[i]);
        }
    }

    private void CombineItemsNoSplit(List<ItemData> items)
    {
        if (items == null) return;

        _combineDictionary.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            ItemData item = items[i];
            if (item == null) continue;

            if (_combineDictionary.TryGetValue(item.idItem, out ItemData existing))
            {
                existing.count += item.count;
            }
            else
            {
                _combineDictionary[item.idItem] = new ItemData
                {
                    idItem = item.idItem,
                    nameItem = item.nameItem,
                    count = item.count,
                    maxCount = item.maxCount,
                    itemtype = item.itemtype
                };
            }
        }

        items.Clear();
        foreach (var kvp in _combineDictionary)
        {
            items.Add(kvp.Value);
        }
    }

    public void CreateUIItemInBoxes(ItemData itemData)
    {
        if (itemData == null || transformsBoxes == null) return;

        UIItemData uiItemPrefab = GetUIItemPrefab(itemData.idItem);
        if (uiItemPrefab == null)
        {
            Debug.LogWarning($"UIItemData prefab not found for itemID: {itemData.idItem}");
            return;
        }

        GameObject uiItemObj = Instantiate(uiItemPrefab.gameObject, transformsBoxes, false);
        UIItemData uiItemData = uiItemObj.GetComponent<UIItemData>();
        ItemClass itemClass = uiItemObj.GetComponent<ItemClass>();

        if (itemClass != null)
        {
            itemClass.quantityItem = itemData.count;
            itemClass.maxCountItem = itemData.maxCount;
        }

        if (uiItemData != null)
        {
            InventorySlots parentSlot = transformsBoxes.GetComponent<InventorySlots>();
            if (parentSlot != null)
            {
                uiItemData.slotTypeParent = parentSlot.slotTypeInventory;
            }
            uiItemData.UpdateDataUI(itemClass);
        }
    }

    public void ClearUIBoxes()
    {
        if (transformsBoxes == null) return;

        for (int i = transformsBoxes.childCount - 1; i >= 0; i--)
        {
            Transform child = transformsBoxes.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void UnlockSlotInventory(int numUnlock, SpecialistRoleNpc specialistRoleNpc, List<ItemData> listItemDataInventoryEqicment)
    {
        if (listInvenrotySlots == null) return;

        // Lock all slots initially
        for (int i = 0; i < listInvenrotySlots.Count; i++)
        {
            if (listInvenrotySlots[i] != null)
            {
                listInvenrotySlots[i].slotTypeInventory = SlotType.SlotLock;
            }
        }

        // Unlock general inventory slots based on numUnlock
        int unlockLimit = Mathf.Min(numUnlock, listInvenrotySlots.Count);
        for (int i = 0; i < unlockLimit; i++)
        {
            if (listInvenrotySlots[i] != null)
            {
                listInvenrotySlots[i].slotTypeInventory = SlotType.SlotBag;
            }
        }

        // Define the item IDs that unlock the special slots
        const int militaryItemID = 1020605;
        const int scavengerItemID = 1020604;

        bool hasMilitaryItem = false;
        bool hasScavengerItem = false;

        if (listItemDataInventoryEqicment != null)
        {
            for (int i = 0; i < listItemDataInventoryEqicment.Count; i++)
            {
                ItemData item = listItemDataInventoryEqicment[i];
                if (item != null)
                {
                    if (item.idItem == militaryItemID) hasMilitaryItem = true;
                    if (item.idItem == scavengerItemID) hasScavengerItem = true;
                }
            }
        }

        // Unlock or lock the special military slot
        if (invenrotySlotSpecialMilitaryLock != null)
        {
            invenrotySlotSpecialMilitaryLock.slotTypeInventory =
                (specialistRoleNpc == SpecialistRoleNpc.Military_training || hasMilitaryItem)
                    ? SlotType.SlotWeapon
                    : SlotType.SlotLock;
        }

        // Unlock or lock the special scavenger slot
        if (invenrotySlotSpecialScavengerLock != null)
        {
            invenrotySlotSpecialScavengerLock.slotTypeInventory =
                (specialistRoleNpc == SpecialistRoleNpc.Scavenger || hasScavengerItem)
                    ? SlotType.SlotTool
                    : SlotType.SlotLock;
        }
    }

    public void RefreshCarInventory()
    {
        if (listInvenrotySlots == null || listItemsDataBox == null) return;

        // Ensure all car slots are cleared
        for (int i = 0; i < listInvenrotySlots.Count; i++)
        {
            InventorySlots slot = listInvenrotySlots[i];
            if (slot != null && slot.slotTypeInventory == SlotType.SlotCar)
            {
                ClearSlot(slot);
            }
        }

        // Populate car inventory slots
        for (int i = 0; i < listItemsDataBox.Count; i++)
        {
            ItemData itemData = listItemsDataBox[i];
            InventorySlots emptyCarSlot = null;

            for (int s = 0; s < listInvenrotySlots.Count; s++)
            {
                InventorySlots slot = listInvenrotySlots[s];
                if (slot != null && slot.slotTypeInventory == SlotType.SlotCar && slot.transform.childCount == 0)
                {
                    emptyCarSlot = slot;
                    break;
                }
            }

            if (emptyCarSlot != null)
            {
                CreateUIItemInSlot(itemData, emptyCarSlot);
            }
        }
    }

    private void ClearSlot(InventorySlots slot)
    {
        if (slot == null) return;

        for (int i = slot.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = slot.transform.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateUIItemInSlot(ItemData itemData, InventorySlots slot)
    {
        if (itemData == null || slot == null) return;

        UIItemData uiPrefab = GetUIItemPrefab(itemData.idItem);
        if (uiPrefab == null) return;

        GameObject uiItem = Instantiate(uiPrefab.gameObject, slot.transform);
        ItemClass itemClass = uiItem.GetComponent<ItemClass>();
        if (itemClass != null)
        {
            itemClass.quantityItem = itemData.count;
            itemClass.maxCountItem = itemData.maxCount;
        }

        UIItemData uiItemData = uiItem.GetComponent<UIItemData>();
        if (uiItemData != null)
        {
            uiItemData.slotTypeParent = slot.slotTypeInventory;
            uiItemData.UpdateDataUI(itemClass);
        }
    }

    public void AddItemByID(int itemID, int count) 
    {
        UIItemData uiItemData = GetUIItemPrefab(itemID);
        if (uiItemData == null)
        {
            Debug.LogWarning($"No UIItemData found for itemID: {itemID}");
            return;
        }

        ItemClass itemClass = uiItemData.GetComponent<ItemClass>();
        if (itemClass == null)
        {
            Debug.LogWarning($"No ItemClass component found on prefab for itemID: {itemID}");
            return;
        }

        ItemData newItemData = new ItemData
        {
            nameItem = uiItemData.nameItem,
            idItem = uiItemData.idItem,
            count = count,
            maxCount = itemClass.maxCountItem,
            itemtype = itemClass.itemtype
        };

        AddItem(newItemData);
    }

    public void AddItem(ItemData itemDataAdd)
    {
        if (itemDataAdd == null) return;
        if (ExcludedItemIds.Contains(itemDataAdd.idItem)) return;

        ItemData itemDataInList = null;
        for (int i = 0; i < listItemsDataBox.Count; i++)
        {
            ItemData item = listItemsDataBox[i];
            if (item != null && item.idItem == itemDataAdd.idItem && item.count != item.maxCount)
            {
                itemDataInList = item;
                break;
            }
        }

        if (itemDataInList != null)
        {
            itemDataInList.count += itemDataAdd.count;
        }
        else
        {
            listItemsDataBox.Add(itemDataAdd);
        }
    }

    public void RemoveItem(ItemData itemDataRemove)
    {
        if (itemDataRemove == null || listItemsDataBox == null) return;

        ItemData itemDataInList = null;
        for (int i = listItemsDataBox.Count - 1; i >= 0; i--)
        {
            if (listItemsDataBox[i] != null && listItemsDataBox[i].idItem == itemDataRemove.idItem)
            {
                itemDataInList = listItemsDataBox[i];
                break;
            }
        }

        if (itemDataInList == null)
        {
            Debug.LogWarning($"Item to remove not found in box: {itemDataRemove.idItem}");
            return;
        }

        if (itemDataInList.count >= itemDataRemove.count)
        {
            itemDataInList.count -= itemDataRemove.count;
            if (itemDataInList.count <= 0)
            {
                listItemsDataBox.Remove(itemDataInList);
            }
        }
        else
        {
            Debug.LogWarning($"Insufficient item quantity in box to remove. Has {itemDataInList.count}, required {itemDataRemove.count}");
        }
    }

    public int GetItemCountByID(int itemID)
    {
        if (listItemsDataBox == null) return 0;

        int totalCount = 0;
        for (int i = 0; i < listItemsDataBox.Count; i++)
        {
            ItemData item = listItemsDataBox[i];
            if (item != null && item.idItem == itemID)
            {
                totalCount += item.count;
            }
        }
        return totalCount;
    }

    public Sprite GetItemIconByID(int itemID)
    {
        UIItemData uiItemData = GetUIItemPrefab(itemID);
        if (uiItemData != null && uiItemData.itemIconImage != null)
        {
            return uiItemData.itemIconImage.sprite;
        }

        Debug.LogWarning($"Item icon not found for itemID: {itemID}");
        return null;
    }

    public bool HasItem(int itemID)
    {
        if (listItemsDataBox == null) return false;

        for (int i = 0; i < listItemsDataBox.Count; i++)
        {
            if (listItemsDataBox[i] != null && listItemsDataBox[i].idItem == itemID && listItemsDataBox[i].count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public ItemData ConventItemClassToItemData(ItemClass itemClass)
    {
        if (itemClass == null) return null;
        return itemClass.ToItemData();
    }

    public void HighlightAmmoItems(Ammotype ammoType)
    {
        if (transformsBoxes == null) return;

        for (int i = 0; i < transformsBoxes.childCount; i++)
        {
            Transform child = transformsBoxes.GetChild(i);
            if (child == null) continue;

            UIItemData uiItemData = child.GetComponent<UIItemData>();
            if (uiItemData == null) continue;

            if (ammoItemIdToAmmoType.TryGetValue(uiItemData.idItem, out Ammotype itemAmmoType))
            {
                if (itemAmmoType == ammoType && uiItemData.itemIconImage != null)
                {
                    uiItemData.itemIconImage.color = Color.yellow;
                }
            }
        }
    }

    public void ResetAmmoHighlighting()
    {
        if (transformsBoxes == null) return;

        for (int i = 0; i < transformsBoxes.childCount; i++)
        {
            Transform child = transformsBoxes.GetChild(i);
            if (child == null) continue;

            UIItemData uiItemData = child.GetComponent<UIItemData>();
            if (uiItemData != null && uiItemData.itemIconImage != null)
            {
                uiItemData.itemIconImage.color = Color.white;
            }
        }
    }
}
