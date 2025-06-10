using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public TradesystemScript tradeSystem; // This will be set dynamically
    public UIInventory uIInventory;
    // Start is called before the first frame update

    private void OnEnable()
    {
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        uIInventory = FindObjectOfType<UIInventory>();
        // Debug.Log("Open UI ScriptMoveItens");
    }
    void Start()
    {
        uIInventory = FindObjectOfType<UIInventory>();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        countItemMove = 1;
        countText.text = countItemMove.ToString();

    }
    public void IncreateCountItem(int count)
    {
        countItemMove += count;
        SlotType slotTypeItemMove = itemClassMove.gameObject.GetComponentInParent<InventorySlots>().slotTypeInventory;

        if (slotTypeItemMove == SlotType.SlotLoot)
        {
            // Handle count increase for loot system
            LootingSystem lootSystem = itemClassMove.GetComponent<UIItemData>().originatingLootSystem;

            if (lootSystem != null)
            {
                ItemData lootItem = lootSystem.droppedItems.FirstOrDefault(d => d.idItem == itemClassMove.idItem);
                if (lootItem != null)
                {
                    // Limit countItemMove to the available loot item quantity
                    if (countItemMove > lootItem.count)
                    {
                        countItemMove = lootItem.count;
                    }
                }
            }
        }

        if (itemClassInChild == null)
        {
            // Check if countItemMove exceeds maxCountItem or available quantity
            if (countItemMove > itemClassMove.quantityItem)
            {
                countItemMove = itemClassMove.quantityItem;
            }
            if (countItemMove > itemClassMove.maxCountItem)
            {
                countItemMove = itemClassMove.maxCountItem;
            }
        }
        else if (itemClassInChild != null)
        {
            // Ensure the total quantity in child and move does not exceed maxCountItem
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
        countText.text = countItemMove.ToString();
    }

    public void DecreasteCountItem(int count)
    {
        countItemMove -= count;

        // Ensure countItemMove does not fall below 1
        if (countItemMove < 1)
        {
            countItemMove = 1;
        }

        SlotType slotTypeItemMove = itemClassMove.gameObject.GetComponentInParent<InventorySlots>().slotTypeInventory;

        if (slotTypeItemMove == SlotType.SlotLoot)
        {
            // Handle count decrease for loot system
            LootingSystem lootSystem = itemClassMove.GetComponent<UIItemData>().originatingLootSystem;

            if (lootSystem != null)
            {
                ItemData lootItem = lootSystem.droppedItems.FirstOrDefault(d => d.idItem == itemClassMove.idItem);
                if (lootItem != null)
                {
                    if (countItemMove > lootItem.count)
                    {
                        countItemMove = lootItem.count;
                    }
                }
            }
        }

        if (countItemMove == 1)
        {
            if (itemClassInChild == null)
            {
                countItemMove = Mathf.Min(itemClassMove.maxCountItem, itemClassMove.quantityItem);
            }
            else if (itemClassInChild != null)
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
            else if (itemClassInChild != null)
            {
                int maxAllowed = itemClassMove.maxCountItem - itemClassInChild.quantityItem;

                if (countItemMove > itemClassMove.quantityItem || countItemMove > maxAllowed)
                {
                    countItemMove = Mathf.Min(itemClassMove.quantityItem, maxAllowed);
                }
            }
        }

        countText.text = countItemMove.ToString();
    }
    public void MoveItem()
    {
        if (itemClassMove == null || draggableItemMove == null) return;

        int actualQuantityToMove = countItemMove;
        ItemData sourceItemData = inventoryItemPresent.ConventItemClassToItemData(itemClassMove);

        List<ItemData> targetList = null;
        TradesystemScript tradesystemScript = TradesystemScript.GetActiveTrade(); // Use activeTrade

        if (targetSlotType == SlotType.SlotBag)
        {
            targetList = uIInventory.listItemDataInventorySlot;
        }
        else if (targetSlotType == SlotType.SlotCar)
        {
            targetList = ((UIInventoryEX)uIInventory).listItemDataCarInventorySlot;
        }
        else if (targetSlotType == SlotType.SlotBoxes)
        {
            targetList = uIInventory.inventoryItemPresent.listItemsDataBox;
        }
        else if (targetSlotType == SlotType.SlotWeapon || targetSlotType == SlotType.SlotVest ||
                targetSlotType == SlotType.SlotTool || targetSlotType == SlotType.SlotBackpack ||
                targetSlotType == SlotType.SlotGrenade)
        {
            targetList = uIInventory.listItemDataInventoryEquipment;
        }
        else if (targetSlotType == SlotType.SlotNpcTrade || targetSlotType == SlotType.SlotPlayerTrade)
        {
            if (tradesystemScript != null)
            {
                if(targetSlotType == SlotType.SlotPlayerTrade)
                    targetList = tradesystemScript.listPlayerItemWaitforTrade;
                else if(targetSlotType == SlotType.SlotNpcItem)
                    targetList = tradesystemScript.listInvenrotyNpcItem;
                else
                    targetList = tradesystemScript.listNpcItemWaitforTrade;
            }
        }
        if (targetList == null) return;

        if (sourceSlotType == SlotType.SlotLoot)
        {
            LootingSystem currentLootSystem = uIInventory.currentLootingSystem;

            if (currentLootSystem != null)
            {
                currentLootSystem.RemoveItemFromLootList(sourceItemData.idItem, actualQuantityToMove);
            }
        }

        if (itemClassMove.itemtype == Itemtype.Backpack)
        {
            if (sourceSlotType != SlotType.SlotBoxes && targetSlotType != SlotType.SlotBoxes)
            {
                // Remove backpack effect from source slot
                uIInventory.npcSelecting.countInventorySlot -= uIInventory.SlotHasincreased;
                uIInventory.SlotHasincreased = 0;
            }

            if (targetSlotType == SlotType.SlotBag)
            {
                // Add backpack effect to target slot
                ItemBackpack backpack = itemClassMove.GetComponent<ItemBackpack>();
                uIInventory.npcSelecting.countInventorySlot += backpack.slotIncreasing;
                uIInventory.SlotHasincreased = backpack.slotIncreasing;
            }
        }

        // Add the item to the target list
        AddOrUpdateItemDataInList(targetList, sourceItemData, actualQuantityToMove);

        // Remove the item from the source list
        RemoveItemDataFromOrigin(sourceSlotType, sourceItemData, actualQuantityToMove);

        // Update UI or destroy item if quantity reaches zero
        if (actualQuantityToMove == itemClassMove.quantityItem)
        {
            Destroy(itemClassMove.gameObject);
        }
        else
        {
            itemClassMove.quantityItem -= actualQuantityToMove;
            UpdateUIItemMove();
        }

        if (targetSlotType == SlotType.SlotCar)
        {
            ((UIInventoryEX)uIInventory).RefreshUIInventory();
        }
        if(tradesystemScript != null)
        {
            tradesystemScript.RefreshTrade();
        }
        uIInventory.RefreshUIInventory();
        inventoryItemPresent.RefreshUIBox();
        uIInventory.RefreshUIBoxCategory(uIInventory.currentNumCategory);
        Debug.Log($"Moved {actualQuantityToMove} of {sourceItemData.nameItem} from {sourceSlotType} to {targetSlotType}");

        // Close the move UI
        gameObject.SetActive(false);
    }


    private void AddOrUpdateItemDataInList(List<ItemData> list, ItemData sourceItem, int quantity)
    {
        // Check if the item already exists in the target list
        var existingItem = list.FirstOrDefault(item => item.idItem == sourceItem.idItem);

        if (existingItem != null)
        {
            int totalQuantity = existingItem.count + quantity;

            if (totalQuantity <= existingItem.maxCount)
            {
                // Fits within the same slot
                existingItem.count = totalQuantity;
            }
            else
            {
                // Exceeds maxCount, split the excess
                existingItem.count = existingItem.maxCount;
                int excess = totalQuantity - existingItem.maxCount;

                // Create new entries for the excess
                while (excess > 0)
                {
                    int newSlotQuantity = Mathf.Min(excess, sourceItem.maxCount);
                    ItemData newItem = new ItemData
                    {
                        idItem = sourceItem.idItem,
                        nameItem = sourceItem.nameItem,
                        count = newSlotQuantity,
                        maxCount = sourceItem.maxCount,
                        itemtype = sourceItem.itemtype,
                    };

                    list.Add(newItem);
                    excess -= newSlotQuantity;
                }
            }
        }
        else
        {
            // No existing entry, add a new one
            while (quantity > 0)
            {
                int newSlotQuantity = Mathf.Min(quantity, sourceItem.maxCount);
                ItemData newItem = new ItemData
                {
                    idItem = sourceItem.idItem,
                    nameItem = sourceItem.nameItem,
                    count = newSlotQuantity,
                    maxCount = sourceItem.maxCount,
                    itemtype = sourceItem.itemtype,
                };

                list.Add(newItem);
                quantity -= newSlotQuantity;
            }
        }
    }


    private void RemoveItemDataFromOrigin(SlotType originSlotType, ItemData sourceItem, int quantity)
    {
        List<ItemData> originList = null;
        TradesystemScript tradesystemScript = TradesystemScript.GetActiveTrade();
        // Determine the appropriate source list based on the origin slot type
        if (originSlotType == SlotType.SlotBag)
            originList = uIInventory.listItemDataInventorySlot;
        else if (originSlotType == SlotType.SlotCar)
            originList = ((UIInventoryEX)uIInventory).listItemDataCarInventorySlot;
        else if (originSlotType == SlotType.SlotBoxes)
            originList = uIInventory.inventoryItemPresent.listItemsDataBox;
        else if (originSlotType == SlotType.SlotWeapon || originSlotType == SlotType.SlotVest ||
                originSlotType == SlotType.SlotTool || originSlotType == SlotType.SlotBackpack || 
                originSlotType == SlotType.SlotGrenade)
            originList = uIInventory.listItemDataInventoryEquipment;
        else if (originSlotType == SlotType.SlotNpcItem)
            originList = tradesystemScript?.listInvenrotyNpcItem;
        else if (originSlotType == SlotType.SlotPlayerTrade)
            originList = tradesystemScript?.listPlayerItemWaitforTrade;
        else if (originSlotType == SlotType.SlotNpcTrade)
            originList = tradesystemScript?.listNpcItemWaitforTrade;

        if (originList == null) return;

        // Find the item in the origin list
        var originItem = originList.FirstOrDefault(item => item.idItem == sourceItem.idItem && item.itemtype == sourceItem.itemtype);
        if (originItem != null)
        {
            // Decrease the count of the item
            originItem.count -= quantity;
            if (originItem.count <= 0)
            {
                // Remove the item from the list if the count drops to zero
                originList.Remove(originItem);
                Debug.Log("Remove Item");
            }
        }
    }


    public void UpdateUIItemMove()
    {
        GameObject uIItemObject = itemClassMove.gameObject;
        UIItemData uIItemData = uIItemObject.GetComponent<UIItemData>();
        uIItemData.slotTypeParent = uIItemData.GetComponentInParent<InventorySlots>().slotTypeInventory;
        uIItemData.UpdateDataUI(itemClassMove);
    }
    public void CancleMove()
    {

        DraggableItem draggableItemMove = itemClassMove.gameObject.GetComponent<DraggableItem>();
        draggableItemMove.transform.SetParent(draggableItemMove.parentBeforeDray);
        draggableItemMove.parentAfterDray = draggableItemMove.parentBeforeDray;
        if (draggableItemMove.parentBeforeDray == inventoryItemPresent.transformsBoxes)
        {
            Destroy(draggableItemMove.gameObject);
        }
        inventoryItemPresent.ResetAmmoHighlighting();
        gameObject.SetActive(false);
    }
}
