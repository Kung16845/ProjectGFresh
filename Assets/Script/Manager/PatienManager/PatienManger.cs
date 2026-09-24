using System;
using System.Collections.Generic;
using UnityEngine;

public class PatienManger : MonoBehaviour
{
    public List<CurePatient> activeHealingClinicPatient = new List<CurePatient>();
    public List<CurePatient> activeHealingHospitalPatient = new List<CurePatient>();

    public NpcManager npcManager;
    public Globalstat globalStat;

    private void Awake()
    {
        if (globalStat == null)
        {
            globalStat = FindFirstObjectByType<Globalstat>();
        }
    }

    private void Start()
    {
        if (npcManager == null)
        {
            npcManager = GameManager.Instance != null && GameManager.Instance.npcManager != null
                ? GameManager.Instance.npcManager
                : FindFirstObjectByType<NpcManager>();
        }
    }

    public void AddPatient(int npcId, float currentHp, float healingRate, PatienSourceSource source)
    {
        CurePatient newPatient = new CurePatient(npcId, currentHp, healingRate, source);

        if (globalStat == null)
        {
            globalStat = FindFirstObjectByType<Globalstat>();
        }

        if (globalStat != null)
        {
            globalStat.Usedcurebed += 1;
            globalStat.Activecurebed = globalStat.Totalcurebed - globalStat.Usedcurebed;
        }

        switch (source)
        {
            case PatienSourceSource.Clinic:
                activeHealingClinicPatient.Add(newPatient);
                break;
            case PatienSourceSource.FieldHaspital:
                activeHealingHospitalPatient.Add(newPatient);
                break;
            default:
                Debug.LogWarning("[PatienManger] Unsupported patient source. Patient not added.");
                return;
        }

        Debug.Log($"[PatienManger] Added new patient with ID: {npcId} to {source} healing list.");
    }

    public void UpdateJobs(List<CurePatient> jobList)
    {
        if (jobList == null) return;

        for (int i = jobList.Count - 1; i >= 0; i--)
        {
            CurePatient job = jobList[i];
            if (job == null) continue;

            if (!job.isfullyhealed)
            {
                float healingPerSecond = job.Healingrate / 60f;
                job.Npchp += healingPerSecond * Time.deltaTime;

                if (job.Npchp >= 100f)
                {
                    job.Npchp = 100f;
                    job.isfullyhealed = true;
                    CompleteHealingPatient(job);
                    jobList.RemoveAt(i);
                }
            }
        }
    }

    private void CompleteHealingPatient(CurePatient job)
    {
        if (job == null) return;

        if (globalStat == null)
        {
            globalStat = FindFirstObjectByType<Globalstat>();
        }

        if (globalStat != null)
        {
            globalStat.Usedcurebed -= 1;
            globalStat.Activecurebed = globalStat.Totalcurebed - globalStat.Usedcurebed;
        }

        if (npcManager == null)
        {
            npcManager = GameManager.Instance != null && GameManager.Instance.npcManager != null
                ? GameManager.Instance.npcManager
                : FindFirstObjectByType<NpcManager>();
        }

        if (npcManager != null)
        {
            NpcClass npc = npcManager.GetNpcById(job.NpcID);
            if (npc != null)
            {
                npc.isWorking = false;
            }
            Debug.Log($"[PatienManger] NPC {job.NpcID} has been fully healed and returned to normal list.");
        }
        else
        {
            Debug.LogWarning("[PatienManger] NpcManager is not assigned. Cannot move NPC back to normal list.");
        }
    }
}

[System.Serializable]
public class CurePatient
{
    public int NpcID;
    public float Npchp;
    public float Healingrate;
    public bool isfullyhealed;
    public PatienSourceSource source;

    public CurePatient(int ID, float PaitenNpchp, float HealinfSpeed, PatienSourceSource patienSourceSource)
    {
        NpcID = ID;
        Npchp = PaitenNpchp;
        Healingrate = HealinfSpeed;
        isfullyhealed = false;
        source = patienSourceSource;
    }
}

public enum PatienSourceSource
{
    Clinic,
    FieldHaspital,
}
