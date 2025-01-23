using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class SaveAndLoadCraftItms : MonoBehaviour
{
    public GameManager gameManager;
    public CraftManager craftManager;
    public List<CraftingItem> listCrafttingWorkShop = new List<CraftingItem>();
    public List<CraftingItem> listCrafttingChemical = new List<CraftingItem>();
    public List<CraftingItem> listCrafttingMedicine = new List<CraftingItem>();
    public List<CraftingItem> listCrafttingMoonShine = new List<CraftingItem>();
    public DataCollentCraftItems dataCollentCraftItems;
    [SerializeField] private string savePathDataCraftItmes;
    private void Start()
    {
        savePathDataCraftItmes = Path.Combine(Application.dataPath, "dataCraftItms.json");
        gameManager = FindObjectOfType<GameManager>();
        craftManager = gameManager.craftManager;
    }
    public void SaveDataCraftItems()
    {
        AddDataCraftItems();
        string json = JsonUtility.ToJson(dataCollentCraftItems, true);
        File.WriteAllText(savePathDataCraftItmes, json);
    }
    public void AddDataCraftItems()
    {
        dataCollentCraftItems = new DataCollentCraftItems();

        foreach (CraftingJob itemCraftJobs in craftManager.activeCraftingJobs)
        {

            dataCollentCraftItems.listActiveCraftingJobs.Add(ConventDataCraftingJobToDataItemCraft(itemCraftJobs));
        }

        foreach (CraftingJob itemChemical in craftManager.ChemicalactiveJobs)
        {

            dataCollentCraftItems.listChemicalactiveJobs.Add(ConventDataCraftingJobToDataItemCraft(itemChemical));
        }

        foreach (CraftingJob itemMedicine in craftManager.MedicineactiveJobs)
        {

            dataCollentCraftItems.listMedicineactiveJobs.Add(ConventDataCraftingJobToDataItemCraft(itemMedicine));
        }

        foreach (CraftingJob itemMedicine in craftManager.MedicineactiveJobs)
        {

            dataCollentCraftItems.listMedicineactiveJobs.Add(ConventDataCraftingJobToDataItemCraft(itemMedicine));
        }
    }
    public DataItemsCraft ConventDataCraftingJobToDataItemCraft(CraftingJob itemsCraftingJob)
    {
        DataItemsCraft dataItemsCraft = new DataItemsCraft();

        dataItemsCraft.idItem = itemsCraftingJob.craftingItem.itemID;
        dataItemsCraft.timeRemaining = itemsCraftingJob.timeRemaining;
        dataItemsCraft.isComplete = itemsCraftingJob.isComplete;

        dataItemsCraft.numCraftingSource = (int)itemsCraftingJob.source;

        return dataItemsCraft;
    }
    public void LoadDataCraftItems()
    {
        if (File.Exists(savePathDataCraftItmes))
        {
            string json = File.ReadAllText(savePathDataCraftItmes);
            dataCollentCraftItems = JsonUtility.FromJson<DataCollentCraftItems>(json);

            if (dataCollentCraftItems == null)
            {
                dataCollentCraftItems = new DataCollentCraftItems();
            }
            if (dataCollentCraftItems.listActiveCraftingJobs == null)
            {
                dataCollentCraftItems.listActiveCraftingJobs = new List<DataItemsCraft>();
            }
            if (dataCollentCraftItems.listMedicineactiveJobs == null)
            {
                dataCollentCraftItems.listMedicineactiveJobs = new List<DataItemsCraft>();
            }
            if (dataCollentCraftItems.listChemicalactiveJobs == null)
            {
                dataCollentCraftItems.listChemicalactiveJobs = new List<DataItemsCraft>();
            }

            if (dataCollentCraftItems.listActiveCraftingJobs.Count > 0)
            {
                foreach (DataItemsCraft dataItemCraft in dataCollentCraftItems.listActiveCraftingJobs)
                {
                    CraftingItem newCraftingItem = new CraftingItem();
                    newCraftingItem = listCrafttingWorkShop.FirstOrDefault(item => item.itemID == dataItemCraft.idItem);

                    craftManager.MedicineactiveJobs.Add(ConventDataItemCraftToDataCraftingJob(dataItemCraft, newCraftingItem));
                }
            }

            if (dataCollentCraftItems.listChemicalactiveJobs.Count > 0)
            {
                foreach (DataItemsCraft dataItemCraft in dataCollentCraftItems.listChemicalactiveJobs)
                {
                    CraftingItem newCraftingItem = new CraftingItem();
                    newCraftingItem = listCrafttingChemical.FirstOrDefault(item => item.itemID == dataItemCraft.idItem);

                    craftManager.MedicineactiveJobs.Add(ConventDataItemCraftToDataCraftingJob(dataItemCraft, newCraftingItem));
                }
            }

            if (dataCollentCraftItems.listMedicineactiveJobs.Count > 0)
            {
                foreach (DataItemsCraft dataItemCraft in dataCollentCraftItems.listMedicineactiveJobs)
                {
                    CraftingItem newCraftingItem = new CraftingItem();
                    newCraftingItem = listCrafttingMedicine.FirstOrDefault(item => item.itemID == dataItemCraft.idItem);

                    craftManager.MedicineactiveJobs.Add(ConventDataItemCraftToDataCraftingJob(dataItemCraft, newCraftingItem));
                }
            }
            if(dataCollentCraftItems.listCrafttingMoonShine.Count > 0)
            {
                foreach (DataItemsCraft dataItemCraft in dataCollentCraftItems.listCrafttingMoonShine)
                {
                    CraftingItem newCraftingItem = new CraftingItem();
                    newCraftingItem = listCrafttingMoonShine.FirstOrDefault(item => item.itemID == dataItemCraft.idItem);

                    craftManager.MedicineactiveJobs.Add(ConventDataItemCraftToDataCraftingJob(dataItemCraft, newCraftingItem));
                }
            }
        }
        else
        {
            dataCollentCraftItems = new DataCollentCraftItems();
            dataCollentCraftItems.listActiveCraftingJobs = new List<DataItemsCraft>();
            dataCollentCraftItems.listMedicineactiveJobs = new List<DataItemsCraft>();
            dataCollentCraftItems.listChemicalactiveJobs = new List<DataItemsCraft>();
        }
    }
    public CraftingJob ConventDataItemCraftToDataCraftingJob(DataItemsCraft dataItemsCraft, CraftingItem newCraftingItem)
    {

        CraftingJob newCraftingItemJob = new CraftingJob(newCraftingItem, dataItemsCraft.timeRemaining
        , (CraftingSource)dataItemsCraft.numCraftingSource);

        return newCraftingItemJob;
    }
    public void ResetDataCraftItems()
    {
        dataCollentCraftItems = new DataCollentCraftItems();
        dataCollentCraftItems.listActiveCraftingJobs = new List<DataItemsCraft>();
        dataCollentCraftItems.listMedicineactiveJobs = new List<DataItemsCraft>();
        dataCollentCraftItems.listChemicalactiveJobs = new List<DataItemsCraft>();
        dataCollentCraftItems.listCrafttingMoonShine = new List<DataItemsCraft>();
        string json = JsonUtility.ToJson(dataCollentCraftItems, true);
        File.WriteAllText(savePathDataCraftItmes, json);
    }
}
[Serializable]
public class DataCollentCraftItems
{
    public List<DataItemsCraft> listActiveCraftingJobs;
    public List<DataItemsCraft> listChemicalactiveJobs;
    public List<DataItemsCraft> listMedicineactiveJobs;
    public List<DataItemsCraft> listCrafttingMoonShine;
}
[Serializable]
public class DataItemsCraft
{
    public int idItem;
    public float timeRemaining;
    public bool isComplete;
    public int numCraftingSource;
}
