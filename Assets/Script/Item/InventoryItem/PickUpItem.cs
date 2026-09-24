using UnityEngine;

public class PickUpItem : MonoBehaviour
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

    public void TestFuctionAddItem()
    {   
        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance ?? FindFirstObjectByType<InventoryItemPresent>();
        }

        if (inventoryItemPresent != null && itemData != null)
        {
            inventoryItemPresent.AddItem(itemData);   
            inventoryItemPresent.RefreshUIBox(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TestFuctionAddItem();
    }
}
