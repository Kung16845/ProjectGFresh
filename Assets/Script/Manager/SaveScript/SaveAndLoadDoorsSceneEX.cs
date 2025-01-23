using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class SaveAndLoadListDoorStatusSceneEX : MonoBehaviour
{
    public GameManager gameManager;
    public ManagerSceneEX managerSceneEX;
    public DataCollentListDoorStatus dataCollentDoorsOpenScneeEx;
    [SerializeField] private string savePathDataDoorsUnlock;
    private void Start()
    {
        savePathDataDoorsUnlock = Path.Combine(Application.dataPath, "dataLsitDoorStatus.json");
        gameManager = FindObjectOfType<GameManager>();
        managerSceneEX = gameManager.managerSceneEX;
    }
    public void SaveDataListDoorStatus()
    {
        AddDataColletListDoorStatus();
        string json = JsonUtility.ToJson(dataCollentDoorsOpenScneeEx, true);
        File.WriteAllText(savePathDataDoorsUnlock, json);
    }
    public void AddDataColletListDoorStatus()
    {
        dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes = managerSceneEX.listDoorSceneExes;
    }
    public void LoadDataListDoorStatus()
    {
        if (File.Exists(savePathDataDoorsUnlock))
        {
            string json = File.ReadAllText(savePathDataDoorsUnlock);
            dataCollentDoorsOpenScneeEx = JsonUtility.FromJson<DataCollentListDoorStatus>(json);
            managerSceneEX.listDoorSceneExes = dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes;
        }
        else
        {
            dataCollentDoorsOpenScneeEx = new DataCollentListDoorStatus();
        }
    }
    public void ResetDataListDoorStatus()
    {
        dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes = managerSceneEX.listDoorSceneExes;

        foreach (DoorsSceneEx doorsSceneEx in dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes)
        {
            for (int i = 0; i < doorsSceneEx.listUnLockDoorInSceneEx.Count; i++)
            {
                doorsSceneEx.listUnLockDoorInSceneEx[i] = false; // ปรับสถานะเป็น false
            }
        }

        string json = JsonUtility.ToJson(dataCollentDoorsOpenScneeEx, true);
        File.WriteAllText(savePathDataDoorsUnlock, json);
    }
}
[Serializable]
public class DataCollentListDoorStatus
{
    public List<DoorsSceneEx> listDoorStatusInSceneExes;
}