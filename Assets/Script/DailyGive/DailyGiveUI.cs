using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class DailyGiveUI : MonoBehaviour
{
    public DailyGive dailyGive;
    public Transform transformContainer;
    public TimeManager timeManager;
    public DateTime dateTime;
    public int currentDay = 0;

     public void ClearContainer()
    {
        foreach (Transform child in transformContainer)
        {
            Destroy(child.gameObject);
        }
    }
    public void CreateUIItemInContainer(ItemData itemData)
    {
        // Find the corresponding UI prefab
        GameObject uiItem = dailyGive.listUIItemPrefab.FirstOrDefault(idItem => idItem.idItem == itemData.idItem)?.gameObject;
        if (uiItem == null)
        {
            Debug.LogWarning($"No UI prefab found for item ID: {itemData.idItem}");
            return;
        }

        // Instantiate the prefab and set its parent
        GameObject uIItemOBJ = Instantiate(uiItem, transformContainer, false);

        // Scale the object by 1.5
        uIItemOBJ.transform.localScale *= 1.6f;

        // Update the UI item data
        UIItemData uIItemData = uIItemOBJ.GetComponent<UIItemData>();
        ItemClass itemClass = uIItemOBJ.GetComponent<ItemClass>();

        if (itemClass != null)
        {
            itemClass.quantityItem = itemData.count;
            itemClass.maxCountItem = itemData.maxCount;
        }

        if (uIItemData != null)
        {
            uIItemData.UpdateDataUI(itemClass);
        }
        else
        {
            Debug.LogWarning("UIItemData component not found on the instantiated prefab.");
        }
    }
}
