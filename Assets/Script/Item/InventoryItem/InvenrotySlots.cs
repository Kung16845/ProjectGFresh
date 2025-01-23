using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InvenrotySlots : MonoBehaviour, IDropHandler
{
    public SlotType slotTypeInventory;
    public GameObject uIMoveItemsBoxesToInventory;
    public Canvas canvas;
    public int maxCountItems;
    public InventoryItemPresent inventoryItemPresent;
    public UIInventory uIInventory;

    void Start()
    {
        // Find canvas and inventory item presenter in the active scene
        canvas = FindObjectOfType<Canvas>();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();

        // Get all objects of type ScriptMoveItems
        if (uIMoveItemsBoxesToInventory == null)
        {
            ScriptMoveItems[] objectsWithScript = Resources.FindObjectsOfTypeAll<ScriptMoveItems>();

            // Get the active scene
            Scene activeScene = SceneManager.GetActiveScene();

            foreach (var item in objectsWithScript)
            {
                GameObject obj = item.gameObject;

                // Ensure the object is in the active scene and is not inactive
                if (obj.scene == activeScene && !obj.activeInHierarchy)
                {
                    uIMoveItemsBoxesToInventory = obj;
                    ScriptMoveItems scriptMoveItems = uIMoveItemsBoxesToInventory.GetComponent<ScriptMoveItems>();
                    scriptMoveItems.uIInventory = uIInventory;
                    break; // Optional: Stop after finding the first match
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject uIitem = eventData.pointerDrag;
        if (uIitem == null) return;

        DraggableItem draggableItem = uIitem.GetComponent<DraggableItem>();
        UIItemData uIItemDataDrag = uIitem.GetComponent<UIItemData>();
        ItemClass itemClassMove = uIitem.GetComponent<ItemClass>();
        if (draggableItem == null || uIItemDataDrag == null || itemClassMove == null) return;

        InvenrotySlots originSlot = draggableItem.parentBeforeDray.GetComponentInParent<InvenrotySlots>();
        if (originSlot == null) return;

        ScriptMoveItems scriptMoveItems = uIMoveItemsBoxesToInventory.GetComponent<ScriptMoveItems>();
        scriptMoveItems.uIInventory = uIInventory;
        scriptMoveItems.itemClassMove = itemClassMove;
        scriptMoveItems.draggableItemMove = draggableItem;

        // The slot we are dropping into
        SlotType destinationSlotType = slotTypeInventory;

        // Check if there's an existing item in the destination slot
        ItemClass itemClassInChild = GetComponentInChildren<ItemClass>();
        if (destinationSlotType == SlotType.SlotLock || 
            (destinationSlotType != SlotType.SlotBag && 
             destinationSlotType != SlotType.SlotCar &&
             destinationSlotType != SlotType.SlotBoxes && 
             destinationSlotType != SlotType.SlotNpcTrade &&
             destinationSlotType != SlotType.SlotPlayerTrade &&
             destinationSlotType != SlotType.SlotNpcItem &&
             itemClassInChild != null &&
             destinationSlotType != uIItemDataDrag.slotType))
        {
            Debug.Log("Invalid drop target.");
            return;
        }

        TradesystemScript tradesystemScript = TradesystemScript.GetActiveTrade();
        if (tradesystemScript == null)
        {
            Debug.Log("No active TradesystemScript instance available. Proceeding without trade.");
        }

        List<ItemData> targetDataList = null;

        switch (destinationSlotType)
        {
            case SlotType.SlotNpcItem:
                targetDataList = tradesystemScript?.listInvenrotyNpcItem;
                break;
            case SlotType.SlotNpcTrade:
                targetDataList = tradesystemScript?.listNpcItemWaitforTrade;
                break;
            case SlotType.SlotPlayerTrade:
                targetDataList = tradesystemScript?.listPlayerItemWaitforTrade;
                break;
            case SlotType.SlotBag:
                targetDataList = uIInventory.listItemDataInventorySlot;
                break;
            case SlotType.SlotCar:
                targetDataList = ((UIInventoryEX)uIInventory).listItemDataCarInventorySlot;
                break;
            case SlotType.SlotWeapon:
            case SlotType.SlotVest:
            case SlotType.SlotTool:
            case SlotType.SlotBackpack:
            case SlotType.SlotGrenade:
                targetDataList = uIInventory.listItemDataInventoryEqicment;
                break;
            case SlotType.SlotBoxes:
                // Boxes use inventoryItemPresent.listItemsDataBox
                targetDataList = uIInventory.inventoryItemPresent.listItemsDataBox;
                break; 
            case SlotType.SlotLoot:
                // Handle SlotLoot separately if needed
                break;
            case SlotType.SlotLock:
                return;
        }

        if ((destinationSlotType == SlotType.SlotWeapon || 
             destinationSlotType == SlotType.SlotVest || 
             destinationSlotType == SlotType.SlotTool || 
             destinationSlotType == SlotType.SlotBackpack || 
             destinationSlotType == SlotType.SlotGrenade) && 
            itemClassInChild != null && 
            destinationSlotType != SlotType.SlotNpcItem)
        {
            // Item already exists in this equipment slot, cancel the transfer
            Debug.Log("Slot already occupied. Cannot move item.");
            return;
        }

        // Determine if UI should be opened
        bool openUI = false;
        int quantityToMove = itemClassMove.quantityItem;

        // Backpack logic: Always transfer 1 backpack
        if (itemClassMove.itemtype == Itemtype.Backpack)
        {
            quantityToMove = 1;
        }
        else if (itemClassMove.maxCountItem == 1)
        {
            // Always transfer 1 item if maxCountItem is 1
            quantityToMove = 1;
        }
        else if (itemClassMove.quantityItem > 1 && destinationSlotType != SlotType.SlotBoxes)
        {
            // Open UI for deciding the quantity if moving to non-boxes with multiple items
            openUI = true;
        }
        if (itemClassMove.itemtype == Itemtype.Ammo && destinationSlotType != SlotType.SlotNpcTrade) 
        {
            openUI = false;
            quantityToMove = itemClassMove.maxCountItem;
        }
        // Prevent invalid trade interactions
        if ((destinationSlotType == SlotType.SlotNpcTrade && originSlot.slotTypeInventory != SlotType.SlotNpcItem) ||
            (destinationSlotType == SlotType.SlotPlayerTrade && originSlot.slotTypeInventory == SlotType.SlotNpcItem) ||
            (destinationSlotType == SlotType.SlotNpcItem && originSlot.slotTypeInventory == SlotType.SlotPlayerTrade) ||
            (destinationSlotType == SlotType.SlotPlayerTrade && originSlot.slotTypeInventory == SlotType.SlotNpcTrade)||
            (destinationSlotType == SlotType.SlotBag && originSlot.slotTypeInventory == SlotType.SlotNpcItem)||
            (destinationSlotType == SlotType.SlotBackpack && originSlot.slotTypeInventory == SlotType.SlotNpcItem)||
            (destinationSlotType == SlotType.SlotBackpack && originSlot.slotTypeInventory == SlotType.SlotNpcTrade)||
            (destinationSlotType == SlotType.SlotBag && originSlot.slotTypeInventory == SlotType.SlotNpcTrade))
        {
            Debug.Log("Invalid trade interaction.");
            return;
        }

        if (destinationSlotType == SlotType.SlotBoxes || destinationSlotType == SlotType.SlotNpcItem)
        {
            // For SlotBoxes, always transfer the full quantity without opening UI
            openUI = false;
            quantityToMove = itemClassMove.quantityItem;
        }

        if (openUI)
        {
            // Set up ScriptMoveItems for UI flow
            scriptMoveItems.countItemMove = 1; // Default to 1
            scriptMoveItems.countText.text = "1";
            scriptMoveItems.itemClassMove = itemClassMove;
            scriptMoveItems.draggableItemMove = draggableItem;
            scriptMoveItems.sourceSlotType = originSlot.slotTypeInventory;
            scriptMoveItems.targetSlotType = destinationSlotType;

            // Show the UI
            uIMoveItemsBoxesToInventory.SetActive(true);

            // Position UI near the cursor
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out mousePos);
            uIMoveItemsBoxesToInventory.GetComponent<RectTransform>().anchoredPosition = mousePos;

            Debug.Log("Opened Move Items UI.");
            return;
        }

        if (targetDataList == null && (destinationSlotType == SlotType.SlotNpcTrade || destinationSlotType == SlotType.SlotPlayerTrade))
        {
            Debug.LogError("Trade target list is null. Cannot proceed with the trade.");
            return;
        }

        TransferItems(itemClassMove, quantityToMove, originSlot, destinationSlotType, itemClassInChild, targetDataList);
        if(tradesystemScript != null)
        {
            tradesystemScript.RefreshTrade();
        }
    }

    private void TransferItems(ItemClass itemClassMove, int quantityToMove, InvenrotySlots originSlot, SlotType destinationSlotType, ItemClass itemClassInChild, List<ItemData> targetDataList)
    {
        // Convert ItemClass to ItemData
        ItemData sourceItemData = inventoryItemPresent.ConventItemClassToItemData(itemClassMove);
        if (originSlot.slotTypeInventory == SlotType.SlotLoot)
        {
            LootingSystem lootSystem = itemClassMove.GetComponent<UIItemData>()?.originatingLootSystem;
            if (lootSystem != null)
            {
                lootSystem.RemoveItemFromLootList(sourceItemData.idItem, quantityToMove);
            }
        }
        // Handle SlotBoxes separately to transfer full quantity
        if (destinationSlotType == SlotType.SlotBoxes)
        {
            // Add to boxes
            AddOrUpdateItemDataInList(uIInventory.inventoryItemPresent.listItemsDataBox, sourceItemData, quantityToMove);

            // Remove from origin
            RemoveItemDataFromOrigin(originSlot.slotTypeInventory, sourceItemData, quantityToMove);

            // Destroy the original UI item if all quantity moved
            if (quantityToMove == itemClassMove.quantityItem)
            {
                Destroy(itemClassMove.gameObject);
            }
            else
            {
                // Decrease the quantity if partially moved
                itemClassMove.quantityItem -= quantityToMove;
                itemClassMove.GetComponent<UIItemData>().UpdateDataUI(itemClassMove);
            }

            // Refresh UI
            inventoryItemPresent.RefreshUIBox();
            uIInventory.RefreshUIInventory();
            uIInventory.RefreshUIBoxCategory(uIInventory.currentNumCategory);
            return;
        }

        AddOrUpdateItemDataInList(targetDataList, sourceItemData, quantityToMove);
        RemoveItemDataFromOrigin(originSlot.slotTypeInventory, sourceItemData, quantityToMove);

        // If entire quantity moved, destroy the UI item
        if (quantityToMove == itemClassMove.quantityItem)
        {
            Destroy(itemClassMove.gameObject);
        }
        else
        {
            // Decrease the quantity if partially moved
            itemClassMove.quantityItem -= quantityToMove;
            itemClassMove.GetComponent<UIItemData>().UpdateDataUI(itemClassMove);
        }
        // Refresh UI
        inventoryItemPresent.RefreshUIBox();
        uIInventory.RefreshUIInventory();
        uIInventory.RefreshUIBoxCategory(uIInventory.currentNumCategory);
    }


    private void AddOrUpdateItemDataInList(List<ItemData> list, ItemData sourceItem, int quantity)
    {
        var existingItem = list.FirstOrDefault(i => i.idItem == sourceItem.idItem && i.itemtype == sourceItem.itemtype);
        if (existingItem != null)
        {
            // Update the count of the existing item
            existingItem.count += quantity;
        }
        else
        {
            // Add a new item to the list
            ItemData newItem = new ItemData
            {
                idItem = sourceItem.idItem,
                nameItem = sourceItem.nameItem,
                count = quantity,
                maxCount = sourceItem.maxCount,
                itemtype = sourceItem.itemtype,
            };
            list.Add(newItem);
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
            originList = uIInventory.listItemDataInventoryEqicment;
        else if (originSlotType == SlotType.SlotNpcItem)
            originList = tradesystemScript?.listInvenrotyNpcItem;
        else if (originSlotType == SlotType.SlotPlayerTrade)
            originList = tradesystemScript?.listPlayerItemWaitforTrade;
        else if (originSlotType == SlotType.SlotNpcTrade)
            originList = tradesystemScript?.listNpcItemWaitforTrade;

        if (originList == null) return;
        
        // Find and update or remove the item in the source list
        var originItem = originList.FirstOrDefault(i => i.idItem == sourceItem.idItem && i.itemtype == sourceItem.itemtype);
        if (originItem != null)
        {
            originItem.count -= quantity;
            if (originItem.count <= 0)
                originList.Remove(originItem);
        }
        tradesystemScript?.RefreshTrade();
        Debug.Log(originSlotType);
        Debug.Log(quantity);
        Debug.Log(sourceItem.nameItem);
    }
}

public enum SlotType
{
    SlotWeapon,
    SlotVest,
    SlotTool,
    SlotBackpack,
    SlotGrenade,
    SlotBag,
    SlotLock,
    SlotBoxes,
    SlotLoot,
    SlotPlayerTrade,
    SlotNpcItem,
    SlotNpcTrade,
    SlotCar
}