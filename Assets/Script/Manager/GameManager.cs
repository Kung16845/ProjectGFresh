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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void NewGame()
    {
        saveAndLoadExpendition.ResetDataUIEX();
        saveAndLoadListNpc.ResetDataListNpc();
        saveAndLoadLIstItemsDataBoxes.ResetDataListItemBoxesAndDailyGive();
        saveDataDDA.ResetDataDDA();
        saveAndLoadListDoorStatusSceneEX.ResetDataListDoorStatus();
        saveAndLoadOutPostReward.ResetDataOutPostReward();
        saveAndLoadTimemanager.ResetDataTime();
        saveAndLoadBuildManager.ResetDataBuilding();
        saveAndLoadResoure.ResetDataResoure();
        saveAndLoadCraftItms.ResetDataCraftItems();
        npcManager.StartGameCreateGropNpc();
        saveAndLoadTunnutAndBroken.ResetDataTunnutAndBroken();
        saveAndLoadPatint.ResetDataPatint();

    }
    public void SaveGame()
    {
        saveAndLoadListNpc.SaveListNpc();
        saveAndLoadLIstItemsDataBoxes.SaveListItemsDataBoxesAndDailyGive();
        saveAndLoadExpendition.SaveUIExpemdition();
        saveAndLoadListDoorStatusSceneEX.SaveDataListDoorStatus();
        saveAndLoadOutPostReward.SaveDataOutPostReward();
        // saveDataDDA.AddDataDDAAndSave();
        saveAndLoadTimemanager.SaveDataTime();
        saveAndLoadBuildManager.SaveBuildInScenes();
        saveAndLoadResoure.SaveDataResoure();
        saveAndLoadCraftItms.SaveDataCraftItems();
        saveAndLoadTunnutAndBroken.SaveDataTunnutAndBroken();
        saveAndLoadPatint.SaveDataPatint();
    }
    public void LoadGane()
    {
        saveAndLoadListNpc.LoadDataListNpc();
        saveAndLoadLIstItemsDataBoxes.LoadDataListItemDataBoxesAndDailyGive();
        saveAndLoadExpendition.LoadDataUIExFromJsonToScriptData();
        saveDataDDA.LoadDataDDAFromJsonToScriptData();
        saveAndLoadListDoorStatusSceneEX.LoadDataListDoorStatus();
        saveAndLoadOutPostReward.LoadDataOutPostReward();
        saveAndLoadTimemanager.LoadDataTime();
        saveAndLoadBuildManager.LoadBuildInScenes();
        saveAndLoadResoure.LoadDataResore();
        saveAndLoadCraftItms.LoadDataCraftItems();
        saveAndLoadTunnutAndBroken.LoadDataTunnutAndBroken();
        saveAndLoadPatint.LoadDataPatint();
    }
    public void QuitGame()
    {

        Application.Quit();

    }
}
