using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [Header("UI Dropdown & NPC Display")]
    public TMP_Dropdown dropdown;
    public Image spriteHeadNpc;
    public NpcManager npcManager;
    public NpcClass npcSelecting;
    public List<InventorySlots> listInventorySlotsUI = new List<InventorySlots>();

    // Equipment Events
    public event Action<List<ItemWeapon>> OnWeaponsChanged;
    public event Action<ItemVest> OnVestChanged;
    public event Action<ItemBackpack> OnBackpackChanged;

    [Header("Inventory Data")]
    public List<ItemData> listItemDataInventoryEquipment = new List<ItemData>();
    public List<ItemData> listItemDataInventorySlot = new List<ItemData>();
    public Transform transformBoxes;
    public InventoryItemPresent inventoryItemPresent;

    [Header("TextMeshProUGUI")]
    public TextMeshProUGUI levelEnduranceText;
    public TextMeshProUGUI levelCombatText;
    public TextMeshProUGUI levelSpeedText;
    public TextMeshProUGUI specialistNpcText;
    public TextMeshProUGUI nameNpcText;

    [Header("State Tracking")]
    public int SlotHasincreased;
    public int currentNumCategory;
    public LootingSystem currentLootingSystem;
    public TradesystemScript currentTradesystem;
    public StatAmplifier statAmplifier;

    // Zero-GC reusable buffers
    private readonly Dictionary<int, int> _itemCountMap = new Dictionary<int, int>();
    private readonly List<ItemData> _updatedItems = new List<ItemData>();

    public void RemoveItemData(ItemClass itemClass, SlotType slotType)
    {
        if (itemClass == null) return;

        List<ItemData> targetList = (slotType == SlotType.SlotBag || slotType == SlotType.SlotBoxes)
            ? listItemDataInventorySlot
            : listItemDataInventoryEquipment;

        if (targetList == null) return;

        for (int i = 0; i < targetList.Count; i++)
        {
            ItemData itemData = targetList[i];
            if (itemData != null && itemData.idItem == itemClass.idItem)
            {
                itemData.count--;
                if (itemData.count <= 0)
                {
                    targetList.RemoveAt(i);
                }
                break;
            }
        }
    }

    public void AddItemlistInvenrotySlots(ItemClass itemClass)
    {
        if (itemClass == null || npcSelecting == null) return;

        ItemData itemData = null;
        for (int i = 0; i < listItemDataInventorySlot.Count; i++)
        {
            if (listItemDataInventorySlot[i] != null && listItemDataInventorySlot[i].idItem == itemClass.idItem)
            {
                itemData = listItemDataInventorySlot[i];
                break;
            }
        }

        if (listItemDataInventorySlot.Count < npcSelecting.countInventorySlot)
        {
            if (itemData != null)
            {
                int totalQuantity = itemData.count + itemClass.quantityItem;
                int maxCount = itemData.maxCount > 0 ? itemData.maxCount : 1;

                if (totalQuantity <= maxCount)
                {
                    itemData.count = totalQuantity;
                }
                else
                {
                    itemData.count = maxCount;
                    int excess = totalQuantity - maxCount;

                    while (excess > 0)
                    {
                        int newSlotQuantity = Math.Min(excess, maxCount);
                        ItemData newItemData = new ItemData
                        {
                            idItem = itemClass.idItem,
                            nameItem = itemClass.nameItem,
                            count = newSlotQuantity,
                            maxCount = maxCount,
                            itemtype = itemData.itemtype
                        };

                        listItemDataInventorySlot.Add(newItemData);
                        excess -= newSlotQuantity;
                    }
                }
            }
            else
            {
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
        ClearAllChildInvenrotySlot();

        if (listInventorySlotsUI == null || listItemDataInventorySlot == null) return;

        for (int i = 0; i < listInventorySlotsUI.Count; i++)
        {
            if (i < listItemDataInventorySlot.Count && listInventorySlotsUI[i] != null)
            {
                CreateUIItem(listItemDataInventorySlot[i], listInventorySlotsUI[i]);
            }
        }
    }

    public void SetCostumeNpcExpentdition(NpcClass npcClass, GameObject npcOBJ)
    {
        if (npcClass == null || npcOBJ == null) return;

        if (npcManager == null && GameManager.Instance != null)
        {
            npcManager = GameManager.Instance.npcManager;
        }
        if (npcManager == null) return;

        HeadCoutume headCoutume = null;
        if (npcManager.listHeadCoutume != null)
        {
            for (int i = 0; i < npcManager.listHeadCoutume.Count; i++)
            {
                if (npcManager.listHeadCoutume[i] != null && npcManager.listHeadCoutume[i].idHead == npcClass.idHead)
                {
                    headCoutume = npcManager.listHeadCoutume[i];
                    break;
                }
            }
        }

        BodyCoutume bodyCoutume = null;
        if (npcManager.listBodyCoutume != null)
        {
            for (int i = 0; i < npcManager.listBodyCoutume.Count; i++)
            {
                if (npcManager.listBodyCoutume[i] != null && npcManager.listBodyCoutume[i].idBody == npcClass.idBody)
                {
                    bodyCoutume = npcManager.listBodyCoutume[i];
                    break;
                }
            }
        }

        FeedCoutume feedCoutume = null;
        if (npcManager.listFeedCoutume != null)
        {
            for (int i = 0; i < npcManager.listFeedCoutume.Count; i++)
            {
                if (npcManager.listFeedCoutume[i] != null && npcManager.listFeedCoutume[i].idFeed == npcClass.idFeed)
                {
                    feedCoutume = npcManager.listFeedCoutume[i];
                    break;
                }
            }
        }

        NpcCoutume npcCoutume = npcOBJ.GetComponent<NpcCoutume>();
        if (npcCoutume != null)
        {
            npcCoutume.SetCostume(headCoutume, bodyCoutume, feedCoutume);
        }
    }

    public void SetValuableUIInventory()
    {
        if (GameManager.Instance != null)
        {
            inventoryItemPresent = GameManager.Instance.inventoryItemPresent;
            npcManager = GameManager.Instance.npcManager;
        }

        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        }

        if (inventoryItemPresent != null)
        {
            inventoryItemPresent.targetObject = gameObject;
        }

        if (npcManager != null)
        {
            npcManager.uIInventory = this;
            npcManager.levelCombatText = levelCombatText;
            npcManager.levelEnduranceText = levelEnduranceText;
            npcManager.levelSpeedText = levelSpeedText;
            npcManager.specialistNpcText = specialistNpcText;

            SetSlotToInventory();

            if (dropdown != null)
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(npcManager.SetNpcOptionDropDown());
                dropdown.onValueChanged.RemoveListener(npcManager.OnDropdownValueChanged);
                dropdown.onValueChanged.AddListener(npcManager.OnDropdownValueChanged);
            }

            npcManager.OnDropdownValueChanged(0);
        }

        if (inventoryItemPresent != null)
        {
            inventoryItemPresent.RefreshUIBox();
        }
    }

    public void SetSlotToInventory()
    {
        if (inventoryItemPresent == null || listInventorySlotsUI == null || listInventorySlotsUI.Count == 0) return;

        inventoryItemPresent.listInvenrotySlots.Clear();

        if (listInventorySlotsUI.Count > 13)
        {
            inventoryItemPresent.invenrotySlotSpecialMilitaryLock = listInventorySlotsUI[13];
        }
        if (listInventorySlotsUI.Count > 16)
        {
            inventoryItemPresent.invenrotySlotSpecialScavengerLock = listInventorySlotsUI[16];
        }

        int maxGeneralSlots = Mathf.Min(12, listInventorySlotsUI.Count);
        for (int i = 0; i < maxGeneralSlots; i++)
        {
            inventoryItemPresent.listInvenrotySlots.Add(listInventorySlotsUI[i]);
        }

        if (transformBoxes != null)
        {
            inventoryItemPresent.transformsBoxes = transformBoxes;
        }
    }

    public void RefreshUIBoxCategory(int numCategory)
    {    
        currentNumCategory = numCategory;
        if (inventoryItemPresent == null) return;

        if (numCategory == -1)
        {
            inventoryItemPresent.RefreshUIBox();
            return;
        }

        inventoryItemPresent.ClearUIBoxes();
        Itemtype itemtypeCategory = (Itemtype)numCategory;

        for (int i = 0; i < inventoryItemPresent.listItemsDataBox.Count; i++)
        {
            ItemData itemData = inventoryItemPresent.listItemsDataBox[i];
            if (itemData != null && itemData.itemtype == itemtypeCategory)
            {
                inventoryItemPresent.CreateUIItemInBoxes(itemData);
            }
        }
    }

    public virtual void RefreshUIInventory()
    {   
        ClearAllChildInvenrotySlot();

        CombineAndSplitItems(listItemDataInventorySlot);
        CombineAndSplitItems(listItemDataInventoryEquipment);

        if (npcSelecting != null)
        {
            npcSelecting.countInventorySlot = 6;
        }

        for (int i = 12; i < listInventorySlotsUI.Count; i++)
        {
            InventorySlots eqSlot = listInventorySlotsUI[i];
            if (eqSlot == null) continue;

            SlotType slotType = eqSlot.slotTypeInventory;

            if (slotType == SlotType.SlotWeapon || slotType == SlotType.SlotTool)
            {
                List<ItemData> matchingItems = new List<ItemData>();
                for (int m = 0; m < listItemDataInventoryEquipment.Count; m++)
                {
                    ItemData itm = listItemDataInventoryEquipment[m];
                    if (itm != null && GetSlotTypeForItemType(itm.itemtype) == slotType)
                    {
                        matchingItems.Add(itm);
                    }
                }

                if (slotType == SlotType.SlotWeapon)
                {
                    if (matchingItems.Count > 0)
                    {
                        if (i == 12 && matchingItems.Count >= 1)
                        {
                            CreateUIItem(matchingItems[0], eqSlot);
                        }
                        else if (i == 13 && matchingItems.Count >= 2)
                        {
                            CreateUIItem(matchingItems[1], eqSlot);
                        }
                    }
                }
                else if (slotType == SlotType.SlotTool)
                {
                    if (matchingItems.Count > 0)
                    {
                        if (i == 15 && matchingItems.Count >= 1)
                        {
                            CreateUIItem(matchingItems[0], eqSlot);
                        }
                        else if (i == 16 && matchingItems.Count >= 2)
                        {
                            CreateUIItem(matchingItems[1], eqSlot);
                        }
                    }
                }
            }
            else
            {
                ItemData eqItem = null;
                for (int e = 0; e < listItemDataInventoryEquipment.Count; e++)
                {
                    ItemData itm = listItemDataInventoryEquipment[e];
                    if (itm != null && GetSlotTypeForItemType(itm.itemtype) == slotType)
                    {
                        eqItem = itm;
                        break;
                    }
                }

                if (eqItem != null)
                {
                    CreateUIItem(eqItem, eqSlot);

                    if (eqItem.itemtype == Itemtype.Backpack && inventoryItemPresent != null && npcSelecting != null)
                    {
                        UIItemData uiPrefab = inventoryItemPresent.GetUIItemPrefab(eqItem.idItem);
                        if (uiPrefab != null)
                        {
                            ItemBackpack backpack = uiPrefab.GetComponent<ItemBackpack>();
                            if (backpack != null)
                            {
                                npcSelecting.countInventorySlot += backpack.slotIncreasing;
                                SlotHasincreased = backpack.slotIncreasing;
                            }
                        }
                    }
                }
            }
        }

        if (npcSelecting != null && listItemDataInventorySlot.Count > npcSelecting.countInventorySlot)
        {
            for (int i = listItemDataInventorySlot.Count - 1; i >= npcSelecting.countInventorySlot; i--)
            {
                listItemDataInventorySlot.RemoveAt(i);
            }
        }

        if (inventoryItemPresent != null && npcSelecting != null)
        {
            inventoryItemPresent.UnlockSlotInventory(npcSelecting.countInventorySlot, npcSelecting.roleNpc, listItemDataInventoryEquipment);
        }

        int maxBagSlots = 12;
        for (int i = 0; i < maxBagSlots; i++)
        {
            if (i < listItemDataInventorySlot.Count && i < listInventorySlotsUI.Count)
            {
                CreateUIItem(listItemDataInventorySlot[i], listInventorySlotsUI[i]);
            }
        }

        if (this is UIInventoryEX exUI && exUI.listItemDataCarInventorySlot != null && exUI.listInvenrotyCarSlotsUI != null)
        {
            exUI.BindCarSlotsToData();
        }
    }

    public void CombineAndSplitItems(List<ItemData> items)
    {
        if (items == null || items.Count == 0) return;

        _itemCountMap.Clear();
        _updatedItems.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            ItemData item = items[i];
            if (item == null) continue;

            if (_itemCountMap.TryGetValue(item.idItem, out int count))
            {
                _itemCountMap[item.idItem] = count + item.count;
            }
            else
            {
                _itemCountMap[item.idItem] = item.count;
            }
        }

        foreach (var kvp in _itemCountMap)
        {
            int itemId = kvp.Key;
            int totalQuantity = kvp.Value;

            ItemData templateItem = null;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].idItem == itemId)
                {
                    templateItem = items[i];
                    break;
                }
            }

            if (templateItem == null) continue;
            int maxCount = templateItem.maxCount > 0 ? templateItem.maxCount : 1;

            while (totalQuantity > 0)
            {
                int splitCount = Mathf.Min(totalQuantity, maxCount);
                _updatedItems.Add(new ItemData
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

        items.Clear();
        items.AddRange(_updatedItems);
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
            default:
                return SlotType.SlotLock;
        }
    }

    public void SelectNpcDefenseScene()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        statAmplifier = FindFirstObjectByType<StatAmplifier>();
        StatManager statManager = FindFirstObjectByType<StatManager>();

        if (statAmplifier != null && npcSelecting != null)
        {
            statAmplifier.SetStatAmplifer(npcSelecting);
            statAmplifier.InitializeAmplifiers();
        }

        if (statManager != null)
        {
            statManager.OnStatAmplifierChanged();
            if (player != null)
            {
                player.currentStamina = statManager.maxStamina;
            }
        }

        if (player != null)
        {
            Weapon weapon = player.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapon.OnStatsChanged();
            }

            if (npcSelecting != null)
            {
                npcSelecting.isWorking = true;
                SetCostumeNpcExpentdition(npcSelecting, player.gameObject);
            }
        }
    }

    public void ClearItemDataInAllInventorySlotToListDataBoxes()
    {
        if (listInventorySlotsUI == null) return;

        InventoryItemPresent presenter = (inventoryItemPresent != null) 
            ? inventoryItemPresent 
            : InventoryItemPresent.Instance;

        for (int i = 0; i < listInventorySlotsUI.Count; i++)
        {
            InventorySlots slotsItem = listInventorySlotsUI[i];
            if (slotsItem == null) continue;

            ItemClass itemClass = slotsItem.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                ItemData itemData = itemClass.ToItemData();

                if (BuildManager.Instance != null)
                {
                    BuildManager.Instance.AddResource(itemData.idItem, itemData.count);
                }

                if (presenter != null)
                {
                    presenter.AddItem(itemData);
                }

                Destroy(itemClass.gameObject);
            }
        }
    }

    public void ClearAllChildInvenrotySlot()
    {
        if (listInventorySlotsUI == null) return;

        for (int i = 0; i < listInventorySlotsUI.Count; i++)
        {
            InventorySlots slotsItem = listInventorySlotsUI[i];
            if (slotsItem == null) continue;

            Transform slotTransform = slotsItem.transform;
            for (int c = slotTransform.childCount - 1; c >= 0; c--)
            {
                Transform child = slotTransform.GetChild(c);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public virtual void ConventAllUIItemInListInventorySlotToListItemData(List<ItemData> listSlotItemDatas)
    {
        if (listSlotItemDatas == null || listInventorySlotsUI == null) return;

        int limit = Mathf.Min(12, listInventorySlotsUI.Count);
        for (int i = 0; i < limit; i++)
        {
            InventorySlots slot = listInventorySlotsUI[i];
            if (slot == null) continue;

            ItemClass itemClass = slot.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                listSlotItemDatas.Add(itemClass.ToItemData());
            }
        }
    }

    public virtual void ConventAllUIItemInListInventorySlotToListEquipmentItemData(List<ItemData> listEqicmentItemDatas)
    {
        if (listEqicmentItemDatas == null || listInventorySlotsUI == null) return;

        int limit = Mathf.Min(19, listInventorySlotsUI.Count);
        for (int i = 12; i < limit; i++)
        {
            InventorySlots slot = listInventorySlotsUI[i];
            if (slot == null) continue;

            ItemClass itemClass = slot.GetComponentInChildren<ItemClass>();
            if (itemClass != null)
            {
                listEqicmentItemDatas.Add(itemClass.ToItemData());
            }
        }
    }

    public virtual void ConventDataUIToItemData()
    {
        listItemDataInventorySlot.Clear();
        listItemDataInventoryEquipment.Clear();
        ConventAllUIItemInListInventorySlotToListItemData(listItemDataInventorySlot);
        ConventAllUIItemInListInventorySlotToListEquipmentItemData(listItemDataInventoryEquipment);

        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        }

        // Weapon logic
        List<ItemWeapon> itemWeapons = null;
        for (int i = 0; i < listItemDataInventoryEquipment.Count; i++)
        {
            ItemData eqItem = listItemDataInventoryEquipment[i];
            if (eqItem != null && eqItem.itemtype == Itemtype.Weapon && inventoryItemPresent != null)
            {
                UIItemData uiPrefab = inventoryItemPresent.GetUIItemPrefab(eqItem.idItem);
                if (uiPrefab != null)
                {
                    ItemWeapon weapon = uiPrefab.GetComponent<ItemWeapon>();
                    if (weapon != null)
                    {
                        if (itemWeapons == null) itemWeapons = new List<ItemWeapon>();
                        itemWeapons.Add(weapon);
                    }
                }
            }
        }
        OnWeaponsChanged?.Invoke(itemWeapons);

        // Vest logic
        ItemVest equippedVest = null;
        for (int i = 0; i < listItemDataInventoryEquipment.Count; i++)
        {
            ItemData eqItem = listItemDataInventoryEquipment[i];
            if (eqItem != null && eqItem.itemtype == Itemtype.Vest && inventoryItemPresent != null)
            {
                UIItemData uiPrefab = inventoryItemPresent.GetUIItemPrefab(eqItem.idItem);
                if (uiPrefab != null)
                {
                    equippedVest = uiPrefab.GetComponent<ItemVest>();
                    break;
                }
            }
        }
        OnVestChanged?.Invoke(equippedVest);

        // Backpack logic
        ItemBackpack equippedBackpack = null;
        for (int i = 0; i < listItemDataInventoryEquipment.Count; i++)
        {
            ItemData eqItem = listItemDataInventoryEquipment[i];
            if (eqItem != null && eqItem.itemtype == Itemtype.Backpack && inventoryItemPresent != null)
            {
                UIItemData uiPrefab = inventoryItemPresent.GetUIItemPrefab(eqItem.idItem);
                if (uiPrefab != null)
                {
                    equippedBackpack = uiPrefab.GetComponent<ItemBackpack>();
                    if (equippedBackpack != null && npcSelecting != null)
                    {
                        npcSelecting.countInventorySlot += equippedBackpack.slotIncreasing;
                        SlotHasincreased = equippedBackpack.slotIncreasing;
                    }
                    break;
                }
            }
        }
        OnBackpackChanged?.Invoke(equippedBackpack);
    }

    public GameObject CreateUIItem(ItemData itemData, InventorySlots invenrotySlots)
    {
        if (itemData == null || invenrotySlots == null) return null;

        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        }
        if (inventoryItemPresent == null) return null;

        UIItemData itemUIPrefab = inventoryItemPresent.GetUIItemPrefab(itemData.idItem);
        if (itemUIPrefab == null)
        {
            Debug.LogWarning($"UI prefab not found for item ID: {itemData.idItem}");
            return null;
        }

        GameObject itemUICreate = Instantiate(itemUIPrefab.gameObject, invenrotySlots.transform, false);

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

        return itemUICreate;
    }

    public void HighlightItemsInSlotsUI(Ammotype ammoType)
    {
        if (listInventorySlotsUI == null || inventoryItemPresent == null) return;

        for (int i = 0; i < listInventorySlotsUI.Count; i++)
        {
            InventorySlots slot = listInventorySlotsUI[i];
            if (slot == null) continue;

            UIItemData uiItemData = slot.GetComponentInChildren<UIItemData>();
            if (uiItemData == null) continue;

            for (int d = 0; d < listItemDataInventorySlot.Count; d++)
            {
                ItemData itemData = listItemDataInventorySlot[d];
                if (itemData != null && itemData.idItem == uiItemData.idItem && itemData.itemtype == Itemtype.Ammo)
                {
                    if (inventoryItemPresent.ammoItemIdToAmmoType.TryGetValue(itemData.idItem, out Ammotype itemAmmoType))
                    {
                        if (itemAmmoType == ammoType && uiItemData.itemIconImage != null)
                        {
                            uiItemData.itemIconImage.color = Color.yellow;
                        }
                    }
                    break;
                }
            }
        }
    }

    public void ResetHighlightInSlotsUI()
    {
        if (listInventorySlotsUI == null) return;

        for (int i = 0; i < listInventorySlotsUI.Count; i++)
        {
            InventorySlots slot = listInventorySlotsUI[i];
            if (slot == null) continue;

            UIItemData uiItemData = slot.GetComponentInChildren<UIItemData>();
            if (uiItemData != null && uiItemData.itemIconImage != null)
            {
                uiItemData.itemIconImage.color = Color.white;
            }
        }
    }
}
