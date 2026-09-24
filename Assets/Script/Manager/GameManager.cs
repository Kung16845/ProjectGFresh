using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Manager Game")]
    public TimeManager timeManager;
    public BuildManager buildManager;
    public InventoryItemPresent inventoryItemPresent;
    public DailyGive dailyGive;
    public ExpenditionManager expenditionManager;
    public Globalstat globalstat;
    public NpcManager npcManager;
    public ManagerSceneEX managerSceneEX;
    public OutpostSystem outpostSystem;
    public CraftManager craftManager;
    public PatienManger patienManger;

    [Header("Script Save and Load Game")]
    public SaveAndLoadTimemanager saveAndLoadTimemanager;
    public SaveAndLoadLIstItemsDataBoxesAndDailyGive saveAndLoadLIstItemsDataBoxes;
    public SaveAndLoadListNpc saveAndLoadListNpc;
    public SaveAndLoadExpendition saveAndLoadExpendition;
    public SaveDataDDA saveDataDDA;
    public SaveAndLoadListDoorStatusSceneEX saveAndLoadListDoorStatusSceneEX;
    public SaveAndLoadOutPostReward saveAndLoadOutPostReward;
    public SaveAndLoadBuildManager saveAndLoadBuildManager;
    public SaveAndLoadResoure saveAndLoadResoure;
    public SaveAndLoadCraftItms saveAndLoadCraftItms;
    public SaveAndLoadTunnutAndBroken saveAndLoadTunnutAndBroken;
    public SaveAndLoadPatint saveAndLoadPatint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        AutoCacheManagers();
    }

    private void AutoCacheManagers()
    {
        if (timeManager == null) timeManager = FindFirstObjectByType<TimeManager>();
        if (buildManager == null) buildManager = FindFirstObjectByType<BuildManager>();
        if (inventoryItemPresent == null) inventoryItemPresent = FindFirstObjectByType<InventoryItemPresent>();
        if (dailyGive == null) dailyGive = FindFirstObjectByType<DailyGive>();
        if (expenditionManager == null) expenditionManager = FindFirstObjectByType<ExpenditionManager>();
        if (globalstat == null) globalstat = FindFirstObjectByType<Globalstat>();
        if (npcManager == null) npcManager = FindFirstObjectByType<NpcManager>();
        if (managerSceneEX == null) managerSceneEX = FindFirstObjectByType<ManagerSceneEX>();
        if (outpostSystem == null) outpostSystem = FindFirstObjectByType<OutpostSystem>();
        if (craftManager == null) craftManager = FindFirstObjectByType<CraftManager>();
        if (patienManger == null) patienManger = FindFirstObjectByType<PatienManger>();

        if (saveAndLoadTimemanager == null) saveAndLoadTimemanager = GetComponentInChildren<SaveAndLoadTimemanager>();
        if (saveAndLoadLIstItemsDataBoxes == null) saveAndLoadLIstItemsDataBoxes = GetComponentInChildren<SaveAndLoadLIstItemsDataBoxesAndDailyGive>();
        if (saveAndLoadListNpc == null) saveAndLoadListNpc = GetComponentInChildren<SaveAndLoadListNpc>();
        if (saveAndLoadExpendition == null) saveAndLoadExpendition = GetComponentInChildren<SaveAndLoadExpendition>();
        if (saveDataDDA == null) saveDataDDA = GetComponentInChildren<SaveDataDDA>();
        if (saveAndLoadListDoorStatusSceneEX == null) saveAndLoadListDoorStatusSceneEX = GetComponentInChildren<SaveAndLoadListDoorStatusSceneEX>();
        if (saveAndLoadOutPostReward == null) saveAndLoadOutPostReward = GetComponentInChildren<SaveAndLoadOutPostReward>();
        if (saveAndLoadBuildManager == null) saveAndLoadBuildManager = GetComponentInChildren<SaveAndLoadBuildManager>();
        if (saveAndLoadResoure == null) saveAndLoadResoure = GetComponentInChildren<SaveAndLoadResoure>();
        if (saveAndLoadCraftItms == null) saveAndLoadCraftItms = GetComponentInChildren<SaveAndLoadCraftItms>();
        if (saveAndLoadTunnutAndBroken == null) saveAndLoadTunnutAndBroken = GetComponentInChildren<SaveAndLoadTunnutAndBroken>();
        if (saveAndLoadPatint == null) saveAndLoadPatint = GetComponentInChildren<SaveAndLoadPatint>();
    }

    public void NewGame()
    {
        if (saveAndLoadExpendition != null) saveAndLoadExpendition.ResetDataUIEX();
        if (saveAndLoadListNpc != null) saveAndLoadListNpc.ResetDataListNpc();
        if (saveAndLoadLIstItemsDataBoxes != null) saveAndLoadLIstItemsDataBoxes.ResetDataListItemBoxesAndDailyGive();
        if (saveDataDDA != null) saveDataDDA.ResetDataDDA();
        if (saveAndLoadListDoorStatusSceneEX != null) saveAndLoadListDoorStatusSceneEX.ResetDataListDoorStatus();
        if (saveAndLoadOutPostReward != null) saveAndLoadOutPostReward.ResetDataOutPostReward();
        if (saveAndLoadTimemanager != null) saveAndLoadTimemanager.ResetDataTime();
        if (saveAndLoadBuildManager != null) saveAndLoadBuildManager.ResetDataBuilding();
        if (saveAndLoadResoure != null) saveAndLoadResoure.ResetDataResoure();
        if (saveAndLoadCraftItms != null) saveAndLoadCraftItms.ResetDataCraftItems();
        if (npcManager != null) npcManager.StartGameCreateGropNpc();
        if (saveAndLoadTunnutAndBroken != null) saveAndLoadTunnutAndBroken.ResetDataTunnutAndBroken();
        if (saveAndLoadPatint != null) saveAndLoadPatint.ResetDataPatint();
    }

    public void SaveGame()
    {
        if (saveAndLoadListNpc != null) saveAndLoadListNpc.SaveListNpc();
        if (saveAndLoadLIstItemsDataBoxes != null) saveAndLoadLIstItemsDataBoxes.SaveListItemsDataBoxesAndDailyGive();
        if (saveAndLoadExpendition != null) saveAndLoadExpendition.SaveUIExpemdition();
        if (saveAndLoadListDoorStatusSceneEX != null) saveAndLoadListDoorStatusSceneEX.SaveDataListDoorStatus();
        if (saveAndLoadOutPostReward != null) saveAndLoadOutPostReward.SaveDataOutPostReward();
        if (saveAndLoadTimemanager != null) saveAndLoadTimemanager.SaveDataTime();
        if (saveAndLoadBuildManager != null) saveAndLoadBuildManager.SaveBuildInScenes();
        if (saveAndLoadResoure != null) saveAndLoadResoure.SaveDataResoure();
        if (saveAndLoadCraftItms != null) saveAndLoadCraftItms.SaveDataCraftItems();
        if (saveAndLoadTunnutAndBroken != null) saveAndLoadTunnutAndBroken.SaveDataTunnutAndBroken();
        if (saveAndLoadPatint != null) saveAndLoadPatint.SaveDataPatint();
    }

    public void LoadGame()
    {
        LoadGane();
    }

    public void LoadGane()
    {
        if (saveAndLoadListNpc != null) saveAndLoadListNpc.LoadDataListNpc();
        if (saveAndLoadLIstItemsDataBoxes != null) saveAndLoadLIstItemsDataBoxes.LoadDataListItemDataBoxesAndDailyGive();
        if (saveAndLoadExpendition != null) saveAndLoadExpendition.LoadDataUIExFromJsonToScriptData();
        if (saveDataDDA != null) saveDataDDA.LoadDataDDAFromJsonToScriptData();
        if (saveAndLoadListDoorStatusSceneEX != null) saveAndLoadListDoorStatusSceneEX.LoadDataListDoorStatus();
        if (saveAndLoadOutPostReward != null) saveAndLoadOutPostReward.LoadDataOutPostReward();
        if (saveAndLoadTimemanager != null) saveAndLoadTimemanager.LoadDataTime();
        if (saveAndLoadBuildManager != null) saveAndLoadBuildManager.LoadBuildInScenes();
        if (saveAndLoadResoure != null) saveAndLoadResoure.LoadDataResore();
        if (saveAndLoadCraftItms != null) saveAndLoadCraftItms.LoadDataCraftItems();
        if (saveAndLoadTunnutAndBroken != null) saveAndLoadTunnutAndBroken.LoadDataTunnutAndBroken();
        if (saveAndLoadPatint != null) saveAndLoadPatint.LoadDataPatint();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
