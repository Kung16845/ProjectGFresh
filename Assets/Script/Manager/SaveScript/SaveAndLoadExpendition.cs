using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class SaveAndLoadExpendition : MonoBehaviour
{
    public DataCollentUIEX dataCollentUIEX;
    public Transform transformParentUIEx;
    public GameManager gameManager;
    [SerializeField] private string savePathDataExpendition;
    private void Awake()
    {
        savePathDataExpendition = Path.Combine(Application.dataPath, "dataExpendition.json");
        gameManager = FindObjectOfType<GameManager>();
    }
    public void SaveUIExpemdition()
    {
        transformParentUIEx = gameManager.expenditionManager.transformsUIEx;
        AddDataBeforeSaveToJaon();
        string json = JsonUtility.ToJson(dataCollentUIEX, true);
        File.WriteAllText(savePathDataExpendition, json);
    }
    public void AddDataBeforeSaveToJaon()
    {
        if (transformParentUIEx.childCount == 0)
        {
            Debug.Log("Child count = 0");
            return;
        }

        UIInventoryEX[] listUIEX = transformParentUIEx.GetComponentsInChildren<UIInventoryEX>(true);

        dataCollentUIEX = new DataCollentUIEX();
        
        foreach (UIInventoryEX uIEx in listUIEX)
        {
            DataSaveExpendition dataExpenditionSave = new DataSaveExpendition();

            dataExpenditionSave.idNPCExpendition = uIEx.npcSelecting.idnpc;

            dataExpenditionSave.listItemDataInventoryEqicment = uIEx.listItemDataInventoryEquipment;
            dataExpenditionSave.listItemDataInventorySlot = uIEx.listItemDataInventorySlot;
            dataExpenditionSave.listItemDataInventoryCar = uIEx.listItemDataCarInventorySlot;

            dataExpenditionSave.timeScale = uIEx.timeScale;
            dataExpenditionSave.riskEventValue = uIEx.riskValue;

            dataExpenditionSave.indexButtonExpendition = uIEx.indexButtonExpendition;
            dataExpenditionSave.indexSceneExpendition = uIEx.indexSceneExpendition;

            dataExpenditionSave.isUseTunnel =  uIEx.isuseTunnel;
            dataExpenditionSave.isUseCar = uIEx.isuseCar;
            dataExpenditionSave.isWalk = uIEx.iswalk;
            dataExpenditionSave.istraveling = uIEx.istraveling;
            dataExpenditionSave.isArriveEx = uIEx.isArriveEx;       
            dataExpenditionSave.isArriveHome = uIEx.isArriveHome;
            dataExpenditionSave.isExpenditon = uIEx.isExpenditon;

            dataExpenditionSave.finishDayCraftingTime = uIEx.finishDayCraftingTime;
            dataExpenditionSave.finishHourCraftingTime = uIEx.finishHourCraftingTime;
            dataExpenditionSave.finishMinutesCraftingTime = uIEx.finishMinutesCraftingTime;

            dataCollentUIEX.listdataUIExpemdition.Add(dataExpenditionSave);
        }

    }
    public void LoadDataUIExFromJsonToScriptData()
    {
        transformParentUIEx = gameManager.expenditionManager.transformsUIEx;
        if (File.Exists(savePathDataExpendition))
        {
            string json = File.ReadAllText(savePathDataExpendition);
            dataCollentUIEX = JsonUtility.FromJson<DataCollentUIEX>(json);
            Debug.Log($"Data loaded from {savePathDataExpendition}");

            foreach (DataSaveExpendition dataUIEX in dataCollentUIEX.listdataUIExpemdition)
            {
                CreateUIEX(dataUIEX);
            }
        }
        else
        {
            dataCollentUIEX = new DataCollentUIEX();
            Debug.Log("No data file found. Created new data collection.");
        }

    }
    public void CreateUIEX(DataSaveExpendition dataSaveExpendition)
    {
        ExpenditionManager expenditionManager = gameManager.expenditionManager;
        NpcManager npcManager = gameManager.npcManager;

        GameObject uIEx = Instantiate(expenditionManager.uIInventoryExPrefab, transformParentUIEx);

        UIInventoryEX newUIInventoryEX = uIEx.GetComponent<UIInventoryEX>();
        newUIInventoryEX.uIBoxesInventory.SetActive(false);
        newUIInventoryEX.uINpcSending.SetActive(true);
        newUIInventoryEX.gameObject.SetActive(false);

        NpcClass npcSentEx = npcManager.GetNpcById(dataSaveExpendition.idNPCExpendition);

        newUIInventoryEX.npcSelecting = npcSentEx;

        newUIInventoryEX.listItemDataInventoryEquipment = dataSaveExpendition.listItemDataInventoryEqicment;
        newUIInventoryEX.listItemDataInventorySlot = dataSaveExpendition.listItemDataInventorySlot;
        newUIInventoryEX.listItemDataCarInventorySlot = dataSaveExpendition.listItemDataInventoryCar;

        newUIInventoryEX.timeScale = dataSaveExpendition.timeScale;
        newUIInventoryEX.riskValue = dataSaveExpendition.riskEventValue;

        newUIInventoryEX.indexButtonExpendition = dataSaveExpendition.indexButtonExpendition;
        newUIInventoryEX.indexSceneExpendition = dataSaveExpendition.indexSceneExpendition;

        newUIInventoryEX.isuseCar = dataSaveExpendition.isUseCar;
        newUIInventoryEX.isuseTunnel = dataSaveExpendition.isUseTunnel;
        newUIInventoryEX.iswalk = dataSaveExpendition.isWalk;
        newUIInventoryEX.istraveling = dataSaveExpendition.istraveling;
        newUIInventoryEX.isArriveEx = dataSaveExpendition.isArriveEx;
        newUIInventoryEX.isArriveHome = dataSaveExpendition.isArriveHome;
        newUIInventoryEX.isExpenditon = dataSaveExpendition.isExpenditon;

        newUIInventoryEX.finishDayCraftingTime = dataSaveExpendition.finishDayCraftingTime;
        newUIInventoryEX.finishHourCraftingTime = dataSaveExpendition.finishHourCraftingTime;
        newUIInventoryEX.finishMinutesCraftingTime = dataSaveExpendition.finishMinutesCraftingTime;

        newUIInventoryEX.npcManager = npcManager;
        newUIInventoryEX.expenditionManager = expenditionManager;
        newUIInventoryEX.globalstat = gameManager.globalstat;
        newUIInventoryEX.sceneSystem = FindObjectOfType<SceneSystem>();

        CountdownTimeDay countdownTimeDay = expenditionManager.AddComponent<CountdownTimeDay>();
        countdownTimeDay.timeScale = newUIInventoryEX.timeScale;
        countdownTimeDay.uIInventoryEX = newUIInventoryEX;
        countdownTimeDay.timeManager = gameManager.timeManager;
        countdownTimeDay.finishDayCraftingTime = newUIInventoryEX.finishDayCraftingTime;
        countdownTimeDay.finishHourCraftingTime = newUIInventoryEX.finishHourCraftingTime;
        countdownTimeDay.finishMinutesCraftingTime = newUIInventoryEX.finishMinutesCraftingTime;

        newUIInventoryEX.SetUIExButton(countdownTimeDay);
        // newUIInventoryEX.SetUIExGameObjectInExScript();
    }
    public void ResetDataUIEX()
    {
        dataCollentUIEX = new DataCollentUIEX();
        string json = JsonUtility.ToJson(dataCollentUIEX, true);
        File.WriteAllText(savePathDataExpendition, json);
    }
}
[Serializable]
public class DataCollentUIEX
{
    public List<DataSaveExpendition> listdataUIExpemdition;
}
[Serializable]
public class DataSaveExpendition
{
    public int idNPCExpendition;
    public List<ItemData> listItemDataInventoryEqicment;
    public List<ItemData> listItemDataInventorySlot;
    public List<ItemData> listItemDataInventoryCar;
    public float timeScale;
    public float riskEventValue;
    public int indexButtonExpendition;
    public int indexSceneExpendition;
    public bool isUseCar;
    public bool isUseTunnel;
    public bool isWalk;
    public bool istraveling;
    public bool isArriveEx;
    public bool isArriveHome;
    public bool isExpenditon;
    public int finishDayCraftingTime;
    public int finishHourCraftingTime;
    public int finishMinutesCraftingTime;
}
