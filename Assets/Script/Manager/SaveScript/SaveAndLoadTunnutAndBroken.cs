using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
        }

        if (tunnel == null)
        {
            tunnel = FindFirstObjectByType<Tunnel>();
        }

        if (sattlelite == null)
        {
            sattlelite = FindFirstObjectByType<Sattlelite>();
        }
    }

    public void SaveDataTunnutAndBroken()
    {
        EnsureDependencies();
        AddDataColletTunnutAndBroken();
        string json = JsonUtility.ToJson(dataCollentTunnutAndBroken, true);
        File.WriteAllText(savePathDataTunnutAndBroken, json);
    }

    public void AddDataColletTunnutAndBroken()
    {
        EnsureDependencies();
        if (dataCollentTunnutAndBroken == null)
        {
            dataCollentTunnutAndBroken = new DataCollentTunnutAndBroken();
        }

        if (sattlelite != null)
        {
            dataCollentTunnutAndBroken.sattleliteOnline = sattlelite.SatelliteOnline;
            dataCollentTunnutAndBroken.reconActiveSattlelite = sattlelite.RecondroneActive;
            dataCollentTunnutAndBroken.recondurationSattlelite = sattlelite.Reconduration;
            dataCollentTunnutAndBroken.supplyDropActive = sattlelite.supplyDropActive;
            dataCollentTunnutAndBroken.supplyDropCountdownSattlelite = sattlelite.supplyDropCountdown;
            dataCollentTunnutAndBroken.isRepireSattlelite = sattlelite.isRepairing;
            dataCollentTunnutAndBroken.listsupplyDropItemsSattlelite = sattlelite.supplyDropItems;
            dataCollentTunnutAndBroken.finishDayBuildingSattleliteTime = sattlelite.finishDayBuildingTime;
        }

        if (tunnel != null)
        {
            dataCollentTunnutAndBroken.tuneelIsopen = tunnel.tuneelisopen;
            dataCollentTunnutAndBroken.isclearingTuneel = tunnel.isclearing;
            dataCollentTunnutAndBroken.finishDayBuildingTunnutTime = tunnel.finishDayBuildingTime;
        }
    }

    public void LoadDataTunnutAndBroken()
    {
        EnsureDependencies();

        if (File.Exists(savePathDataTunnutAndBroken))
        {
            string json = File.ReadAllText(savePathDataTunnutAndBroken);
            dataCollentTunnutAndBroken = JsonUtility.FromJson<DataCollentTunnutAndBroken>(json);

            if (dataCollentTunnutAndBroken != null)
            {
                if (sattlelite != null)
                {
                    sattlelite.SatelliteOnline = dataCollentTunnutAndBroken.sattleliteOnline;
                    sattlelite.RecondroneActive = dataCollentTunnutAndBroken.reconActiveSattlelite;
                    sattlelite.Reconduration = dataCollentTunnutAndBroken.recondurationSattlelite;
                    sattlelite.supplyDropActive = dataCollentTunnutAndBroken.supplyDropActive;
                    sattlelite.supplyDropCountdown = dataCollentTunnutAndBroken.supplyDropCountdownSattlelite;
                    sattlelite.isRepairing = dataCollentTunnutAndBroken.isRepireSattlelite;
                    sattlelite.supplyDropItems = dataCollentTunnutAndBroken.listsupplyDropItemsSattlelite;
                    sattlelite.finishDayBuildingTime = dataCollentTunnutAndBroken.finishDayBuildingSattleliteTime;
                }

                if (tunnel != null)
                {
                    tunnel.tuneelisopen = dataCollentTunnutAndBroken.tuneelIsopen;
                    tunnel.isclearing = dataCollentTunnutAndBroken.isclearingTuneel;
                    tunnel.finishDayBuildingTime = dataCollentTunnutAndBroken.finishDayBuildingTunnutTime;
                }
            }
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
    public List<ItemData> listsupplyDropItemsSattlelite = new List<ItemData>();
    public int finishDayBuildingSattleliteTime;
    public bool tuneelIsopen;
    public bool isclearingTuneel;
    public int finishDayBuildingTunnutTime;
}