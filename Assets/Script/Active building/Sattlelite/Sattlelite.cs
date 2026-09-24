using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Sattlelite : MonoBehaviour
{
    public bool SatelliteOnline = false;
    public bool RecondroneActive = false;
    public int Reconduration;
    public TimeManager timeManager;
    public DateTime dateTime;
    public int currentDay;
    public UImanger uImanger;
    public BuildManager buildManager;
    public Globalstat globalstat;
    public InventoryItemPresent inventoryItemPresent;
    public SpriteRenderer spriteRenderer;
    public Sprite repairedSpriteRenderer;
    private int daycost = 2;
    public bool isRepairing = false;
    private int npcCost = 1;
    public int finishDayBuildingTime = 0;
    public NpcManager npcManager;
    public TextMeshProUGUI SattleliteStatusText;
    public TextMeshProUGUI SattleliteWire;
    public TextMeshProUGUI SattleliteCircuit;
    public TextMeshProUGUI SattleliteSteel;
    public Image CircuitIcon;
    public Image WireIcon;
    public Image SteelIcon;
    public Image newworkIcon;
    public Button RepariButton;
    public Button ReconButton;
    public Button SupplyDropButton;

    public DailyGive dailyGive;

    public bool supplyDropActive = false;
    public int supplyDropCountdown = 0;
    private SuuplyDropType currentSupplyDropType;

    public List<ItemData> supplyDropItems = new List<ItemData>();

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (timeManager == null) timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        if (buildManager == null) buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        if (globalstat == null) globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        if (uImanger == null) uImanger = FindFirstObjectByType<UImanger>();
        if (inventoryItemPresent == null) inventoryItemPresent = FindFirstObjectByType<InventoryItemPresent>();
        if (npcManager == null) npcManager = GameManager.Instance != null ? GameManager.Instance.npcManager : FindFirstObjectByType<NpcManager>();
        if (dailyGive == null) dailyGive = FindFirstObjectByType<DailyGive>();
    }

    private void Start()
    {
        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day;
        }
    }

    private void OnMouseDown()
    {
        if (uImanger != null)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.SattleliteUI);
        }

        UpdateSattleliteStatusText();
        UpdateRepairButton();
        UpdateSpecialistIcon(SpecialistRoleNpc.Network);
    }

    private void Update()
    {
        if (dateTime == null && timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        WaitRepair();
        CheckMaintenance();
        Checkrecon();
        DynamicbuttonandIcon();
        CheckSupplyDropCountdown();
    }

    private void DynamicbuttonandIcon()
    {
        if (globalstat == null) return;

        bool shouldReconBeInteractable = globalstat.SatelliteOnline && Reconduration <= 0;
        if (ReconButton != null && ReconButton.interactable != shouldReconBeInteractable)
        {
            ReconButton.interactable = shouldReconBeInteractable;
        }

        if (Reconduration > 0)
        {
            globalstat.ReconActive = true;
        }

        bool shouldSupplyDropBeInteractable = globalstat.SatelliteOnline && supplyDropCountdown <= 0;
        if (SupplyDropButton != null && SupplyDropButton.interactable != shouldSupplyDropBeInteractable)
        {
            SupplyDropButton.interactable = shouldSupplyDropBeInteractable;
        }

        if (SatelliteOnline)
        {
            if (WireIcon != null && WireIcon.gameObject.activeSelf) WireIcon.gameObject.SetActive(false);
            if (CircuitIcon != null && CircuitIcon.gameObject.activeSelf) CircuitIcon.gameObject.SetActive(false);
            if (SteelIcon != null && SteelIcon.gameObject.activeSelf) SteelIcon.gameObject.SetActive(false);
            if (newworkIcon != null && newworkIcon.gameObject.activeSelf) newworkIcon.gameObject.SetActive(false);
        }
    }

    private void Checkrecon()
    {
        if (dateTime != null && dateTime.day != currentDay && Reconduration > 0)
        {
            Reconduration--;
            currentDay = dateTime.day;
        }
    }

    public void ActiveRecon()
    {
        Reconduration = 5;
    }

    private void UpdateSpecialistIcon(SpecialistRoleNpc requiredSpecialist)
    {
        if (newworkIcon != null)
        {
            bool hasSpecialist = HasRequiredSpecialist(requiredSpecialist);

            if (hasSpecialist)
            {
                if (ColorUtility.TryParseHtmlString("#FFFFFF", out Color whiteColor))
                {
                    newworkIcon.color = whiteColor;
                }
            }
            else
            {
                if (ColorUtility.TryParseHtmlString("#8C8C8C", out Color greyColor))
                {
                    newworkIcon.color = greyColor;
                }
            }
        }
    }

    public void WaitRepair()
    {
        if (dateTime != null && dateTime.day >= finishDayBuildingTime && isRepairing)
        {
            isRepairing = false;
            SatelliteOnline = true;
            if (globalstat != null) globalstat.SatelliteOnline = SatelliteOnline;
            if (buildManager != null) buildManager.npc += npcCost;

            if (spriteRenderer != null && repairedSpriteRenderer != null)
            {
                spriteRenderer.sprite = repairedSpriteRenderer;
            }

            if (npcManager != null)
            {
                NpcClass npc = npcManager.GetNpcByClass(SpecialistRoleNpc.Network);
                if (npc != null)
                {
                    npc.isWorking = false;
                }
            }
        }
    }

    private void CheckMaintenance()
    {
        if (buildManager == null || globalstat == null) return;

        if (SatelliteOnline && !buildManager.iselecticitiesactive)
        {
            SatelliteOnline = false;
            globalstat.SatelliteOnline = false;
        }
        else if (SatelliteOnline)
        {
            SatelliteOnline = true;
            globalstat.SatelliteOnline = true;
        }
    }

    public void InitializeRepair()
    {
        isRepairing = true;

        if (inventoryItemPresent != null)
        {
            inventoryItemPresent.RemoveItem(new ItemData { idItem = 1020102, count = 20 });
            inventoryItemPresent.RemoveItem(new ItemData { idItem = 1020103, count = 30 });
        }

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        int currentDayVal = dateTime != null ? dateTime.day : 0;
        finishDayBuildingTime = currentDayVal + daycost;

        if (buildManager != null)
        {
            buildManager.steel -= 3;
            buildManager.npc -= npcCost;
        }

        AssignSpecialistToUpgrade(SpecialistRoleNpc.Network);

        if (uImanger != null)
        {
            uImanger.DisableUIPanel(UImanger.UIPanel.SattleliteUpgradeButton);
            uImanger.DisableUIPanel(UImanger.UIPanel.SattleliteUI);
        }
    }

    private void UpdateSattleliteStatusText()
    {
        int currentCircuit = inventoryItemPresent != null ? inventoryItemPresent.GetItemCountByID(1020102) : 0;
        int requiredCircuit = 20;
        int currentWire = inventoryItemPresent != null ? inventoryItemPresent.GetItemCountByID(1020103) : 0;
        int requiredWire = 30;
        int currentSteel = buildManager != null ? buildManager.steel : 0;
        int requiredSteel = 3;

        string circuitColor = currentCircuit >= requiredCircuit ? "green" : "yellow";
        string wireColor = currentWire >= requiredWire ? "green" : "yellow";
        string steelColor = currentSteel >= requiredSteel ? "green" : "yellow";

        if (SattleliteCircuit != null) SattleliteCircuit.text = $"<color={circuitColor}>Circuits: {currentCircuit}/{requiredCircuit}</color>";
        if (SattleliteWire != null) SattleliteWire.text = $"<color={wireColor}>Wires: {currentWire}/{requiredWire}</color>";
        if (SattleliteSteel != null) SattleliteSteel.text = $"<color={steelColor}>Steel: {currentSteel}/{requiredSteel}</color>";

        if (SattleliteStatusText != null)
        {
            bool hasPower = buildManager != null && buildManager.iselecticitiesactive;

            if (!SatelliteOnline && !isRepairing)
            {
                SattleliteStatusText.text = "The satellite is damaged. Once repaired, it can be used to call an airstrike during the night.";
            }
            else if (isRepairing && !hasPower)
            {
                SattleliteStatusText.text = "The satellite is currently being repaired, but there’s no power supply. Even if repaired, it cannot be used without electricity.";
            }
            else if (isRepairing && hasPower)
            {
                SattleliteStatusText.text = "The satellite is being repaired and we have power. It should be operational soon.";
            }
            else if (SatelliteOnline && !hasPower)
            {
                SattleliteStatusText.text = "The satellite is online, but there’s no electricity to power it. Restore power to use its functionality.";
            }
            else if (SatelliteOnline && hasPower)
            {
                SattleliteStatusText.text = "The satellite is fully operational and ready for use.";
            }
            else
            {
                SattleliteStatusText.text = string.Empty;
            }
        }
    }

    private void UpdateRepairButton()
    {
        if (RepariButton == null) return;

        int currentCircuit = inventoryItemPresent != null ? inventoryItemPresent.GetItemCountByID(1020102) : 0;
        int requiredCircuit = 20;
        int currentWire = inventoryItemPresent != null ? inventoryItemPresent.GetItemCountByID(1020103) : 0;
        int requiredWire = 30;
        int currentSteel = buildManager != null ? buildManager.steel : 0;
        int requiredSteel = 3;

        bool hasSufficientMaterials = currentCircuit >= requiredCircuit &&
                                      currentWire >= requiredWire &&
                                      currentSteel >= requiredSteel;

        bool hasRequiredSpecialist = HasRequiredSpecialist(SpecialistRoleNpc.Network);

        RepariButton.interactable = hasSufficientMaterials && hasRequiredSpecialist;
    }

    private void AssignSpecialistToUpgrade(SpecialistRoleNpc requiredSpecialist)
    {
        if (npcManager != null && npcManager.listNpc != null)
        {
            NpcClass specialistNpc = npcManager.listNpc.Find(npc => npc.roleNpc == requiredSpecialist);
            if (specialistNpc != null)
            {
                npcManager.listNpc.Remove(specialistNpc);
            }
        }
    }

    private bool HasRequiredSpecialist(SpecialistRoleNpc requiredSpecialist)
    {
        if (npcManager != null && npcManager.listNpc != null)
        {
            return npcManager.listNpc.Exists(npc => npc.roleNpc == requiredSpecialist);
        }
        return false;
    }

    public void InitiateSupplyDropByIndex(int typeIndex)
    {
        SuuplyDropType supplyType = (SuuplyDropType)typeIndex;
        InitiateSupplyDrop(supplyType);
    }

    public void InitiateSupplyDrop(SuuplyDropType supplyType)
    {
        if (!SatelliteOnline || (buildManager != null && !buildManager.iselecticitiesactive)) 
        {
            Debug.Log("Cannot initiate supply drop without satellite online and electricity.");
            return;
        }

        currentSupplyDropType = supplyType;
        supplyDropActive = true;
        supplyDropItems.Clear();

        switch (supplyType)
        {
            case SuuplyDropType.FirePower:
                supplyDropCountdown = 3; 
                supplyDropItems.Add(new ItemData { idItem = 1020124, count = 200 });
                supplyDropItems.Add(new ItemData { idItem = 1020125, count = 50 });
                supplyDropItems.Add(new ItemData { idItem = 1020126, count = 100 });
                supplyDropItems.Add(new ItemData { idItem = 1020127, count = 180 });
                supplyDropItems.Add(new ItemData { idItem = 1020110, count = 50 });
                break;

            case SuuplyDropType.Chemical:
                supplyDropCountdown = 2;
                supplyDropItems.Add(new ItemData { idItem = 1020108, count = 50 });
                supplyDropItems.Add(new ItemData { idItem = 1020111, count = 10 });
                supplyDropItems.Add(new ItemData { idItem = 1020117, count = 6 });
                supplyDropItems.Add(new ItemData { idItem = 1020118, count = 6 });
                break;

            case SuuplyDropType.Food:
                supplyDropCountdown = 2;
                supplyDropItems.Add(new ItemData { idItem = 1020130, count = 15 });
                supplyDropItems.Add(new ItemData { idItem = 1020107, count = 30 });
                supplyDropItems.Add(new ItemData { idItem = 1020119, count = 2 });
                supplyDropItems.Add(new ItemData { idItem = 1020106, count = 2 });
                break;

            case SuuplyDropType.Building:
                supplyDropCountdown = 2;
                supplyDropItems.Add(new ItemData { idItem = 1020128, count = 10 });
                supplyDropItems.Add(new ItemData { idItem = 1020129, count = 10 });
                supplyDropItems.Add(new ItemData { idItem = 1020101, count = 25 });
                supplyDropItems.Add(new ItemData { idItem = 1020102, count = 10 });
                supplyDropItems.Add(new ItemData { idItem = 1020103, count = 6 });
                break;
        }

        if (dateTime != null)
        {
            currentDay = dateTime.day;
        }

        Debug.Log($"Supply drop initiated: {supplyType}. Countdown: {supplyDropCountdown} days.");
    }

    public void CheckSupplyDropCountdown()
    {
        if (!supplyDropActive) return;

        if (dateTime != null && dateTime.day != currentDay && supplyDropCountdown > 0)
        {
            supplyDropCountdown--;
            currentDay = dateTime.day;

            if (supplyDropCountdown <= 0)
            {
                CompleteSupplyDrop();
            }
        }
    }

    private void CompleteSupplyDrop()
    {
        supplyDropActive = false;
        supplyDropCountdown = 0;

        if (dailyGive != null && supplyDropItems.Count > 0)
        {
            for (int i = 0; i < supplyDropItems.Count; i++)
            {
                ItemData item = supplyDropItems[i];
                dailyGive.AddItemByID(item.idItem, item.count);
            }
            Debug.Log("Supply drop complete! Items added to DailyGive.");
        }
        else
        {
            Debug.LogWarning("No DailyGive reference or no items defined.");
        }

        supplyDropItems.Clear();
    }
}

public enum SuuplyDropType
{
    FirePower,
    Chemical,
    Food,
    Building
}
