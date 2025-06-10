using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClinicUI : MonoBehaviour
{
    private Clinic clinic;
    public TextMeshProUGUI AviableBed;
    public TextMeshProUGUI UpgradeBenefits;
    public TextMeshProUGUI HealingLate;
    public Transform DisplayParent;    // For displaying patients currently being healed or injured NPCs
    public Transform SelectParent;     // For displaying select slots or buttons
    public GameObject SelectPrefab;
    public GameObject PatienPrefab;
    public UImanger uImanger;
    private PatienManger patienManger;
    private NpcManager npcManager;
    private Globalstat globalstat;

    // Keep a reference of injured NPCs currently displayed for selection
    private List<NpcClass> displayedInjuredNpcs = new List<NpcClass>();

    void OnEnable()
    {
        uImanger = FindObjectOfType<UImanger>();
        npcManager = FindObjectOfType<NpcManager>();
        patienManger = FindObjectOfType<PatienManger>();
        clinic = FindObjectOfType<Clinic>();
        globalstat = FindObjectOfType<Globalstat>();
        globalstat.Activecurebed = globalstat.Totalcurebed - globalstat.Usedcurebed; 
        UpdateUI();
    }

    public void InitializeUpgradeData()
    {
        clinic.AssignUpgradeData();
    }

    public void displayPatient()
    {
        // Clear existing displays
        foreach (Transform child in DisplayParent)
            Destroy(child.gameObject);
        foreach (Transform child in SelectParent)
            Destroy(child.gameObject);

        int totalSlots = globalstat.Activecurebed; 
        int displayedCount = 0;

        // Display currently healing patients in the clinic
         foreach (var patient in patienManger.activeHealingClinicPatient)
        {

            // Instantiate a select slot for this patient

            // Instantiate the patient prefab inside the display parent
            GameObject patientObj = Instantiate(PatienPrefab, SelectParent);

            // Retrieve NPC data from npcManager (looking in working list first, then normal list)
            NpcClass npcData = npcManager.GetNpcById(patient.NpcID);


            if (npcData != null)
            {
                // Get face image
                Sprite faceSprite = GetNpcFaceSprite(npcData);

                // Set data on the patient UI
                PatientUIItem uiItem = patientObj.GetComponent<PatientUIItem>();
                if (uiItem != null)
                {
                    uiItem.SetData(npcData.nameNpc, faceSprite, patient.Npchp);
                }
            }

            displayedCount++;
        }

        int remainingSlots = totalSlots - displayedCount;
        for (int i = 0; i < remainingSlots; i++)
        {
            Debug.Log(remainingSlots);
            // Create an empty slot (no patient object needed here)
            GameObject slotObj = Instantiate(SelectPrefab, SelectParent);
            SelectButtonItem selectButtonItem = slotObj.GetComponent<SelectButtonItem>();
            if (selectButtonItem != null)
            {
                // For empty slots, clicking the button should display injured NPCs
                selectButtonItem.InitializeForEmptySlot(this);
            }
        }
    }


    public void DisplayInjuredNpc()
    {
        uImanger.ToggleUIPanel(UImanger.UIPanel.ClinicInhuredNpcUI);

        // Clear existing children in the display and select parents
        foreach (Transform child in DisplayParent)
            Destroy(child.gameObject);
        foreach (Transform child in SelectParent)
            Destroy(child.gameObject);

        displayedInjuredNpcs.Clear();

        // Display only NPCs that are injured (hp < 100) from listNpc
        var injuredNpcs = npcManager.listNpc.Where(npc => npc.hp < 100).ToList();
        foreach (var npcData in injuredNpcs)
        {
            // Instantiate patient prefab
            GameObject patientObj = Instantiate(PatienPrefab, DisplayParent);

            // Get face image
            Sprite faceSprite = GetNpcFaceSprite(npcData);

            // Set data on the patient UI
            PatientUIItem uiItem = patientObj.GetComponent<PatientUIItem>();
            if (uiItem != null)
            {
                uiItem.SetData(npcData.nameNpc, faceSprite, npcData.hp);

                // Initialize button with a valid Action and label
                uiItem.InitializeButton(() => AddPatientToHealing(npcData.idnpc));
            }

            displayedInjuredNpcs.Add(npcData);
        }
    }

    private void AddPatientToHealing(int npcId)
    {
        Debug.Log($"Adding patient with ID: {npcId} to healing");
        // Call the method to add the patient to the manager
        patienManger.AddPatient(npcId, npcManager.listNpc.First(npc => npc.idnpc == npcId).hp, 2f, PatienSourceSource.Clinic);

        // Refresh UI after adding
        DisplayInjuredNpc();
        displayPatient();
    }
    // Updated AddPatient function to take an npcId
    public void AddPatient(int npcId)
    {
        // Find the NPC with this ID in the displayedInjuredNpcs
        NpcClass npcToAdd = displayedInjuredNpcs.FirstOrDefault(n => n.idnpc == npcId);
        if (npcToAdd != null)
        {
            float healingRate = 2f; // example healing rate
            patienManger.AddPatient(npcToAdd.idnpc, npcToAdd.hp, healingRate, PatienSourceSource.Clinic);
            npcToAdd.isWorking = true; // Mark the NPC as working in the clinic
            Debug.Log($"Added NPC {npcToAdd.nameNpc}(ID: {npcToAdd.idnpc}) as a patient to the clinic.");

            // Refresh UI
            DisplayInjuredNpc();
            displayPatient();
        }
        else
        {
            Debug.LogWarning($"No injured NPC with ID {npcId} found in the currently displayed list.");
        }
    }

    void UpdateUI()
    {
        if (clinic != null)
        {
            int bed = clinic.CurrentActiveCurebed;
            AviableBed.text = $"Bed Count: {bed} ";

            if (clinic.upgradeBuilding.currentLevel < clinic.upgradeBuilding.maxLevel)
            {
                UpgradeBenefits.text = $"Upgrade Benefit: Provide 2 Bed.";
            }
            else
            {
                UpgradeBenefits.text = "Fully Upgraded";
            }
        }
        else
        {
            AviableBed.text = "clinic not found";
            UpgradeBenefits.text = string.Empty;
        }
    }

    private Sprite GetNpcFaceSprite(NpcClass npcData)
    {
        // Retrieve the head sprite based on npcData.idHead
        HeadCoutume headCoutume = npcManager.GetSpriteHeadCoutumeById(npcData.idHead);
        if (headCoutume != null)
        {
            return headCoutume.spriteHead;
        }
        return null;
    }
    public void OpenApplyMedicineUI()
    {
        Debug.Log("OpenMedicine");
        DisplayPatientsWithAction((uiItem, patient) =>
        {
            int medicineItemID = 1020122; // Replace with actual medicine item ID
            if (InventoryItemPresent.Instance.HasItem(medicineItemID))
            {
                Debug.Log("HasPIll");
                uiItem.InitializeButton(() =>ApplyMedicineToPatient(patient, medicineItemID));
            }
        });
    }

    public void OpenApplyBandageUI()
    {
        // Display patients currently healing
        DisplayPatientsWithAction((uiItem, patient) =>
        {
            int bandageItemID = 1020121; // Replace with actual bandage item ID
            if (InventoryItemPresent.Instance.HasItem(bandageItemID))
            {
                uiItem.InitializeButton(() =>ApplyBandageToPatient(patient, bandageItemID));
            }
        });
    }

    private void ApplyMedicineToPatient(CurePatient patient, int medicineItemID)
    {
        float healingBoost = 5f; // Increase healing rate by this value
        patient.Healingrate += healingBoost;
        InventoryItemPresent.Instance.RemoveItem(new ItemData { idItem = medicineItemID, count = 1 });
        Debug.Log($"Applied medicine to Patient ID: {patient.NpcID}, new healing rate: {patient.Healingrate}");
    }

    private void ApplyBandageToPatient(CurePatient patient, int bandageItemID)
    {
        float instantHealAmount = 20f; // Heal this amount instantly
        patient.Npchp = Mathf.Min(patient.Npchp + instantHealAmount, 100f);
        InventoryItemPresent.Instance.RemoveItem(new ItemData { idItem = bandageItemID, count = 1 });
        Debug.Log($"Applied bandage to Patient ID: {patient.NpcID}, new HP: {patient.Npchp}");
    }

    private void DisplayPatientsWithAction(System.Action<PatientUIItem, CurePatient> setupAction)
    {
        // Clear existing UI
        foreach (Transform child in SelectParent)
        {
            Destroy(child.gameObject);
        }

        // Display each patient with the specified action
        foreach (var patient in patienManger.activeHealingClinicPatient)
        {
            GameObject patientObj = Instantiate(PatienPrefab, SelectParent);
            PatientUIItem uiItem = patientObj.GetComponent<PatientUIItem>();

            if (uiItem != null)
            {
                NpcClass npcData = npcManager.GetNpcById(patient.NpcID);
                Sprite faceSprite = npcData != null ? GetNpcFaceSprite(npcData) : null;

                uiItem.SetData(npcData?.nameNpc ?? $"NPC {patient.NpcID}", faceSprite, patient.Npchp);

                // Allow the caller to define the specific setup for this patient
                setupAction(uiItem, patient);
            }
        }
    }
}
