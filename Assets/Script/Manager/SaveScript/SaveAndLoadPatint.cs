using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class SaveAndLoadPatint : MonoBehaviour
{
    public PatienManger patienManger;
    public GameManager gameManager;
    public DataCollentPatint dataCollentPatint;
    [SerializeField] private string savePathDataPatint;
    void Start()
    {
        savePathDataPatint = Path.Combine(Application.dataPath, "dataPatint.json");
        gameManager = FindObjectOfType<GameManager>();
        patienManger = gameManager.patienManger;
    }
    public void SaveDataPatint()
    {
        dataCollentPatint.listactiveHealingClinicPatient = patienManger.activeHealingClinicPatient;
        dataCollentPatint.listactiveHealingHospitalPatient = patienManger.activeHealingHospitalPatient;
        string json = JsonUtility.ToJson(dataCollentPatint, true);
        File.WriteAllText(savePathDataPatint, json);
    }
    public void LoadDataPatint()
    {
        if (File.Exists(savePathDataPatint))
        {
            string json = File.ReadAllText(savePathDataPatint);
            dataCollentPatint = JsonUtility.FromJson<DataCollentPatint>(json);
            patienManger.activeHealingClinicPatient = dataCollentPatint.listactiveHealingClinicPatient;
            patienManger.activeHealingHospitalPatient =dataCollentPatint.listactiveHealingHospitalPatient;
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
    public List<CurePatient> listactiveHealingClinicPatient;
    public List<CurePatient> listactiveHealingHospitalPatient;
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