using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SaveAndLoadTimemanager : MonoBehaviour
{   
    public GameManager gameManager;
    public TimeManager timeManager;
    public DataCollentTime dataCollentTime;
    [SerializeField] private string savePathDataTime;
    private void Start()
    {
        savePathDataTime = Path.Combine(Application.dataPath, "dataTime.json");
        gameManager = FindObjectOfType<GameManager>();
        timeManager = gameManager.timeManager;
    }
    public void SaveDataTime()
    {   
        AddDataColletTime();
        string json = JsonUtility.ToJson(dataCollentTime,true);
        File.WriteAllText(savePathDataTime,json);
    }
    public void AddDataColletTime()
    {
        dataCollentTime.dateTime = timeManager.dateTime;
    }
    public void LoadDataTime()
    {   
         if(File.Exists(savePathDataTime))
        {
            string json = File.ReadAllText(savePathDataTime);
            dataCollentTime = JsonUtility.FromJson<DataCollentTime>(json);
            timeManager.dateTime.day = dataCollentTime.dateTime.day;
            timeManager.dateTime.hour = dataCollentTime.dateTime.hour;
            timeManager.dateTime.minutes= dataCollentTime.dateTime.minutes;
            // timeManager.dateTime.sceneSystem = FindObjectOfType<SceneSystem>();
            
        }
        else 
        {
            dataCollentTime = new DataCollentTime();
        }
        
    }
    public void ResetDataTime()
    {
        dataCollentTime = new DataCollentTime();
        string json = JsonUtility.ToJson(dataCollentTime,true);
        File.WriteAllText(savePathDataTime,json);
    }
}
[Serializable]
public class DataCollentTime
{
    public DateTime dateTime;
}
