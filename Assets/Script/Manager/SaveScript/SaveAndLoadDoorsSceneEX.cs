using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadListDoorStatusSceneEX : MonoBehaviour
{
    public GameManager gameManager;
    public ManagerSceneEX managerSceneEX;
    public DataCollentListDoorStatus dataCollentDoorsOpenScneeEx;
    [SerializeField] private string savePathDataDoorsUnlock;

    private void Start()
    {
        savePathDataDoorsUnlock = Path.Combine(Application.dataPath, "dataLsitDoorStatus.json");
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
        }

        if (managerSceneEX == null && gameManager != null)
        {
            managerSceneEX = gameManager.managerSceneEX;
        }

        if (managerSceneEX == null)
        {
            managerSceneEX = FindFirstObjectByType<ManagerSceneEX>();
        }
    }

    public void SaveDataListDoorStatus()
    {
        EnsureDependencies();
        AddDataColletListDoorStatus();
        string json = JsonUtility.ToJson(dataCollentDoorsOpenScneeEx, true);
        File.WriteAllText(savePathDataDoorsUnlock, json);
    }

    public void AddDataColletListDoorStatus()
    {
        EnsureDependencies();
        if (dataCollentDoorsOpenScneeEx == null)
        {
            dataCollentDoorsOpenScneeEx = new DataCollentListDoorStatus();
        }

        if (managerSceneEX != null)
        {
            dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes = managerSceneEX.listDoorSceneExes;
        }
    }

    public void LoadDataListDoorStatus()
    {
        EnsureDependencies();

        if (File.Exists(savePathDataDoorsUnlock))
        {
            string json = File.ReadAllText(savePathDataDoorsUnlock);
            dataCollentDoorsOpenScneeEx = JsonUtility.FromJson<DataCollentListDoorStatus>(json);

            if (dataCollentDoorsOpenScneeEx != null && managerSceneEX != null && dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes != null)
            {
                managerSceneEX.listDoorSceneExes = dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes;
            }
        }
        else
        {
            dataCollentDoorsOpenScneeEx = new DataCollentListDoorStatus();
        }
    }

    public void ResetDataListDoorStatus()
    {
        EnsureDependencies();

        if (dataCollentDoorsOpenScneeEx == null)
        {
            dataCollentDoorsOpenScneeEx = new DataCollentListDoorStatus();
        }

        if (managerSceneEX != null && managerSceneEX.listDoorSceneExes != null)
        {
            dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes = managerSceneEX.listDoorSceneExes;

            for (int i = 0; i < dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes.Count; i++)
            {
                DoorsSceneEx doorsSceneEx = dataCollentDoorsOpenScneeEx.listDoorStatusInSceneExes[i];
                if (doorsSceneEx != null && doorsSceneEx.listUnLockDoorInSceneEx != null)
                {
                    for (int j = 0; j < doorsSceneEx.listUnLockDoorInSceneEx.Count; j++)
                    {
                        doorsSceneEx.listUnLockDoorInSceneEx[j] = false;
                    }
                }
            }
        }

        string json = JsonUtility.ToJson(dataCollentDoorsOpenScneeEx, true);
        File.WriteAllText(savePathDataDoorsUnlock, json);
    }
}

[Serializable]
public class DataCollentListDoorStatus
{
    public List<DoorsSceneEx> listDoorStatusInSceneExes = new List<DoorsSceneEx>();
}