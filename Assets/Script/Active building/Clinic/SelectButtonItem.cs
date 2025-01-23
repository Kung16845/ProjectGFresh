using UnityEngine;
using UnityEngine.UI;

public class SelectButtonItem : MonoBehaviour
{
    public Button selectButton; // Assign via Inspector

    private ClinicUI clinicUIRef;
    private FieldHospitalUI fieldHospital;
    private bool isEmptySlot = false;

    public void InitializeButton(ClinicUI clinicUI, int npcId)
    {
        clinicUIRef = clinicUI;
        isEmptySlot = false;
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => clinicUIRef.AddPatient(npcId));
        }
    }
    public void InitializeFieldButton(FieldHospitalUI fieldHospitalUI, int npcId)
    {
        fieldHospital = fieldHospitalUI;
        isEmptySlot = false;
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => fieldHospital.AddPatient(npcId));
        }
    }
    public void InitializeFieldForEmptySlot(FieldHospitalUI fieldHospitalUI)
    {
        fieldHospital = fieldHospitalUI;
        isEmptySlot = true;
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => fieldHospital.DisplayInjuredNpc());
        }
    }

    public void InitializeForEmptySlot(ClinicUI clinicUI)
    {
        clinicUIRef = clinicUI;
        isEmptySlot = true;
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => clinicUIRef.DisplayInjuredNpc());
        }
    }

    public void DisableButton()
    {
        if (selectButton != null)
        {
            selectButton.interactable = false;
        }
    }
}
