using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
        }

        if (craftManager == null && gameManager != null)
        {
            craftManager = gameManager.craftManager;
        }

        if (craftManager == null)
        {
            craftManager = FindFirstObjectByType<CraftManager>();
        }
    }

    public void SaveDataCraftItems()
    {
        EnsureDependencies();
        AddDataCraftItems();
        string json = JsonUtility.ToJson(dataCollentCraftItems, true);
        File.WriteAllText(savePathDataCraftItmes, json);
    }

    public void AddDataCraftItems()
    {
        EnsureDependencies();
        dataCollentCraftItems = new DataCollentCraftItems
        {
            listActiveCraftingJobs = new List<DataItemsCraft>(),
            listChemicalactiveJobs = new List<DataItemsCraft>(),
            listMedicineactiveJobs = new List<DataItemsCraft>(),
            listCrafttingMoonShine = new List<DataItemsCraft>()
        };

        if (craftManager == null) return;

        if (craftManager.activeCraftingJobs != null)
        {
            for (int i = 0; i < craftManager.activeCraftingJobs.Count; i++)
            {
                var job = craftManager.activeCraftingJobs[i];
                if (job != null && job.craftingItem != null)
                {
                    dataCollentCraftItems.listActiveCraftingJobs.Add(ConventDataCraftingJobToDataItemCraft(job));
                }
            }
        }

        if (craftManager.ChemicalactiveJobs != null)
        {
            for (int i = 0; i < craftManager.ChemicalactiveJobs.Count; i++)
            {
                var job = craftManager.ChemicalactiveJobs[i];
                if (job != null && job.craftingItem != null)
                {
                    dataCollentCraftItems.listChemicalactiveJobs.Add(ConventDataCraftingJobToDataItemCraft(job));
                }
            }
        }

        if (craftManager.MedicineactiveJobs != null)
        {
            for (int i = 0; i < craftManager.MedicineactiveJobs.Count; i++)
            {
                var job = craftManager.MedicineactiveJobs[i];
                if (job != null && job.craftingItem != null)
                {
                    dataCollentCraftItems.listMedicineactiveJobs.Add(ConventDataCraftingJobToDataItemCraft(job));
                }
            }
        }

        if (craftManager.MoonshienactiveJobs != null)
        {
            for (int i = 0; i < craftManager.MoonshienactiveJobs.Count; i++)
            {
                var job = craftManager.MoonshienactiveJobs[i];
                if (job != null && job.craftingItem != null)
                {
                    dataCollentCraftItems.listCrafttingMoonShine.Add(ConventDataCraftingJobToDataItemCraft(job));
                }
            }
        }
    }

    public DataItemsCraft ConventDataCraftingJobToDataItemCraft(CraftingJob itemsCraftingJob)
    {
        if (itemsCraftingJob == null || itemsCraftingJob.craftingItem == null) return null;

        DataItemsCraft dataItemsCraft = new DataItemsCraft
        {
            idItem = itemsCraftingJob.craftingItem.itemID,
            timeRemaining = itemsCraftingJob.timeRemaining,
            isComplete = itemsCraftingJob.isComplete,
            numCraftingSource = (int)itemsCraftingJob.source
        };

        return dataItemsCraft;
    }

    public void LoadDataCraftItems()
    {
        EnsureDependencies();

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
            if (dataCollentCraftItems.listChemicalactiveJobs == null)
            {
                dataCollentCraftItems.listChemicalactiveJobs = new List<DataItemsCraft>();
            }
            if (dataCollentCraftItems.listMedicineactiveJobs == null)
            {
                dataCollentCraftItems.listMedicineactiveJobs = new List<DataItemsCraft>();
            }
            if (dataCollentCraftItems.listCrafttingMoonShine == null)
            {
                dataCollentCraftItems.listCrafttingMoonShine = new List<DataItemsCraft>();
            }

            if (craftManager != null)
            {
                // Clear existing runtime jobs to avoid duplicates on load
                craftManager.activeCraftingJobs?.Clear();
                craftManager.ChemicalactiveJobs?.Clear();
                craftManager.MedicineactiveJobs?.Clear();
                craftManager.MoonshienactiveJobs?.Clear();

                // Workshop -> activeCraftingJobs
                for (int i = 0; i < dataCollentCraftItems.listActiveCraftingJobs.Count; i++)
                {
                    DataItemsCraft dataItemCraft = dataCollentCraftItems.listActiveCraftingJobs[i];
                    if (dataItemCraft == null) continue;
                    CraftingItem item = FindCraftingItem(listCrafttingWorkShop, dataItemCraft.idItem);
                    if (item != null && craftManager.activeCraftingJobs != null)
                    {
                        craftManager.activeCraftingJobs.Add(ConventDataItemCraftToDataCraftingJob(dataItemCraft, item));
                    }
                }

                // Chemical -> ChemicalactiveJobs
                for (int i = 0; i < dataCollentCraftItems.listChemicalactiveJobs.Count; i++)
                {
                    DataItemsCraft dataItemCraft = dataCollentCraftItems.listChemicalactiveJobs[i];
                    if (dataItemCraft == null) continue;
                    CraftingItem item = FindCraftingItem(listCrafttingChemical, dataItemCraft.idItem);
                    if (item != null && craftManager.ChemicalactiveJobs != null)
                    {
                        craftManager.ChemicalactiveJobs.Add(ConventDataItemCraftToDataCraftingJob(dataItemCraft, item));
                    }
                }

                // Medicine -> MedicineactiveJobs
                for (int i = 0; i < dataCollentCraftItems.listMedicineactiveJobs.Count; i++)
                {
                    DataItemsCraft dataItemCraft = dataCollentCraftItems.listMedicineactiveJobs[i];
                    if (dataItemCraft == null) continue;
                    CraftingItem item = FindCraftingItem(listCrafttingMedicine, dataItemCraft.idItem);
                    if (item != null && craftManager.MedicineactiveJobs != null)
                    {
                        craftManager.MedicineactiveJobs.Add(ConventDataItemCraftToDataCraftingJob(dataItemCraft, item));
                    }
                }

                // Moonshine -> MoonshienactiveJobs
                for (int i = 0; i < dataCollentCraftItems.listCrafttingMoonShine.Count; i++)
                {
                    DataItemsCraft dataItemCraft = dataCollentCraftItems.listCrafttingMoonShine[i];
                    if (dataItemCraft == null) continue;
                    CraftingItem item = FindCraftingItem(listCrafttingMoonShine, dataItemCraft.idItem);
                    if (item != null && craftManager.MoonshienactiveJobs != null)
                    {
                        craftManager.MoonshienactiveJobs.Add(ConventDataItemCraftToDataCraftingJob(dataItemCraft, item));
                    }
                }
            }
        }
        else
        {
            ResetDataCraftItemsInMemory();
        }
    }

    private CraftingItem FindCraftingItem(List<CraftingItem> list, int id)
    {
        if (list == null) return null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].itemID == id)
            {
                return list[i];
            }
        }
        return null;
    }

    public CraftingJob ConventDataItemCraftToDataCraftingJob(DataItemsCraft dataItemsCraft, CraftingItem newCraftingItem)
    {
        return new CraftingJob(newCraftingItem, dataItemsCraft.timeRemaining, (CraftingSource)dataItemsCraft.numCraftingSource);
    }

    private void ResetDataCraftItemsInMemory()
    {
        dataCollentCraftItems = new DataCollentCraftItems
        {
            listActiveCraftingJobs = new List<DataItemsCraft>(),
            listMedicineactiveJobs = new List<DataItemsCraft>(),
            listChemicalactiveJobs = new List<DataItemsCraft>(),
            listCrafttingMoonShine = new List<DataItemsCraft>()
        };
    }

    public void ResetDataCraftItems()
    {
        ResetDataCraftItemsInMemory();
        string json = JsonUtility.ToJson(dataCollentCraftItems, true);
        File.WriteAllText(savePathDataCraftItmes, json);
    }
}

[Serializable]
public class DataCollentCraftItems
{
    public List<DataItemsCraft> listActiveCraftingJobs = new List<DataItemsCraft>();
    public List<DataItemsCraft> listChemicalactiveJobs = new List<DataItemsCraft>();
    public List<DataItemsCraft> listMedicineactiveJobs = new List<DataItemsCraft>();
    public List<DataItemsCraft> listCrafttingMoonShine = new List<DataItemsCraft>();
}

[Serializable]
public class DataItemsCraft
{
    public int idItem;
    public float timeRemaining;
    public bool isComplete;
    public int numCraftingSource;
}
