using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
public class SaveAndLoadLIstItemsDataBoxesAndDailyGive : MonoBehaviour
{
    public DataCollentListItemsBoxes dataCollentListItemsBoxes;
    public GameManager gameManager;
    public InventoryItemPresent inventoryItemPresent;
    public DailyGive dailyGive;
    [SerializeField] private string savePathDataListItemsBoxes;
    private void Start()
    {
        savePathDataListItemsBoxes = Path.Combine(Application.dataPath, "dataListItemsDataBoxes.json");
        gameManager = FindObjectOfType<GameManager>();
        inventoryItemPresent = gameManager.inventoryItemPresent;
        dailyGive = gameManager.dailyGive;
    }
    public void SaveListItemsDataBoxesAndDailyGive()
    {
        dataCollentListItemsBoxes.listItemBoxes = inventoryItemPresent.listItemsDataBox;
        dataCollentListItemsBoxes.listItemsDailyGive = dailyGive.listItemsTogiveDaily;
        string json = JsonUtility.ToJson(dataCollentListItemsBoxes,true);
        File.WriteAllText(savePathDataListItemsBoxes,json);
    }
    public void LoadDataListItemDataBoxesAndDailyGive()
    {
        if(File.Exists(savePathDataListItemsBoxes))
        {
            string json = File.ReadAllText(savePathDataListItemsBoxes);
            dataCollentListItemsBoxes = JsonUtility.FromJson<DataCollentListItemsBoxes>(json);
            inventoryItemPresent.listItemsDataBox = dataCollentListItemsBoxes.listItemBoxes;
        }
        else 
        {
            dataCollentListItemsBoxes = new DataCollentListItemsBoxes();
        }
    }
    public void ResetDataListItemBoxesAndDailyGive()
    {
        dataCollentListItemsBoxes = new DataCollentListItemsBoxes();
        string json = JsonUtility.ToJson(dataCollentListItemsBoxes,true);
        File.WriteAllText(savePathDataListItemsBoxes,json);
    }
}
[Serializable]
public class DataCollentListItemsBoxes
{
    public List<ItemData> listItemBoxes;
    public List<ItemData> listItemsDailyGive;
}
