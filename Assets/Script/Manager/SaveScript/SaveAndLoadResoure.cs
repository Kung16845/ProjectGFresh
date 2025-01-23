using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class SaveAndLoadResoure : MonoBehaviour
{
    public BuildManager buildManager;
    public GameManager gameManager;
    public DataCollentResoure dataCollentResoure;
    [SerializeField] private string savePathDataResoure;
    // Start is called before the first frame update
    void Start()
    {
        savePathDataResoure = Path.Combine(Application.dataPath, "dataResoure.json");
        gameManager = FindObjectOfType<GameManager>();
        buildManager = gameManager.buildManager;
    }

    public void SaveDataResoure()
    {
        AddDataColletResoure();
        string json = JsonUtility.ToJson(dataCollentResoure, true);
        File.WriteAllText(savePathDataResoure, json);
    }
    public void AddDataColletResoure()
    {
        dataCollentResoure.steel = buildManager.steel;
        dataCollentResoure.plank = buildManager.plank;
        dataCollentResoure.food = buildManager.food;
        dataCollentResoure.fuel = buildManager.fuel;
        dataCollentResoure.ammo = buildManager.ammo;
        dataCollentResoure.npc = buildManager.npc;
    }
    public void LoadDataResore()
    {
        if (File.Exists(savePathDataResoure))
        {
            string json = File.ReadAllText(savePathDataResoure);
            dataCollentResoure = JsonUtility.FromJson<DataCollentResoure>(json);
            buildManager.steel = dataCollentResoure.steel;
            buildManager.plank = dataCollentResoure.plank;
            buildManager.food = dataCollentResoure.food;
            buildManager.fuel = dataCollentResoure.fuel;
            buildManager.ammo = dataCollentResoure.ammo;
            buildManager.npc = dataCollentResoure.npc;
        }
        else
        {
            dataCollentResoure = new DataCollentResoure();
        }
    }
    public void ResetDataResoure()
    {
        dataCollentResoure = new DataCollentResoure();
        string json = JsonUtility.ToJson(dataCollentResoure, true);
        File.WriteAllText(savePathDataResoure, json);
    }
}
[Serializable]
public class DataCollentResoure
{
    public int steel;
    public int plank;
    public int food;
    public int fuel;
    public int ammo;
    public int npc;
}

