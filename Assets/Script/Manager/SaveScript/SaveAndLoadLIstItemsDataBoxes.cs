using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
        }

        if (inventoryItemPresent == null && gameManager != null)
        {
            inventoryItemPresent = gameManager.inventoryItemPresent;
        }

        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = FindFirstObjectByType<InventoryItemPresent>();
        }

        if (dailyGive == null && gameManager != null)
        {
            dailyGive = gameManager.dailyGive;
        }

        if (dailyGive == null)
        {
            dailyGive = FindFirstObjectByType<DailyGive>();
        }
    }

    public void SaveListItemsDataBoxesAndDailyGive()
    {
        EnsureDependencies();

        if (dataCollentListItemsBoxes == null)
        {
            dataCollentListItemsBoxes = new DataCollentListItemsBoxes();
        }

        if (inventoryItemPresent != null)
        {
            dataCollentListItemsBoxes.listItemBoxes = inventoryItemPresent.listItemsDataBox;
        }

        if (dailyGive != null)
        {
            dataCollentListItemsBoxes.listItemsDailyGive = dailyGive.listItemsTogiveDaily;
        }

        string json = JsonUtility.ToJson(dataCollentListItemsBoxes, true);
        File.WriteAllText(savePathDataListItemsBoxes, json);
    }

    public void LoadDataListItemDataBoxesAndDailyGive()
    {
        EnsureDependencies();

        if (File.Exists(savePathDataListItemsBoxes))
        {
            string json = File.ReadAllText(savePathDataListItemsBoxes);
            dataCollentListItemsBoxes = JsonUtility.FromJson<DataCollentListItemsBoxes>(json);

            if (dataCollentListItemsBoxes != null)
            {
                if (inventoryItemPresent != null && dataCollentListItemsBoxes.listItemBoxes != null)
                {
                    inventoryItemPresent.listItemsDataBox = dataCollentListItemsBoxes.listItemBoxes;
                }

                if (dailyGive != null && dataCollentListItemsBoxes.listItemsDailyGive != null)
                {
                    dailyGive.listItemsTogiveDaily = dataCollentListItemsBoxes.listItemsDailyGive;
                }
            }
        }
        else
        {
            dataCollentListItemsBoxes = new DataCollentListItemsBoxes();
        }
    }

    public void ResetDataListItemBoxesAndDailyGive()
    {
        dataCollentListItemsBoxes = new DataCollentListItemsBoxes();
        string json = JsonUtility.ToJson(dataCollentListItemsBoxes, true);
        File.WriteAllText(savePathDataListItemsBoxes, json);
    }
}

[Serializable]
public class DataCollentListItemsBoxes
{
    public List<ItemData> listItemBoxes = new List<ItemData>();
    public List<ItemData> listItemsDailyGive = new List<ItemData>();
}
