using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadPatint : MonoBehaviour
{
    public PatienManger patienManger;
    public GameManager gameManager;
    public DataCollentPatint dataCollentPatint;
    [SerializeField] private string savePathDataPatint;

    private void Start()
    {
        savePathDataPatint = Path.Combine(Application.dataPath, "dataPatint.json");
        EnsureDependencies();
    }

    private void EnsureDependencies()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
        }

        if (patienManger == null && gameManager != null)
        {
            patienManger = gameManager.patienManger;
        }

        if (patienManger == null)
        {
            patienManger = FindFirstObjectByType<PatienManger>();
        }
    }

    public void SaveDataPatint()
    {
        EnsureDependencies();

        if (dataCollentPatint == null)
        {
            dataCollentPatint = new DataCollentPatint();
        }

        if (patienManger != null)
        {
            dataCollentPatint.listactiveHealingClinicPatient = patienManger.activeHealingClinicPatient;
            dataCollentPatint.listactiveHealingHospitalPatient = patienManger.activeHealingHospitalPatient;
        }

        string json = JsonUtility.ToJson(dataCollentPatint, true);
        File.WriteAllText(savePathDataPatint, json);
    }

    public void LoadDataPatint()
    {
        EnsureDependencies();

        if (File.Exists(savePathDataPatint))
        {
            string json = File.ReadAllText(savePathDataPatint);
            dataCollentPatint = JsonUtility.FromJson<DataCollentPatint>(json);

            if (dataCollentPatint != null && patienManger != null)
            {
                if (dataCollentPatint.listactiveHealingClinicPatient != null)
                {
                    patienManger.activeHealingClinicPatient = dataCollentPatint.listactiveHealingClinicPatient;
                }

                if (dataCollentPatint.listactiveHealingHospitalPatient != null)
                {
                    patienManger.activeHealingHospitalPatient = dataCollentPatint.listactiveHealingHospitalPatient;
                }
            }
        }
        else
        {
            dataCollentPatint = new DataCollentPatint();
        }
    }

    public void ResetDataPatint()
    {
        dataCollentPatint = new DataCollentPatint();
        string json = JsonUtility.ToJson(dataCollentPatint, true);
        File.WriteAllText(savePathDataPatint, json);
    }
}

[Serializable]
public class DataCollentPatint
{
    public List<CurePatient> listactiveHealingClinicPatient = new List<CurePatient>();
    public List<CurePatient> listactiveHealingHospitalPatient = new List<CurePatient>();
}

[Serializable]
public class InfoCurePatient
{
    public int infoNpcID;
    public float infoNpchp;
    public float infoHealingrate;
    public bool infoisfullyhealed;
    public PatienSourceSource infoSource;
}