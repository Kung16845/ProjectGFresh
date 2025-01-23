using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TradesystemScript : MonoBehaviour
{
    [SerializeField] private Transform itemsContainer;
    public List<ItemData> listInvenrotyNpcItem;
    public List<InvenrotySlots> listNpcItemWaitforTradeUI = new List<InvenrotySlots>();
    public List<ItemData> listNpcItemWaitforTrade = new List<ItemData>();
    public List<InvenrotySlots> listPlayerItemWaitforTradeUI = new List<InvenrotySlots>();
    public List<ItemData> listPlayerItemWaitforTrade = new List<ItemData>();

    private InventoryItemPresent inventoryItemPresent;
    private ActionController actionController;
    public UIInventory uIInventoryEX;
    public ScriptMoveItems scriptMoveItems;
    public GameObject Confirmobject;
    public Button ConfirmButton;
    public GameObject TradeUI;
    public TextMeshProUGUI statusTrade;
    private bool inrange;

    // Static list to keep track of all active trades
    private static List<TradesystemScript> activeTrades = new List<TradesystemScript>();

    // Static reference to the currently active trade
    private static TradesystemScript activeTrade;
    public static TradesystemScript GetActiveTrade()
    {
        return activeTrade;
    }

    private void Start()
    {
        actionController = FindObjectOfType<ActionController>();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        Confirmobject.SetActive(false);
        statusTrade.text = "What is your offer?";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inrange = true;
            activeTrades.Add(this);
            activeTrade = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inrange = false;
            activeTrades.Remove(this);
            // If this was the active trade, close it
            if (activeTrade == this)
            {
                CloseTradeUI();
            }
        }
    }

    private void Update()
    {
        // Only proceed if this is the closest active trade
        if (activeTrade != null && activeTrade != this)
            return;

        if (inrange && Input.GetKeyDown(KeyCode.Tab))
        {
            if (TradeUI.activeSelf)
            {
                CloseTradeUI();
                if (activeTrade == this)
                    activeTrade = null;
            }
            else
            {
                // If another trade is active, close it first
                if (activeTrade != null && activeTrade != this)
                {
                    activeTrade.CloseTradeUI();
                }

                activeTrade = this;
                TradeUI.SetActive(true);
                scriptMoveItems.tradeSystem = this;
                InitializeTradeUI();
            }
        }
        else if (!inrange && TradeUI.activeSelf)
        {
            if (activeTrade == this)
                activeTrade = null;
        }
    }

    private TradesystemScript GetClosestActiveTrade()
    {
        // Assuming you have a PlayerController with a Transform
        ActionController player = FindObjectOfType<ActionController>();
        if (player == null)
            return null;

        return activeTrades.OrderBy(trade => Vector2.Distance(trade.transform.position, player.transform.position)).FirstOrDefault();
    }

    private void CloseTradeUI()
    {
        // Return items to the player's inventory
        ReturnItemsToPlayer();

        // Clear UI and deactivate TradeUI
        ClearAllTradeUI();
        TradeUI.SetActive(false);

        // Clear the active trade if this is the active one
        if (activeTrade == this)
        {
            activeTrade = null;
        }
    }

    private void ReturnItemsToPlayer()
    {
        // Return NPC items in the trade slots back to their inventory
        foreach (var itemData in listNpcItemWaitforTrade)
        {
            AddOrUpdateItemDataInList(listInvenrotyNpcItem, itemData);
        }

        // Return Player items in the trade slots back to the player's inventory
        foreach (var itemData in this.listPlayerItemWaitforTrade)
        {
            uIInventoryEX.listItemDataInventorySlot.Add(itemData);
            uIInventoryEX.RefreshUIInventory();
            Debug.Log($"Items {itemData.nameItem} to inventories.{uIInventoryEX.name}");
        }

        // Clear the trade lists
        listNpcItemWaitforTrade.Clear();
        listPlayerItemWaitforTrade.Clear();

        Debug.Log("Items returned to respective inventories.");
    }

    private void AddOrUpdateItemDataInList(List<ItemData> itemList, ItemData itemData)
    {
        var existingItem = itemList.FirstOrDefault(item => item.idItem == itemData.idItem);
        if (existingItem != null)
        {
            existingItem.count += itemData.count;
        }
        else
        {
            itemList.Add(new ItemData
            {
                idItem = itemData.idItem,
                nameItem = itemData.nameItem,
                count = itemData.count,
                maxCount = itemData.maxCount,
                itemtype = itemData.itemtype,
            });
        }
    }

    private void InitializeTradeUI()
    {
        ClearAllTradeUI();
        foreach (var item in listInvenrotyNpcItem)
        {
            UIItemData prefabData = inventoryItemPresent.listUIItemPrefab.FirstOrDefault(prefab => prefab.idItem == item.idItem);

            if (prefabData != null)
            {
                GameObject newItemUI = Instantiate(prefabData.gameObject, itemsContainer);
                UIItemData uiItemData = newItemUI.GetComponent<UIItemData>();

                ItemClass itemClass = newItemUI.GetComponent<ItemClass>();
                itemClass.nameItem = item.nameItem;
                itemClass.idItem = item.idItem;
                itemClass.quantityItem = item.count;
                itemClass.maxCountItem = item.maxCount;
                itemClass.itemtype = item.itemtype;

                uiItemData.idItem = item.idItem;
                uiItemData.nameItem = item.nameItem;
                uiItemData.slotTypeParent = SlotType.SlotNpcItem;

                Sprite icon = inventoryItemPresent.GetItemIconByID(item.idItem);
                if (icon != null)
                {
                    uiItemData.itemIconImage.sprite = icon;
                }

                uiItemData.UpdateDataUI(itemClass);
            }
            else
            {
                Debug.LogWarning($"No matching UI prefab found for item ID: {item.idItem}");
            }
        }
        UpdateTradeStatus();
    }

    public void RefreshTrade()
    {
        // Step 1: Clear all child objects in NPC trade UI slots
        foreach (var slot in listNpcItemWaitforTradeUI)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Step 2: Clear all child objects in Player trade UI slots
        foreach (var slot in listPlayerItemWaitforTradeUI)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Step 3: Clear all child objects in the main NPC inventory container
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        uIInventoryEX.CombineAndSplitItems(listNpcItemWaitforTrade);
        uIInventoryEX.CombineAndSplitItems(listPlayerItemWaitforTrade);

        // Step 4: Populate the NPC inventory UI with items from `listInvenrotyNpcItem`
        foreach (var item in listInvenrotyNpcItem)
        {
            UIItemData prefabData = inventoryItemPresent.listUIItemPrefab.FirstOrDefault(prefab => prefab.idItem == item.idItem);

            if (prefabData != null)
            {
                GameObject newItemUI = Instantiate(prefabData.gameObject, itemsContainer);
                UIItemData uiItemData = newItemUI.GetComponent<UIItemData>();

                ItemClass itemClass = newItemUI.GetComponent<ItemClass>();
                itemClass.nameItem = item.nameItem;
                itemClass.idItem = item.idItem;
                itemClass.quantityItem = item.count;
                itemClass.maxCountItem = item.maxCount;
                itemClass.itemtype = item.itemtype;

                uiItemData.idItem = item.idItem;
                uiItemData.nameItem = item.nameItem;
                uiItemData.slotTypeParent = SlotType.SlotNpcItem;

                Sprite icon = inventoryItemPresent.GetItemIconByID(item.idItem);
                if (icon != null)
                {
                    uiItemData.itemIconImage.sprite = icon;
                }

                uiItemData.UpdateDataUI(itemClass);
            }
            else
            {
                Debug.LogWarning($"No matching UI prefab found for item ID: {item.idItem}");
            }
        }

        // Step 5: Populate the NPC trade UI with items from `listNpcItemWaitforTrade`
        for (int i = 0; i < listNpcItemWaitforTrade.Count; i++)
        {
            if (i < listNpcItemWaitforTradeUI.Count)
            {
                // Create a UI item for each NPC trade item
                CreateUIItem(listNpcItemWaitforTrade[i], listNpcItemWaitforTradeUI[i]);
            }
        }

        // Step 6: Populate the Player trade UI with items from `listPlayerItemWaitforTrade`
        for (int i = 0; i < listPlayerItemWaitforTrade.Count; i++)
        {
            if (i < listPlayerItemWaitforTradeUI.Count)
            {
                // Create a UI item for each Player trade item
                CreateUIItem(listPlayerItemWaitforTrade[i], listPlayerItemWaitforTradeUI[i]);
            }
        }
        UpdateTradeStatus();
        ConfirmButton.onClick.AddListener(Confirmtrade);
    }

    public GameObject CreateUIItem(ItemData itemData, InvenrotySlots invenrotySlots)
    {
        // Get the UI prefab matching the item's ID
        GameObject itemUIPrefab = inventoryItemPresent.listUIItemPrefab
            .FirstOrDefault(prefab => prefab.idItem == itemData.idItem)?.gameObject;

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

    public void UpdateTradeLists(SlotType slotType, ItemData itemData, int quantity)
    {
        if (slotType == SlotType.SlotNpcTrade)
        {
            ModifyTradeList(listNpcItemWaitforTrade, itemData, quantity);
        }
        else if (slotType == SlotType.SlotPlayerTrade)
        {
            ModifyTradeList(listPlayerItemWaitforTrade, itemData, quantity);
        }
        RefreshTrade();
    }

    private void ModifyTradeList(List<ItemData> tradeList, ItemData itemData, int quantity)
    {
        var existingItem = tradeList.FirstOrDefault(item => item.idItem == itemData.idItem);

        if (existingItem != null)
        {
            existingItem.count += quantity;

            if (existingItem.count <= 0)
            {
                tradeList.Remove(existingItem);
            }
        }
        else if (quantity > 0)
        {
            tradeList.Add(new ItemData
            {
                idItem = itemData.idItem,
                nameItem = itemData.nameItem,
                count = quantity,
                maxCount = itemData.maxCount,
                itemtype = itemData.itemtype,
            });
        }
    }

    private void UpdateTradeStatus()
    {
        float npcTradeValue = 0;
        float playerTradeValue = 0;

        // Calculate trade value for NPC slots
        foreach (var slot in listNpcItemWaitforTradeUI)
        {
            if (slot.transform.childCount > 0)
            {
                // Get the first child in the slot
                Transform firstChild = slot.transform.GetChild(0);
                ItemClass itemClass = firstChild.GetComponent<ItemClass>();
                if (itemClass != null)
                {
                    npcTradeValue += itemClass.tradeValueItem * itemClass.quantityItem;
                    Debug.Log($"NPC Slot Trade Value: {itemClass.tradeValueItem} * {itemClass.quantityItem} = {itemClass.tradeValueItem * itemClass.quantityItem}");
                }
            }
        }

        // Calculate trade value for Player slots
        foreach (var slot in listPlayerItemWaitforTradeUI)
        {
            if (slot.transform.childCount > 0)
            {
                // Get the first child in the slot
                Transform firstChild = slot.transform.GetChild(0);
                ItemClass itemClass = firstChild.GetComponent<ItemClass>();
                if (itemClass != null)
                {
                    playerTradeValue += itemClass.tradeValueItem * itemClass.quantityItem;
                    Debug.Log($"Player Slot Trade Value: {itemClass.tradeValueItem} * {itemClass.quantityItem} = {itemClass.tradeValueItem * itemClass.quantityItem}");
                }
            }
        }

        // Divide player trade value by 2 as per the original logic
        playerTradeValue *= 0.7f;
        Debug.Log($"Total NPC Trade Value: {npcTradeValue}, Total Player Trade Value: {playerTradeValue}");

        // Default text handling when no items are present
        if (playerTradeValue == 0 && npcTradeValue == 0)
        {
            statusTrade.text = "What can you offer me?";
            Confirmobject.SetActive(false);
            return;
        }
       if (playerTradeValue == 0 && npcTradeValue >= 1)
        {
            statusTrade.text = npcTradeValue >= 50 
                ? "You're asking for a fortune for nothing. Be serious." 
                : npcTradeValue >= 30
                ? "That's quite an ask for nothing. Add something." 
                : "Nothing is free, you know. Bring something to the table.";
            Confirmobject.SetActive(false);
            return;
        }
        else if (playerTradeValue >= 1 && npcTradeValue == 0)
        {
            statusTrade.text = playerTradeValue >= 45 
                ? "This is really interesting—what exactly are you looking for in return?"
                : playerTradeValue >= 30
                ? "This is a generous offer, I can give you alot from that"
                : "Well, what are you looking for in return for that?";
            Confirmobject.SetActive(true);
            return;
        }

        // Update status and confirm button
        if (playerTradeValue >= npcTradeValue)
        {
            Confirmobject.SetActive(true);
            statusTrade.text = playerTradeValue - npcTradeValue >= 30 
                ? "This is unbelievably generous. Are you sure about this?" 
                : playerTradeValue - npcTradeValue >= 15
                ? "You're offering a lot. I really appreciate it!" 
                : "This works for me. Let's make the trade.";
        }
        else
        {
            Confirmobject.SetActive(false);
            statusTrade.text = npcTradeValue - playerTradeValue  >= 30 
                ? "Your offer is way too low for what I'm asking. Be realistic."
                : npcTradeValue - playerTradeValue  >= 15
                ? "You’re asking quite a bit, and your offer doesn’t match up."
                : "You're close, but I need a little more to make this fair.";
        }


    }

    private void ClearAllTradeUI()
    {
        // Clear NPC trade slots
        foreach (var slot in listNpcItemWaitforTradeUI)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Clear Player trade slots
        foreach (var slot in listPlayerItemWaitforTradeUI)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Clear the main items container
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Clear the trade data lists
        listNpcItemWaitforTrade.Clear();
        listPlayerItemWaitforTrade.Clear();
    }

    public void Confirmtrade()
    {
        // Clear NPC trade slots
        foreach (var slot in listNpcItemWaitforTradeUI)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Clear Player trade slots
        foreach (var slot in listPlayerItemWaitforTradeUI)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Transfer NPC trade items to player's inventory
        foreach (var itemData in listNpcItemWaitforTrade)
        {
            uIInventoryEX.listItemDataInventorySlot.Add(itemData);
        }

        // Clear the trade lists
        listNpcItemWaitforTrade.Clear();
        listPlayerItemWaitforTrade.Clear();

        uIInventoryEX.RefreshUIInventory();
        RefreshTrade();
        Debug.Log("Trade Confirmed!");

        // Optionally, close the Trade UI after confirmation
        CloseTradeUI();
    }
}
