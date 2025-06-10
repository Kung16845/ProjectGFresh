using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SaveAndLoadOutPostReward : MonoBehaviour
{
    public DataColletsOutPostReWard dataColletsOutPostReWard;
    public OutpostSystem outpostSystem;
    [SerializeField] private string savePathOutPostReward;
    // Start is called before the first frame update
    void Start()
    {
        savePathOutPostReward = Path.Combine(Application.dataPath, "dda_OutPostReward.json");
        
        outpostSystem = GameManager.Instance.outpostSystem;
    }
    public void SaveDataOutPostReward()
    {
        AddDataOutPostReward();
        string json = JsonUtility.ToJson(dataColletsOutPostReWard, true);
        File.WriteAllText(savePathOutPostReward, json);
    }
    public void AddDataOutPostReward()
    {   
        dataColletsOutPostReWard.listDataOutPostReward = outpostSystem.outpostRewards;
    }
    public void LoadDataOutPostReward()
    {
        if (File.Exists(savePathOutPostReward))
        {
            string json = File.ReadAllText(savePathOutPostReward);
            dataColletsOutPostReWard = JsonUtility.FromJson<DataColletsOutPostReWard>(json);
            outpostSystem.outpostRewards = dataColletsOutPostReWard.listDataOutPostReward;
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
    public List<OutpostReward> listDataOutPostReward;
}

