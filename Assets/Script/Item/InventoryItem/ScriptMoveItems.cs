using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScriptMoveItems : MonoBehaviour
{
    public int countItemMove = 1;
    public TextMeshProUGUI countText;
    public ItemClass itemClassMove;
    public LootingSystem originatingLootSystem;
    public ItemClass itemClassInChild;
    public DraggableItem draggableItemMove;
    public SlotType sourceSlotType;
    public SlotType targetSlotType;
    public InventoryItemPresent inventoryItemPresent;
    public TradesystemScript tradeSystem;
    public UIInventory uIInventory;

    private void Awake()
    {
        CacheReferences();
    }

    private void OnEnable()
    {
        CacheReferences();
        countItemMove = 1;
        if (countText != null)
        {
            countText.text = countItemMove.ToString();
        }
    }

    private void Start()
    {
        CacheReferences();
        countItemMove = 1;
        if (countText != null)
        {
            countText.text = countItemMove.ToString();
        }
    }

    private void CacheReferences()
    {
        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        }
        if (uIInventory == null)
        {
            uIInventory = FindFirstObjectByType<UIInventory>();
        }
    }

    public void IncreateCountItem(int count)
    {
        if (itemClassMove == null) return;
        countItemMove += count;

        InventorySlots parentSlot = itemClassMove.GetComponentInParent<InventorySlots>();
        SlotType slotTypeItemMove = parentSlot != null ? parentSlot.slotTypeInventory : SlotType.SlotLock;

        if (slotTypeItemMove == SlotType.SlotLoot)
        {
            UIItemData uiData = itemClassMove.GetComponent<UIItemData>();
            LootingSystem lootSystem = uiData != null ? uiData.originatingLootSystem : null;

            if (lootSystem != null && lootSystem.droppedItems != null)
            {
                ItemData lootItem = null;
                for (int i = 0; i < lootSystem.droppedItems.Count; i++)
                {
                    if (lootSystem.droppedItems[i] != null && lootSystem.droppedItems[i].idItem == itemClassMove.idItem)
                    {
                        lootItem = lootSystem.droppedItems[i];
                        break;
                    }
                }
                if (lootItem != null && countItemMove > lootItem.count)
                {
                    countItemMove = lootItem.count;
                }
            }
        }

        if (itemClassInChild == null)
        {
            if (countItemMove > itemClassMove.quantityItem)
            {
                countItemMove = itemClassMove.quantityItem;
            }
            if (countItemMove > itemClassMove.maxCountItem)
            {
                countItemMove = itemClassMove.maxCountItem;
            }
        }
        else
        {
            int totalQuantity = itemClassInChild.quantityItem + countItemMove;
            if (totalQuantity > itemClassMove.quantityItem)
            {
                countItemMove = itemClassMove.quantityItem - itemClassInChild.quantityItem;
            }
            if (totalQuantity > itemClassMove.maxCountItem)
            {
                countItemMove = itemClassMove.maxCountItem - itemClassInChild.quantityItem;
            }
        }

        if (countItemMove < 1) countItemMove = 1;

        if (countText != null)
        {
            countText.text = countItemMove.ToString();
        }
    }

    public void DecreasteCountItem(int count)
    {
        if (itemClassMove == null) return;
        countItemMove -= count;

        if (countItemMove < 1)
        {
            countItemMove = 1;
        }

        InventorySlots parentSlot = itemClassMove.GetComponentInParent<InventorySlots>();
        SlotType slotTypeItemMove = parentSlot != null ? parentSlot.slotTypeInventory : SlotType.SlotLock;

        if (slotTypeItemMove == SlotType.SlotLoot)
        {
            UIItemData uiData = itemClassMove.GetComponent<UIItemData>();
            LootingSystem lootSystem = uiData != null ? uiData.originatingLootSystem : null;

            if (lootSystem != null && lootSystem.droppedItems != null)
            {
                ItemData lootItem = null;
                for (int i = 0; i < lootSystem.droppedItems.Count; i++)
                {
                    if (lootSystem.droppedItems[i] != null && lootSystem.droppedItems[i].idItem == itemClassMove.idItem)
                    {
                        lootItem = lootSystem.droppedItems[i];
                        break;
                    }
                }
                if (lootItem != null && countItemMove > lootItem.count)
                {
                    countItemMove = lootItem.count;
                }
            }
        }

        if (countItemMove == 1)
        {
            if (itemClassInChild == null)
            {
                countItemMove = Mathf.Min(itemClassMove.maxCountItem, itemClassMove.quantityItem);
            }
            else
            {
                int maxAllowed = itemClassMove.maxCountItem - itemClassInChild.quantityItem;
                countItemMove = Mathf.Min(itemClassMove.quantityItem, maxAllowed);
            }
        }
        else
        {
            if (itemClassInChild == null)
            {
                if (countItemMove > itemClassMove.quantityItem)
                {
                    countItemMove = itemClassMove.quantityItem;
                }
            }
            else
            {
                int maxAllowed = itemClassMove.maxCountItem - itemClassInChild.quantityItem;
                if (countItemMove > itemClassMove.quantityItem || countItemMove > maxAllowed)
                {
                    countItemMove = Mathf.Min(itemClassMove.quantityItem, maxAllowed);
                }
            }
        }

        if (countItemMove < 1) countItemMove = 1;

        if (countText != null)
        {
            countText.text = countItemMove.ToString();
        }
    }

    public void MoveItem()
    {
        if (itemClassMove == null || draggableItemMove == null) return;

        InventoryItemPresent presenter = (inventoryItemPresent != null) 
            ? inventoryItemPresent 
            : (uIInventory != null && uIInventory.inventoryItemPresent != null 
                ? uIInventory.inventoryItemPresent 
                : InventoryItemPresent.Instance);

        if (presenter == null) return;

        int actualQuantityToMove = countItemMove;
        ItemData sourceItemData = presenter.ConventItemClassToItemData(itemClassMove);
        if (sourceItemData == null) return;

        List<ItemData> targetList = null;
        TradesystemScript tradesystemScript = TradesystemScript.GetActiveTrade();

        if (targetSlotType == SlotType.SlotBag && uIInventory != null)
        {
            targetList = uIInventory.listItemDataInventorySlot;
        }
        else if (targetSlotType == SlotType.SlotCar && uIInventory is UIInventoryEX exUI)
        {
            targetList = exUI.listItemDataCarInventorySlot;
        }
        else if (targetSlotType == SlotType.SlotBoxes)
        {
            targetList = presenter.listItemsDataBox;
        }
        else if (uIInventory != null && (targetSlotType == SlotType.SlotWeapon || targetSlotType == SlotType.SlotVest ||
                targetSlotType == SlotType.SlotTool || targetSlotType == SlotType.SlotBackpack ||
                targetSlotType == SlotType.SlotGrenade))
        {
            targetList = uIInventory.listItemDataInventoryEquipment;
        }
        else if (targetSlotType == SlotType.SlotNpcTrade || targetSlotType == SlotType.SlotPlayerTrade)
        {
            if (tradesystemScript != null)
            {
                if (targetSlotType == SlotType.SlotPlayerTrade)
                    targetList = tradesystemScript.listPlayerItemWaitforTrade;
                else if (targetSlotType == SlotType.SlotNpcItem)
                    targetList = tradesystemScript.listInvenrotyNpcItem;
                else
                    targetList = tradesystemScript.listNpcItemWaitforTrade;
            }
        }

        if (targetList == null) return;

        if (sourceSlotType == SlotType.SlotLoot && uIInventory != null)
        {
            LootingSystem currentLootSystem = uIInventory.currentLootingSystem;
            if (currentLootSystem != null)
            {
                currentLootSystem.RemoveItemFromLootList(sourceItemData.idItem, actualQuantityToMove);
            }
        }

        if (itemClassMove.itemtype == Itemtype.Backpack && uIInventory != null && uIInventory.npcSelecting != null)
        {
            if (sourceSlotType != SlotType.SlotBoxes && targetSlotType != SlotType.SlotBoxes)
            {
                uIInventory.npcSelecting.countInventorySlot -= uIInventory.SlotHasincreased;
                uIInventory.SlotHasincreased = 0;
            }

            if (targetSlotType == SlotType.SlotBag)
            {
                ItemBackpack backpack = itemClassMove.GetComponent<ItemBackpack>();
                if (backpack != null)
                {
                    uIInventory.npcSelecting.countInventorySlot += backpack.slotIncreasing;
                    uIInventory.SlotHasincreased = backpack.slotIncreasing;
                }
            }
        }

        AddOrUpdateItemDataInList(targetList, sourceItemData, actualQuantityToMove);
        RemoveItemDataFromOrigin(sourceSlotType, sourceItemData, actualQuantityToMove);

        if (actualQuantityToMove >= itemClassMove.quantityItem)
        {
            Destroy(itemClassMove.gameObject);
        }
        else
        {
            itemClassMove.quantityItem -= actualQuantityToMove;
            UpdateUIItemMove();
        }

        if (targetSlotType == SlotType.SlotCar && uIInventory is UIInventoryEX exInventory)
        {
            exInventory.RefreshUIInventory();
        }

        if (tradesystemScript != null)
        {
            tradesystemScript.RefreshTrade();
        }

        if (uIInventory != null)
        {
            uIInventory.RefreshUIInventory();
            uIInventory.RefreshUIBoxCategory(uIInventory.currentNumCategory);
        }

        presenter.RefreshUIBox();
        gameObject.SetActive(false);
    }

    private void AddOrUpdateItemDataInList(List<ItemData> list, ItemData sourceItem, int quantity)
    {
        if (list == null || sourceItem == null) return;

        ItemData existingItem = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].idItem == sourceItem.idItem)
            {
                existingItem = list[i];
                break;
            }
        }

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
                    ItemData newItem = new ItemData
                    {
                        idItem = sourceItem.idItem,
                        nameItem = sourceItem.nameItem,
                        count = newSlotQuantity,
                        maxCount = sourceItem.maxCount,
                        itemtype = sourceItem.itemtype
                    };

                    list.Add(newItem);
                    excess -= newSlotQuantity;
                }
            }
        }
        else
        {
            while (quantity > 0)
            {
                int newSlotQuantity = Mathf.Min(quantity, sourceItem.maxCount);
                ItemData newItem = new ItemData
                {
                    idItem = sourceItem.idItem,
                    nameItem = sourceItem.nameItem,
                    count = newSlotQuantity,
                    maxCount = sourceItem.maxCount,
                    itemtype = sourceItem.itemtype
                };

                list.Add(newItem);
                quantity -= newSlotQuantity;
            }
        }
    }

    private void RemoveItemDataFromOrigin(SlotType originSlotType, ItemData sourceItem, int quantity)
    {
        if (sourceItem == null) return;

        List<ItemData> originList = null;
        TradesystemScript tradesystemScript = TradesystemScript.GetActiveTrade();

        if (originSlotType == SlotType.SlotBag && uIInventory != null)
            originList = uIInventory.listItemDataInventorySlot;
        else if (originSlotType == SlotType.SlotCar && uIInventory is UIInventoryEX exUI)
            originList = exUI.listItemDataCarInventorySlot;
        else if (originSlotType == SlotType.SlotBoxes)
        {
            var presenter = (uIInventory != null && uIInventory.inventoryItemPresent != null) 
                ? uIInventory.inventoryItemPresent 
                : (inventoryItemPresent != null ? inventoryItemPresent : InventoryItemPresent.Instance);
            originList = presenter?.listItemsDataBox;
        }
        else if (uIInventory != null && (originSlotType == SlotType.SlotWeapon || originSlotType == SlotType.SlotVest ||
                originSlotType == SlotType.SlotTool || originSlotType == SlotType.SlotBackpack || 
                originSlotType == SlotType.SlotGrenade))
            originList = uIInventory.listItemDataInventoryEquipment;
        else if (originSlotType == SlotType.SlotNpcItem)
            originList = tradesystemScript?.listInvenrotyNpcItem;
        else if (originSlotType == SlotType.SlotPlayerTrade)
            originList = tradesystemScript?.listPlayerItemWaitforTrade;
        else if (originSlotType == SlotType.SlotNpcTrade)
            originList = tradesystemScript?.listNpcItemWaitforTrade;

        if (originList == null) return;

        for (int i = 0; i < originList.Count; i++)
        {
            ItemData originItem = originList[i];
            if (originItem != null && originItem.idItem == sourceItem.idItem && originItem.itemtype == sourceItem.itemtype)
            {
                originItem.count -= quantity;
                if (originItem.count <= 0)
                {
                    originList.RemoveAt(i);
                }
                break;
            }
        }
    }

    public void UpdateUIItemMove()
    {
        if (itemClassMove == null) return;

        GameObject uIItemObject = itemClassMove.gameObject;
        UIItemData uIItemData = uIItemObject.GetComponent<UIItemData>();
        if (uIItemData != null)
        {
            InventorySlots parentSlot = uIItemData.GetComponentInParent<InventorySlots>();
            if (parentSlot != null)
            {
                uIItemData.slotTypeParent = parentSlot.slotTypeInventory;
            }
            uIItemData.UpdateDataUI(itemClassMove);
        }
    }

    public void CancleMove()
    {
        if (itemClassMove != null)
        {
            DraggableItem draggable = itemClassMove.GetComponent<DraggableItem>();
            if (draggable != null)
            {
                if (draggable.parentBeforeDray != null)
                {
                    draggable.transform.SetParent(draggable.parentBeforeDray);
                    draggable.parentAfterDray = draggable.parentBeforeDray;
                }

                InventoryItemPresent presenter = (inventoryItemPresent != null) 
                    ? inventoryItemPresent 
                    : InventoryItemPresent.Instance;

                if (presenter != null && draggable.parentBeforeDray == presenter.transformsBoxes)
                {
                    Destroy(draggable.gameObject);
                }
            }
        }

        InventoryItemPresent p = (inventoryItemPresent != null) ? inventoryItemPresent : InventoryItemPresent.Instance;
        if (p != null)
        {
            p.ResetAmmoHighlighting();
        }

        gameObject.SetActive(false);
    }
}
