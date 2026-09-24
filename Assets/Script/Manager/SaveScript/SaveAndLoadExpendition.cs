using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadExpendition : MonoBehaviour
{
    public DataCollentUIEX dataCollentUIEX;
    public Transform transformParentUIEx;
    public GameManager gameManager;
    [SerializeField] private string savePathDataExpendition;

    private void Awake()
    {
        savePathDataExpendition = Path.Combine(Application.dataPath, "dataExpendition.json");
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
        }

        if (transformParentUIEx == null && gameManager != null && gameManager.expenditionManager != null)
        {
            transformParentUIEx = gameManager.expenditionManager.transformsUIEx;
        }

        if (transformParentUIEx == null && ExpenditionManager.Instance != null)
        {
            transformParentUIEx = ExpenditionManager.Instance.transformsUIEx;
        }
    }

    public void SaveUIExpemdition()
    {
        EnsureDependencies();
        AddDataBeforeSaveToJaon();
        string json = JsonUtility.ToJson(dataCollentUIEX, true);
        File.WriteAllText(savePathDataExpendition, json);
    }

    public void AddDataBeforeSaveToJaon()
    {
        dataCollentUIEX = new DataCollentUIEX();

        if (transformParentUIEx == null || transformParentUIEx.childCount == 0)
        {
            return;
        }

        UIInventoryEX[] listUIEX = transformParentUIEx.GetComponentsInChildren<UIInventoryEX>(true);

        foreach (UIInventoryEX uIEx in listUIEX)
        {
            if (uIEx == null) continue;

            DataSaveExpendition dataExpenditionSave = new DataSaveExpendition
            {
                idNPCExpendition = (uIEx.npcSelecting != null) ? uIEx.npcSelecting.idnpc : -1,
                listItemDataInventoryEqicment = uIEx.listItemDataInventoryEquipment,
                listItemDataInventorySlot = uIEx.listItemDataInventorySlot,
                listItemDataInventoryCar = uIEx.listItemDataCarInventorySlot,
                timeScale = uIEx.timeScale,
                riskEventValue = uIEx.riskValue,
                indexButtonExpendition = uIEx.indexButtonExpendition,
                indexSceneExpendition = uIEx.indexSceneExpendition,
                isUseTunnel = uIEx.isuseTunnel,
                isUseCar = uIEx.isuseCar,
                isWalk = uIEx.iswalk,
                istraveling = uIEx.istraveling,
                isArriveEx = uIEx.isArriveEx,
                isArriveHome = uIEx.isArriveHome,
                isExpenditon = uIEx.isExpenditon,
                finishDayCraftingTime = uIEx.finishDayCraftingTime,
                finishHourCraftingTime = uIEx.finishHourCraftingTime,
                finishMinutesCraftingTime = uIEx.finishMinutesCraftingTime
            };

            dataCollentUIEX.listdataUIExpemdition.Add(dataExpenditionSave);
        }
    }

    public void LoadDataUIExFromJsonToScriptData()
    {
        EnsureDependencies();

        if (File.Exists(savePathDataExpendition))
        {
            string json = File.ReadAllText(savePathDataExpendition);
            dataCollentUIEX = JsonUtility.FromJson<DataCollentUIEX>(json);

            if (dataCollentUIEX != null && dataCollentUIEX.listdataUIExpemdition != null)
            {
                for (int i = 0; i < dataCollentUIEX.listdataUIExpemdition.Count; i++)
                {
                    DataSaveExpendition dataUIEX = dataCollentUIEX.listdataUIExpemdition[i];
                    if (dataUIEX != null)
                    {
                        CreateUIEX(dataUIEX);
                    }
                }
            }
        }
        else
        {
            dataCollentUIEX = new DataCollentUIEX();
        }
    }

    public void CreateUIEX(DataSaveExpendition dataSaveExpendition)
    {
        EnsureDependencies();
        if (gameManager == null) return;

        ExpenditionManager expenditionManager = gameManager.expenditionManager;
        NpcManager npcManager = gameManager.npcManager;

        if (expenditionManager == null || expenditionManager.uIInventoryExPrefab == null || transformParentUIEx == null)
        {
            Debug.LogWarning("[SaveAndLoadExpendition] Cannot instantiate UIInventoryEx: missing references.");
            return;
        }

        GameObject uIEx = Instantiate(expenditionManager.uIInventoryExPrefab, transformParentUIEx);
        UIInventoryEX newUIInventoryEX = uIEx.GetComponent<UIInventoryEX>();
        if (newUIInventoryEX == null) return;

        if (newUIInventoryEX.uIBoxesInventory != null) newUIInventoryEX.uIBoxesInventory.SetActive(false);
        if (newUIInventoryEX.uINpcSending != null) newUIInventoryEX.uINpcSending.SetActive(true);
        newUIInventoryEX.gameObject.SetActive(false);

        NpcClass npcSentEx = (npcManager != null) ? npcManager.GetNpcById(dataSaveExpendition.idNPCExpendition) : null;
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
        newUIInventoryEX.sceneSystem = SceneSystem.Instance != null ? SceneSystem.Instance : FindFirstObjectByType<SceneSystem>();

        CountdownTimeDay countdownTimeDay = expenditionManager.gameObject.AddComponent<CountdownTimeDay>();
        countdownTimeDay.timeScale = newUIInventoryEX.timeScale;
        countdownTimeDay.uIInventoryEX = newUIInventoryEX;
        countdownTimeDay.timeManager = gameManager.timeManager;
        countdownTimeDay.finishDayCraftingTime = newUIInventoryEX.finishDayCraftingTime;
        countdownTimeDay.finishHourCraftingTime = newUIInventoryEX.finishHourCraftingTime;
        countdownTimeDay.finishMinutesCraftingTime = newUIInventoryEX.finishMinutesCraftingTime;

        newUIInventoryEX.SetUIExButton(countdownTimeDay);
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
    public List<DataSaveExpendition> listdataUIExpemdition = new List<DataSaveExpendition>();
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
