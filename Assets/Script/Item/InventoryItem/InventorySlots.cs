using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InventorySlots : MonoBehaviour, IDropHandler
{
    public SlotType slotTypeInventory;
    public GameObject uIMoveItemsBoxesToInventory;
    public Canvas canvas;
    public int maxCountItems;
    public InventoryItemPresent inventoryItemPresent;
    public UIInventory uIInventory;

    private static GameObject s_cachedMoveItemsUI;

    private void Start()
    {
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        if (GameManager.Instance != null && GameManager.Instance.inventoryItemPresent != null)
        {
            inventoryItemPresent = GameManager.Instance.inventoryItemPresent;
        }
        else if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance;
        }

        // Cache and find ScriptMoveItems UI object once across slots
        if (uIMoveItemsBoxesToInventory == null)
        {
            if (s_cachedMoveItemsUI != null)
            {
                uIMoveItemsBoxesToInventory = s_cachedMoveItemsUI;
            }
            else
            {
                ScriptMoveItems[] objectsWithScript = Resources.FindObjectsOfTypeAll<ScriptMoveItems>();
                Scene activeScene = SceneManager.GetActiveScene();

                for (int i = 0; i < objectsWithScript.Length; i++)
                {
                    GameObject obj = objectsWithScript[i].gameObject;
                    if (obj.scene == activeScene && !obj.activeInHierarchy)
                    {
                        uIMoveItemsBoxesToInventory = obj;
                        s_cachedMoveItemsUI = obj;
                        break;
                    }
                }
            }

            if (uIMoveItemsBoxesToInventory != null)
            {
                ScriptMoveItems scriptMoveItems = uIMoveItemsBoxesToInventory.GetComponent<ScriptMoveItems>();
                if (scriptMoveItems != null && uIInventory != null)
                {
                    scriptMoveItems.uIInventory = uIInventory;
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (slotTypeInventory == SlotType.SlotLock) return;
        if (eventData == null || eventData.pointerDrag == null) return;

        GameObject uIitem = eventData.pointerDrag;
        DraggableItem draggableItem = uIitem.GetComponent<DraggableItem>();
        UIItemData uIItemDataDrag = uIitem.GetComponent<UIItemData>();
        ItemClass itemClassMove = uIitem.GetComponent<ItemClass>();
        if (draggableItem == null || uIItemDataDrag == null || itemClassMove == null) return;

        if (draggableItem.parentBeforeDray == null) return;
        InventorySlots originSlot = draggableItem.parentBeforeDray.GetComponentInParent<InventorySlots>();
        if (originSlot == null) return;

        if (uIMoveItemsBoxesToInventory == null) return;
        ScriptMoveItems scriptMoveItems = uIMoveItemsBoxesToInventory.GetComponent<ScriptMoveItems>();
        if (scriptMoveItems == null) return;

        scriptMoveItems.uIInventory = uIInventory;
        scriptMoveItems.itemClassMove = itemClassMove;
        scriptMoveItems.draggableItemMove = draggableItem;

        // The slot we are dropping into
        SlotType destinationSlotType = slotTypeInventory;

        // Check if there's an existing item in the destination slot
        ItemClass itemClassInChild = GetComponentInChildren<ItemClass>();
        if (slotTypeInventory == SlotType.SlotLock || 
            (slotTypeInventory != SlotType.SlotBag && 
             slotTypeInventory != SlotType.SlotCar &&
             slotTypeInventory != SlotType.SlotBoxes && 
             slotTypeInventory != SlotType.SlotNpcTrade &&
             slotTypeInventory != SlotType.SlotPlayerTrade &&
             slotTypeInventory != SlotType.SlotNpcItem &&
             itemClassInChild != null &&
             destinationSlotType != uIItemDataDrag.slotType))
        {
            Debug.Log("Invalid drop target.");
            return;
        }

        TradesystemScript tradesystemScript = TradesystemScript.GetActiveTrade();

        List<ItemData> targetDataList = null;

        switch (slotTypeInventory)
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
                targetDataList = uIInventory != null ? uIInventory.listItemDataInventorySlot : null;
                break;
            case SlotType.SlotCar:
                targetDataList = (uIInventory is UIInventoryEX exUI) ? exUI.listItemDataCarInventorySlot : null;
                break;
            case SlotType.SlotWeapon:
            case SlotType.SlotVest:
            case SlotType.SlotTool:
            case SlotType.SlotBackpack:
            case SlotType.SlotGrenade:
                targetDataList = uIInventory != null ? uIInventory.listItemDataInventoryEquipment : null;
                break;
            case SlotType.SlotBoxes:
                var present = (uIInventory != null && uIInventory.inventoryItemPresent != null) 
                    ? uIInventory.inventoryItemPresent 
                    : (inventoryItemPresent != null ? inventoryItemPresent : InventoryItemPresent.Instance);
                targetDataList = present != null ? present.listItemsDataBox : null;
                break; 
            case SlotType.SlotLoot:
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
            Debug.Log("Slot already occupied. Cannot move item.");
            return;
        }

        // Determine if UI should be opened
        bool openUI = false;
        int quantityToMove = itemClassMove.quantityItem;

        if (itemClassMove.itemtype == Itemtype.Backpack || itemClassMove.maxCountItem == 1)
        {
            quantityToMove = 1;
        }
        else if (itemClassMove.quantityItem > 1 && destinationSlotType != SlotType.SlotBoxes)
        {
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
            (destinationSlotType == SlotType.SlotPlayerTrade && originSlot.slotTypeInventory == SlotType.SlotNpcTrade) ||
            (destinationSlotType == SlotType.SlotBag && originSlot.slotTypeInventory == SlotType.SlotNpcItem) ||
            (destinationSlotType == SlotType.SlotBackpack && originSlot.slotTypeInventory == SlotType.SlotNpcItem) ||
            (destinationSlotType == SlotType.SlotBackpack && originSlot.slotTypeInventory == SlotType.SlotNpcTrade) ||
            (destinationSlotType == SlotType.SlotBag && originSlot.slotTypeInventory == SlotType.SlotNpcTrade))
        {
            Debug.Log("Invalid trade interaction.");
            return;
        }

        if (destinationSlotType == SlotType.SlotBoxes || destinationSlotType == SlotType.SlotNpcItem)
        {
            openUI = false;
            quantityToMove = itemClassMove.quantityItem;
        }

        if (openUI)
        {
            scriptMoveItems.countItemMove = 1;
            if (scriptMoveItems.countText != null)
            {
                scriptMoveItems.countText.text = "1";
            }
            scriptMoveItems.itemClassMove = itemClassMove;
            scriptMoveItems.draggableItemMove = draggableItem;
            scriptMoveItems.sourceSlotType = originSlot.slotTypeInventory;
            scriptMoveItems.targetSlotType = destinationSlotType;

            uIMoveItemsBoxesToInventory.SetActive(true);

            if (canvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    Input.mousePosition,
                    canvas.worldCamera,
                    out Vector2 mousePos);
                RectTransform rectTransform = uIMoveItemsBoxesToInventory.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = mousePos;
                }
            }
            return;
        }

        if (targetDataList == null && (destinationSlotType == SlotType.SlotNpcTrade || destinationSlotType == SlotType.SlotPlayerTrade))
        {
            Debug.LogError("Trade target list is null. Cannot proceed with the trade.");
            return;
        }

        TransferItems(itemClassMove, quantityToMove, originSlot, destinationSlotType, itemClassInChild, targetDataList);
        if (tradesystemScript != null)
        {
            tradesystemScript.RefreshTrade();
        }
    }

    private void TransferItems(ItemClass itemClassMove, int quantityToMove, InventorySlots originSlot, SlotType destinationSlotType, ItemClass itemClassInChild, List<ItemData> targetDataList)
    {
        InventoryItemPresent presenter = (inventoryItemPresent != null) ? inventoryItemPresent : InventoryItemPresent.Instance;
        if (presenter == null || itemClassMove == null) return;

        ItemData sourceItemData = presenter.ConventItemClassToItemData(itemClassMove);
        if (sourceItemData == null) return;

        if (originSlot.slotTypeInventory == SlotType.SlotLoot)
        {
            UIItemData uiItemData = itemClassMove.GetComponent<UIItemData>();
            if (uiItemData != null && uiItemData.originatingLootSystem != null)
            {
                uiItemData.originatingLootSystem.RemoveItemFromLootList(sourceItemData.idItem, quantityToMove);
            }
        }

        if (destinationSlotType == SlotType.SlotBoxes)
        {
            List<ItemData> boxList = (uIInventory != null && uIInventory.inventoryItemPresent != null)
                ? uIInventory.inventoryItemPresent.listItemsDataBox
                : presenter.listItemsDataBox;

            AddOrUpdateItemDataInList(boxList, sourceItemData, quantityToMove);
            RemoveItemDataFromOrigin(originSlot.slotTypeInventory, sourceItemData, quantityToMove);

            if (quantityToMove >= itemClassMove.quantityItem)
            {
                Destroy(itemClassMove.gameObject);
            }
            else
            {
                itemClassMove.quantityItem -= quantityToMove;
                UIItemData uiItem = itemClassMove.GetComponent<UIItemData>();
                if (uiItem != null) uiItem.UpdateDataUI(itemClassMove);
            }

            presenter.RefreshUIBox();
            if (uIInventory != null)
            {
                uIInventory.RefreshUIInventory();
                uIInventory.RefreshUIBoxCategory(uIInventory.currentNumCategory);
            }
            return;
        }

        if (targetDataList != null)
        {
            AddOrUpdateItemDataInList(targetDataList, sourceItemData, quantityToMove);
        }
        RemoveItemDataFromOrigin(originSlot.slotTypeInventory, sourceItemData, quantityToMove);

        if (quantityToMove >= itemClassMove.quantityItem)
        {
            Destroy(itemClassMove.gameObject);
        }
        else
        {
            itemClassMove.quantityItem -= quantityToMove;
            UIItemData uiItem = itemClassMove.GetComponent<UIItemData>();
            if (uiItem != null) uiItem.UpdateDataUI(itemClassMove);
        }

        presenter.RefreshUIBox();
        if (uIInventory != null)
        {
            uIInventory.RefreshUIInventory();
            uIInventory.RefreshUIBoxCategory(uIInventory.currentNumCategory);
        }
    }

    private void AddOrUpdateItemDataInList(List<ItemData> list, ItemData sourceItem, int quantity)
    {
        if (list == null || sourceItem == null) return;

        ItemData existingItem = null;
        for (int i = 0; i < list.Count; i++)
        {
            ItemData item = list[i];
            if (item != null && item.idItem == sourceItem.idItem && item.itemtype == sourceItem.itemtype)
            {
                existingItem = item;
                break;
            }
        }

        if (existingItem != null)
        {
            existingItem.count += quantity;
        }
        else
        {
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

        tradesystemScript?.RefreshTrade();
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