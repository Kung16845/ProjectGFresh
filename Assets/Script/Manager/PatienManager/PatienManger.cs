using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatienManger : MonoBehaviour
{
    public List<CurePatient> activeHealingClinicPatient = new List<CurePatient>();
    public List<CurePatient> activeHealingHospitalPatient = new List<CurePatient>();

    // Reference to the NpcManager (can be assigned in the inspector or found at runtime)
    public NpcManager npcManager;
    public Globalstat globalStat;
    public void AddPatient(int npcId, float currentHp, float healingRate, PatienSourceSource source)
    {
        // Create a new CurePatient object
        CurePatient newPatient = new CurePatient(npcId, currentHp, healingRate, source);
        globalStat.Usedcurebed +=1;
        globalStat.Activecurebed = globalStat.Totalcurebed - globalStat.Usedcurebed;
        // Decide which list to add the patient to based on the source
        switch (source)
        {
            case PatienSourceSource.Clinic:
                activeHealingClinicPatient.Add(newPatient);
                break;
            case PatienSourceSource.FieldHaspital:
                activeHealingHospitalPatient.Add(newPatient);
                break;
            default:
                Debug.LogWarning("Unsupported patient source. Patient not added.");
                return;
        }

        Debug.Log($"Added new patient with ID: {npcId} to {source} healing list.");
    }

    private void Start()
    {
        if (npcManager == null)
        {
            npcManager = GameManager.Instance.npcManager;
        }
    }

    public void UpdateJobs(List<CurePatient> jobList)
    {
        for (int i = jobList.Count - 1; i >= 0; i--)
        {
            CurePatient job = jobList[i];
            if (!job.isfullyhealed)
            {
                // Use the patient's dynamic healing rate
                float healingPerSecond = job.Healingrate / 60f;

                // Apply healing over time
                job.Npchp += healingPerSecond * Time.deltaTime;

                // Check if the patient is fully healed
                if (job.Npchp >= 100)
                {
                    job.Npchp = 100; // Cap HP at 100
                    job.isfullyhealed = true;

                    // Handle the completion logic
                    CompleteHealingPatient(job);

                    // Remove the patient from the healing list
                    jobList.RemoveAt(i);
                }
            }
        }
    }


    private void CompleteHealingPatient(CurePatient job)
    {
        // Once the patient is fully healed, they should be moved back to the normal NPC list
        globalStat.Usedcurebed -=1;
        globalStat.Activecurebed = globalStat.Totalcurebed - globalStat.Usedcurebed;
        if (npcManager != null)
        {   
            NpcClass npc = npcManager.GetNpcById(job.NpcID);
            npc.isWorking = false; // Mark the NPC as not working
            Debug.Log("NPC " + job.NpcID + " has been fully healed and returned to normal list.");
        }
        else
        {
            Debug.LogWarning("NpcManager is not assigned. Cannot move NPC back to normal list.");
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
    // Add other sources as needed
}

