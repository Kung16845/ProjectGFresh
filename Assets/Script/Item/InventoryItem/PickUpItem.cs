using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{   
    public ItemData itemData;
    public InventoryItemPresent inventoryItemPresent;
    // Start is called before the first frame update
    void Start()
    {
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
    }

    public void TestFuctionAddItem()
    {   
        Debug.Log("Add ITem");  
        inventoryItemPresent.AddItem(itemData);   
        inventoryItemPresent.RefreshUIBox(); 
    }
    private void OnTriggerEnter2D(Collider2D other) {
        TestFuctionAddItem();
    }
}
