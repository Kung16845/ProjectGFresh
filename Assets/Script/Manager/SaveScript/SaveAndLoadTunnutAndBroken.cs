using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class SaveAndLoadTunnutAndBroken : MonoBehaviour
{
    public GameManager gameManager;
    public Tunnel tunnel;
    public Sattlelite sattlelite;
    public DataCollentTunnutAndBroken dataCollentTunnutAndBroken;
    [SerializeField] private string savePathDataTunnutAndBroken;
    private void Start()
    {
        savePathDataTunnutAndBroken = Path.Combine(Application.dataPath, "dataTunnutAndBroken.json");
        gameManager = FindObjectOfType<GameManager>();
        tunnel = FindObjectOfType<Tunnel>();
        sattlelite = FindObjectOfType<Sattlelite>();
    }
    public void SaveDataTunnutAndBroken()
    {
        AddDataColletTunnutAndBroken();
        string json = JsonUtility.ToJson(dataCollentTunnutAndBroken, true);
        File.WriteAllText(savePathDataTunnutAndBroken, json);
    }
    public void AddDataColletTunnutAndBroken()
    {

        dataCollentTunnutAndBroken.sattleliteOnline = sattlelite.SatelliteOnline;
        dataCollentTunnutAndBroken.reconActiveSattlelite = sattlelite.RecondroneActive;
        dataCollentTunnutAndBroken.recondurationSattlelite = sattlelite.Reconduration;
        dataCollentTunnutAndBroken.supplyDropActive = sattlelite.supplyDropActive;
        dataCollentTunnutAndBroken.supplyDropCountdownSattlelite = sattlelite.supplyDropCountdown;
        dataCollentTunnutAndBroken.isRepireSattlelite = sattlelite.isRepairing;
        dataCollentTunnutAndBroken.listsupplyDropItemsSattlelite = sattlelite.supplyDropItems;
        dataCollentTunnutAndBroken.finishDayBuildingSattleliteTime = sattlelite.finishDayBuildingTime;

        dataCollentTunnutAndBroken.tuneelIsopen = tunnel.tuneelisopen;
        dataCollentTunnutAndBroken.isclearingTuneel = tunnel.isclearing;
        dataCollentTunnutAndBroken.finishDayBuildingTunnutTime = tunnel.finishDayBuildingTime;
    }
    public void LoadDataTunnutAndBroken()
    {
        if (File.Exists(savePathDataTunnutAndBroken))
        {
            string json = File.ReadAllText(savePathDataTunnutAndBroken);
            dataCollentTunnutAndBroken = JsonUtility.FromJson<DataCollentTunnutAndBroken>(json);

            sattlelite.SatelliteOnline = dataCollentTunnutAndBroken.sattleliteOnline;
            sattlelite.RecondroneActive = dataCollentTunnutAndBroken.reconActiveSattlelite;
            sattlelite.Reconduration = dataCollentTunnutAndBroken.recondurationSattlelite;
            sattlelite.supplyDropActive = dataCollentTunnutAndBroken.supplyDropActive;
            sattlelite.supplyDropCountdown = dataCollentTunnutAndBroken.supplyDropCountdownSattlelite;
            sattlelite.isRepairing = dataCollentTunnutAndBroken.isRepireSattlelite;
            sattlelite.supplyDropItems = dataCollentTunnutAndBroken.listsupplyDropItemsSattlelite;
            sattlelite.finishDayBuildingTime = dataCollentTunnutAndBroken.finishDayBuildingSattleliteTime;

            tunnel.tuneelisopen = dataCollentTunnutAndBroken.tuneelIsopen;
            tunnel.isclearing = dataCollentTunnutAndBroken.isclearingTuneel;
            tunnel.finishDayBuildingTime = dataCollentTunnutAndBroken.finishDayBuildingTunnutTime;
            // timeManager.dateTime.sceneSystem = FindObjectOfType<SceneSystem>();

        }
        else
        {
            dataCollentTunnutAndBroken = new DataCollentTunnutAndBroken();
        }

    }
    public void ResetDataTunnutAndBroken()
    {
        dataCollentTunnutAndBroken = new DataCollentTunnutAndBroken();
        string json = JsonUtility.ToJson(dataCollentTunnutAndBroken, true);
        File.WriteAllText(savePathDataTunnutAndBroken, json);
    }
}
[Serializable]
public class DataCollentTunnutAndBroken
{
    public bool sattleliteOnline;
    public bool reconActiveSattlelite;
    public int recondurationSattlelite;
    public bool supplyDropActive;
    public int supplyDropCountdownSattlelite;
    public bool isRepireSattlelite;
    public List<ItemData> listsupplyDropItemsSattlelite;
    public int finishDayBuildingSattleliteTime;
    public bool tuneelIsopen;
    public bool isclearingTuneel;
    public int finishDayBuildingTunnutTime;
}