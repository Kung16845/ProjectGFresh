using System.Collections.Generic;
using UnityEngine;

public class UImanger : MonoBehaviour
{
    public enum UIPanel
    {
        UpgradeUI,
        WorkshopUI,
        ChemcicalLabWorkshopUI,
        ChemcicalLabButtonUpgradeUI,
        CarWorkshopUI,
        CarUpgradeWorkshopUI,
        TunnelUI,
        ClearingTunnelUI,
        WorkshopUpgradeUI,
        ExpeditionUI,
        BeaconUI,
        BeaconUpgradeUI,
        SmallGardenUI,
        SmallGardenUpgradeButton,
        MediumGardenUI,
        MediumGardenUpgradeButton,
        SattleliteUI,
        SattleliteUpgradeButton,
        WaterPumpUI,
        WaterPumpUpgradeUi,
        SolarUI,
        SolarUpgradeUI,
        SmallBedUI,
        SmallBedUpgradeButton,
        MediumBedUI,
        MediumBedUpgradeUI,
        ClinicUI,
        ClinicUpgradeUI,
        ClinicInhuredNpcUI,
        FieldHospitalUI,
        FieldHospitalUpgradeUI,
        FieldHospitalInhuredNpcUI,
        LoungeUI,
        LoungeUpgradeButton,
        MoonshineUI,
        MoonshineUpgradeButton,

    }

    [System.Serializable]
    public struct UIPanelMapping
    {
        public UIPanel panelType;
        public GameObject panelObject;
    }

    public UIPanelMapping[] uiPanelMappings;
    private Dictionary<UIPanel, GameObject> uiPanels;

    public Globalstat globalstat;
    public List<GameObject> objectsToDisableColliders;
    private bool isExpeditionUIActive;

    void Awake()
    {
        // Initialize the dictionary
        uiPanels = new Dictionary<UIPanel, GameObject>();
        foreach (var mapping in uiPanelMappings)
        {
            uiPanels[mapping.panelType] = mapping.panelObject;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !globalstat.expiditionactiveeventactive)
        {
            ToggleUIPanel(UIPanel.ExpeditionUI);
        }
    }

    public void ToggleUIPanel(UIPanel panel)
    {
        if (uiPanels.ContainsKey(panel))
        {
            bool isActive = uiPanels[panel].activeSelf;
            uiPanels[panel].SetActive(!isActive);

            if (panel == UIPanel.ExpeditionUI)
            {
                isExpeditionUIActive = !isExpeditionUIActive;
            }
        }
        else
        {
            Debug.LogWarning($"UIPanel {panel} not found in dictionary!");
        }
    }

    public void ActivateUIPanel(UIPanel panel)
    {
        if (uiPanels.ContainsKey(panel))
        {
            uiPanels[panel].SetActive(true);
        }
    }

    public void DisableUIPanel(UIPanel panel)
    {
        if (uiPanels.ContainsKey(panel))
        {
            uiPanels[panel].SetActive(false);
        }
    }
}
