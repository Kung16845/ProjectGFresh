using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseInventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panelRoot;
    public Transform itemContainer;

    private float toggleCooldown = 0.3f;
    private float nextToggleTime = 0f;
    private InventoryItemPresent inventoryItemPresent;

    private void Start()
    {
        inventoryItemPresent = InventoryItemPresent.Instance;
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && Time.time >= nextToggleTime)
        {
            TogglePanel();
            nextToggleTime = Time.time + toggleCooldown;
        }
    }

    private void TogglePanel()
    {
        if (panelRoot == null) return;

        bool opening = !panelRoot.activeSelf;
        panelRoot.SetActive(opening);

        if (opening)
            RefreshItems();
    }

    private int currentFilter = -1; // -1 = show all

    public void RefreshItems()
    {
        if (currentFilter == -1)
            ShowAllItems();
        else
            ShowItemsByType(currentFilter);
    }

    /// <summary>
    /// Show all items (no filter). Assign to button onClick.
    /// </summary>
    public void ShowAllItems()
    {
        currentFilter = -1;
        if (inventoryItemPresent == null)
            inventoryItemPresent = InventoryItemPresent.Instance;
        if (inventoryItemPresent == null) return;

        ClearItems();

        var items = inventoryItemPresent.listItemsDataBox
            .OrderBy(item => item.itemtype)
            .ThenBy(item => item.nameItem)
            .ToList();

        foreach (ItemData itemData in items)
        {
            CreateItemUI(itemData);
        }
    }

    /// <summary>
    /// Show only items of a specific type. Assign to button onClick with int matching Itemtype enum.
    /// Weapon=0, Vest=1, Backpack=2, Tool=3, Grenade=4, Ammo=5, Pill=6, General=7
    /// </summary>
    public void ShowItemsByType(int itemTypeIndex)
    {
        currentFilter = itemTypeIndex;
        if (inventoryItemPresent == null)
            inventoryItemPresent = InventoryItemPresent.Instance;
        if (inventoryItemPresent == null) return;

        ClearItems();

        Itemtype filterType = (Itemtype)itemTypeIndex;

        var items = inventoryItemPresent.listItemsDataBox
            .Where(item => item.itemtype == filterType)
            .OrderBy(item => item.nameItem)
            .ToList();

        foreach (ItemData itemData in items)
        {
            CreateItemUI(itemData);
        }
    }

    private void CreateItemUI(ItemData itemData)
    {
        if (inventoryItemPresent == null || itemContainer == null) return;

        UIItemData prefab = inventoryItemPresent.listUIItemPrefab
            .FirstOrDefault(ui => ui.idItem == itemData.idItem);
        if (prefab == null) return;

        GameObject itemObj = Instantiate(prefab.gameObject, itemContainer, false);

        // Force RectTransform to fill the Grid Layout cell
        RectTransform rt = itemObj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
        }

        ItemClass itemClass = itemObj.GetComponent<ItemClass>();
        if (itemClass != null)
        {
            itemClass.quantityItem = itemData.count;
            itemClass.maxCountItem = itemData.maxCount;
        }

        UIItemData uiItemData = itemObj.GetComponent<UIItemData>();
        if (uiItemData != null)
        {
            uiItemData.slotTypeParent = SlotType.SlotBoxes;
            uiItemData.UpdateDataUI(itemClass);
        }

        // Disable drag on this view-only UI
        DraggableItem draggable = itemObj.GetComponent<DraggableItem>();
        if (draggable != null)
            draggable.enabled = false;
    }

    private void ClearItems()
    {
        if (itemContainer == null) return;
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
