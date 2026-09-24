using UnityEngine;

public class RemoveItem : MonoBehaviour
{
    public ItemData itemData;
    public InventoryItemPresent inventoryItemPresent;

    private void Start()
    {
        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        }
    }

    public void TestFuctionRemoveItem()
    {   
        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        }

        if (inventoryItemPresent != null && itemData != null)
        {
            inventoryItemPresent.RemoveItem(itemData);   
            inventoryItemPresent.RefreshUIBox(); 
        }
    }
}
