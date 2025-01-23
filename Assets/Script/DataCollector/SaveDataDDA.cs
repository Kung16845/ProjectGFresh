using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DataDDA
{
    public float killPerMinute;
    public float accuracy;
    public float multiKillCount;
    public float barrierDamage;
    public float valueFail;
    public int recordCount;
}

[Serializable]
public class DataDDACollection
{
    public List<DataDDA> records = new List<DataDDA>();
    public DataDDA averageData = new DataDDA();
}

public class SaveDataDDA : MonoBehaviour
{
    public DDAdataCollector scriptDDAdataCollector;
    public DataDDACollection dataDDACollection;
    [SerializeField] private string savePathDataDDA;

    private void Start()
    {
        scriptDDAdataCollector = FindObjectOfType<DDAdataCollector>();
        savePathDataDDA = Path.Combine(Application.dataPath,"dda_data.json");
        // LoadDataFromDataJsonToScriptData();
    }

    public void AddDataDDAAndSave()
    {
        // ดึงข้อมูลปัจจุบันจากตัวเก็บข้อมูล
        var newRecord = new DataDDA
        {
            killPerMinute = scriptDDAdataCollector.killPerMinute,
            accuracy = scriptDDAdataCollector.accuracy,
            multiKillCount = scriptDDAdataCollector.multiKillCount,
            barrierDamage = scriptDDAdataCollector.barrierDamage,
            valueFail = scriptDDAdataCollector.valueFail,
            recordCount = dataDDACollection.records.Count + 1

        };

        // เพิ่มข้อมูลใหม่ลงในรายการ
        dataDDACollection.records.Add(newRecord);

        // คำนวณค่าเฉลี่ยใหม่
        CalculateAverage();

        // บันทึกข้อมูล
        SaveDataDDATOJSON();
    }

    private void CalculateAverage()
    {
        // รีเซ็ตค่าเฉลี่ย
        var totalKills = 0f;
        var totalAccuracy = 0f;
        var totalMultiKill = 0f;
        var totalBarrierDamage = 0f;
        var totalValueFail = 0f;
        var totalCount = dataDDACollection.records.Count;


        foreach (var record in dataDDACollection.records)
        {
            totalKills += record.killPerMinute;
            totalAccuracy += record.accuracy;
            totalMultiKill += record.multiKillCount;
            totalBarrierDamage += record.barrierDamage;
            totalValueFail += record.valueFail;
        }

        dataDDACollection.averageData.killPerMinute = totalKills / totalCount;
        dataDDACollection.averageData.accuracy = totalAccuracy / totalCount;
        dataDDACollection.averageData.multiKillCount = totalMultiKill / totalCount;
        dataDDACollection.averageData.barrierDamage = totalBarrierDamage / totalCount;
        dataDDACollection.averageData.valueFail = totalValueFail;
        dataDDACollection.averageData.recordCount = totalCount;
    }

    public void SaveDataDDATOJSON()
    {
        string json = JsonUtility.ToJson(dataDDACollection, true);
        File.WriteAllText(savePathDataDDA, json);
        Debug.Log($"Data saved to {savePathDataDDA}");
    }
 
    public void LoadDataDDAFromJsonToScriptData()
    {
        if (File.Exists(savePathDataDDA))
        {
            string json = File.ReadAllText(savePathDataDDA);
            dataDDACollection = JsonUtility.FromJson<DataDDACollection>(json);
            
            Debug.Log($"Data loaded from {savePathDataDDA}");
        }
        else
        {
            dataDDACollection = new DataDDACollection();
            Debug.Log("No data file found. Created new data collection.");
        }
    }

    public void ResetDataDDA()
    {
        dataDDACollection = new DataDDACollection();
        SaveDataDDATOJSON();
    }
}
