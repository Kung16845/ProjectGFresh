using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SlotType uITypeItem;
    public Transform parentAfterDray;
    public Transform parentBeforeDray;
    public Image imageItem;
    public ItemClass itemClass;

    private InventoryItemPresent inventoryItemPresent;
    private UIInventory uIInventory;

    private void Start()
    {
        if (imageItem == null)
        {
            imageItem = GetComponent<Image>() ?? GetComponentInChildren<Image>();
        }

        if (itemClass == null)
        {
            itemClass = GetComponent<ItemClass>();
        }

        inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        uIInventory = FindFirstObjectByType<UIInventory>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.parent == null) return;

        parentAfterDray = transform.parent;
        parentBeforeDray = parentAfterDray;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        if (imageItem == null)
        {
            imageItem = GetComponent<Image>() ?? GetComponentInChildren<Image>();
        }

        if (imageItem != null)
        {
            imageItem.raycastTarget = false;
        }

        inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        if (uIInventory == null)
        {
            uIInventory = FindFirstObjectByType<UIInventory>();
        }

        if (itemClass == null)
        {
            itemClass = GetComponent<ItemClass>();
        }

        if (itemClass is ItemWeapon weapon)
        {
            Ammotype weaponAmmoType = weapon.ammoType;
            HighlightItem(transform, Color.yellow);

            if (inventoryItemPresent != null)
            {
                inventoryItemPresent.HighlightAmmoItems(weaponAmmoType);
            }

            if (uIInventory != null)
            {
                uIInventory.HighlightItemsInSlotsUI(weaponAmmoType);
            }
        }
    }

    private void HighlightItem(Transform itemTransform, Color highlightColor)
    {
        if (itemTransform == null) return;
        Image itemImage = itemTransform.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.color = highlightColor;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Transform targetParent = parentAfterDray != null ? parentAfterDray : parentBeforeDray;
        if (targetParent != null)
        {
            transform.SetParent(targetParent);
        }

        if (imageItem != null)
        {
            imageItem.raycastTarget = true;
        }

        if (inventoryItemPresent != null)
        {
            inventoryItemPresent.ResetAmmoHighlighting();
        }

        Image itemImage = GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.color = Color.white;
        }

        if (uIInventory != null)
        {
            uIInventory.ResetHighlightInSlotsUI();
        }
    }
}
