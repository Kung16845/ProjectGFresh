using System;
using System.IO;
using UnityEngine;

public class SaveAndLoadTimemanager : MonoBehaviour
{
    public GameManager gameManager;
    public TimeManager timeManager;
    public DataCollentTime dataCollentTime;
    [SerializeField] private string savePathDataTime;

    private void Start()
    {
        savePathDataTime = Path.Combine(Application.dataPath, "dataTime.json");
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
        }

        if (timeManager == null && gameManager != null)
        {
            timeManager = gameManager.timeManager;
        }

        if (timeManager == null)
        {
            timeManager = TimeManager.Instance != null ? TimeManager.Instance : FindFirstObjectByType<TimeManager>();
        }
    }

    public void SaveDataTime()
    {
        EnsureDependencies();
        AddDataColletTime();
        string json = JsonUtility.ToJson(dataCollentTime, true);
        File.WriteAllText(savePathDataTime, json);
    }

    public void AddDataColletTime()
    {
        EnsureDependencies();
        if (dataCollentTime == null)
        {
            dataCollentTime = new DataCollentTime();
        }

        if (timeManager != null)
        {
            dataCollentTime.dateTime = timeManager.dateTime;
        }
    }

    public void LoadDataTime()
    {
        EnsureDependencies();

        if (File.Exists(savePathDataTime))
        {
            string json = File.ReadAllText(savePathDataTime);
            dataCollentTime = JsonUtility.FromJson<DataCollentTime>(json);

            if (dataCollentTime != null && dataCollentTime.dateTime != null && timeManager != null)
            {
                if (timeManager.dateTime == null)
                {
                    timeManager.dateTime = new DateTime(dataCollentTime.dateTime.day, dataCollentTime.dateTime.hour, dataCollentTime.dateTime.minutes, dataCollentTime.dateTime.isDayNight);
                }
                else
                {
                    timeManager.dateTime.day = dataCollentTime.dateTime.day;
                    timeManager.dateTime.hour = dataCollentTime.dateTime.hour;
                    timeManager.dateTime.minutes = dataCollentTime.dateTime.minutes;
                    timeManager.dateTime.isDayNight = dataCollentTime.dateTime.isDayNight;
                }
            }
        }
        else
        {
            dataCollentTime = new DataCollentTime();
        }
    }

    public void ResetDataTime()
    {
        dataCollentTime = new DataCollentTime();
        string json = JsonUtility.ToJson(dataCollentTime, true);
        File.WriteAllText(savePathDataTime, json);
    }
}

[Serializable]
public class DataCollentTime
{
    public DateTime dateTime;
}
