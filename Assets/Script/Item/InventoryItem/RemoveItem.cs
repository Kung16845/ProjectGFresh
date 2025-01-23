using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveItem : MonoBehaviour
{
    public ItemData itemData;
    public InventoryItemPresent inventoryItemPresent;
    // Start is called before the first frame update
    void Start()
    {
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
    }

    public void TestFuctionRemoveItem()
    {   
        Debug.Log("Remove ITem");  
        inventoryItemPresent.RemoveItem(itemData);   
        inventoryItemPresent.RefreshUIBox(); 
    }
}
