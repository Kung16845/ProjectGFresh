using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadOutPostReward : MonoBehaviour
{
    public DataColletsOutPostReWard dataColletsOutPostReWard;
    public OutpostSystem outpostSystem;
    [SerializeField] private string savePathOutPostReward;

    private void Start()
    {
        savePathOutPostReward = Path.Combine(Application.dataPath, "dda_OutPostReward.json");
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (outpostSystem == null)
        {
            outpostSystem = GameManager.Instance != null && GameManager.Instance.outpostSystem != null
                ? GameManager.Instance.outpostSystem
                : FindFirstObjectByType<OutpostSystem>();
        }
    }

    public void SaveDataOutPostReward()
    {
        EnsureDependencies();
        AddDataOutPostReward();
        string json = JsonUtility.ToJson(dataColletsOutPostReWard, true);
        File.WriteAllText(savePathOutPostReward, json);
    }

    public void AddDataOutPostReward()
    {
        EnsureDependencies();
        if (dataColletsOutPostReWard == null)
        {
            dataColletsOutPostReWard = new DataColletsOutPostReWard();
        }

        if (outpostSystem != null)
        {
            dataColletsOutPostReWard.listDataOutPostReward = outpostSystem.outpostRewards;
        }
    }

    public void LoadDataOutPostReward()
    {
        EnsureDependencies();

        if (File.Exists(savePathOutPostReward))
        {
            string json = File.ReadAllText(savePathOutPostReward);
            dataColletsOutPostReWard = JsonUtility.FromJson<DataColletsOutPostReWard>(json);

            if (dataColletsOutPostReWard != null && outpostSystem != null)
            {
                outpostSystem.outpostRewards = dataColletsOutPostReWard.listDataOutPostReward;
            }
        }
        else
        {
            dataColletsOutPostReWard = new DataColletsOutPostReWard();
        }
    }

    public void ResetDataOutPostReward()
    {
        dataColletsOutPostReWard = new DataColletsOutPostReWard();
        string json = JsonUtility.ToJson(dataColletsOutPostReWard, true);
        File.WriteAllText(savePathOutPostReward, json);
    }
}

[Serializable]
public class DataColletsOutPostReWard
{
    public List<OutpostReward> listDataOutPostReward = new List<OutpostReward>();
}
