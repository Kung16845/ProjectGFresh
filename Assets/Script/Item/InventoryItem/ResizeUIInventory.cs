using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeUIInventory : MonoBehaviour
{
    public GameObject RightBookInventoryUI;
    public GameObject CarInventoryUI;
    public RectTransform rectTransformUIInventory;


    // Update is called once per frame
    void Update()
    {
        
        if (RightBookInventoryUI.activeSelf || (CarInventoryUI != null && CarInventoryUI.activeSelf)) 
        {
            Vector2 size = rectTransformUIInventory.sizeDelta;
            size.x = 1600;
            rectTransformUIInventory.sizeDelta = size;
        }
        else 
        {
            Vector2 size = rectTransformUIInventory.sizeDelta;
            size.x = 800;
            rectTransformUIInventory.sizeDelta = size;
        }
    }
}