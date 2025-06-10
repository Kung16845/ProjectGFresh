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
    public TextMeshProUGUI SattleliteSteel; // Combined status and hint
    public Image CircuitIcon;
    public Image WireIcon;
    public Image SteelIcon;
    public Image newworkIcon;
    public Button RepariButton;
    public Button ReconButton;
    public Button SupplyDropButton;

    // Reference to the DailyGive component where we add items when countdown finishes
    public DailyGive dailyGive;

    // Variables for Supply Drop system
    public bool supplyDropActive = false;
    public int supplyDropCountdown = 0;
    private SuuplyDropType currentSupplyDropType;

    // List of items that will be given once the countdown ends
    // Rename this as needed; for example: supplyDropItems
    public List<ItemData> supplyDropItems = new List<ItemData>();

    void Start()
    {
        dateTime = timeManager.dateTime;
    }

    void OnMouseDown()
    {
        uImanger.ToggleUIPanel(UImanger.UIPanel.SattleliteUI);

        UpdateSattleliteStatusText();
        UpdateRepairButton();
        UpdateSpecialistIcon(SpecialistRoleNpc.Network);
    }
    void Update()
    {
        WaitRepair();
        CheckMaintenance();
        Checkrecon();
        DynamicbuttonandIcon();
        CheckSupplyDropCountdown();
        // If needed, you can call CheckSupplyDropCountdown() here each frame,
        // but it's likely better to call it once per in-game day as shown in Checkrecon().
    }
    void DynamicbuttonandIcon()
    {
        ReconButton.interactable = globalstat.SatelliteOnline;
         SupplyDropButton.interactable = globalstat.SatelliteOnline;
        if(Reconduration > 0)
        {
            ReconButton.interactable = false;
            globalstat.ReconActive = true;
        }
        if(supplyDropCountdown > 0)
        {
             SupplyDropButton.interactable = false;
        }
        if(SatelliteOnline)
        {
            WireIcon.gameObject.SetActive(false);
            CircuitIcon.gameObject.SetActive(false);
            SteelIcon.gameObject.SetActive(false);
            newworkIcon.gameObject.SetActive(false);
        }
    }
    void Checkrecon()
    {
        if (dateTime.day != currentDay && Reconduration > 0)
        {
            Reconduration--;
            currentDay = dateTime.day;
            return;
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
                Color whiteColor;
                if (ColorUtility.TryParseHtmlString("#FFFFFF", out whiteColor))
                {
                    newworkIcon.color = whiteColor; // Set to white
                }
            }
            else
            {
                Color greyColor;
                if (ColorUtility.TryParseHtmlString("#8C8C8C", out greyColor))
                {
                    newworkIcon.color = greyColor; // Set to grey
                }
            }
        }
    }

    public void WaitRepair()
    {
        if (dateTime.day >= finishDayBuildingTime && isRepairing)
        {
            isRepairing = false;
            SatelliteOnline = true;
            globalstat.SatelliteOnline = SatelliteOnline;
            buildManager.npc += npcCost;

            // Change the sprite to the repaired version
            if (spriteRenderer != null && repairedSpriteRenderer != null)
            {
                spriteRenderer.sprite = repairedSpriteRenderer;
            }
            NpcClass npc = npcManager.GetNpcByClass(SpecialistRoleNpc.Network);
            npc.isWorking = false; // Mark the specialist as not working
            return;
        }
    }

    void CheckMaintenance()
    {
        if (SatelliteOnline && !buildManager.iselecticitiesactive)
        {
            SatelliteOnline= false;
            globalstat.SatelliteOnline = false;
        }
        else if (SatelliteOnline)
        {
            SatelliteOnline= true;
            globalstat.SatelliteOnline = true;
        }
    }

    public void InitializeRepair()
    {
        isRepairing = true;
        ItemData itemDataToRemove = new ItemData
        {
            idItem = 1020102,
            count = 20
        };
        inventoryItemPresent.RemoveItem(itemDataToRemove);
        ItemData itemDataToRemove2 = new ItemData
        {
            idItem = 1020103,
            count = 30
        };
        inventoryItemPresent.RemoveItem(itemDataToRemove2);
        dateTime = timeManager.dateTime;
        buildManager.steel -= 3;
        finishDayBuildingTime += dateTime.day + daycost;
        buildManager.npc -= npcCost;
        AssignSpecialistToUpgrade(SpecialistRoleNpc.Network);
        uImanger.DisableUIPanel(UImanger.UIPanel.SattleliteUpgradeButton);
        uImanger.DisableUIPanel(UImanger.UIPanel.SattleliteUI);
    }

    private void UpdateSattleliteStatusText()
    {
        int currentCircuit = inventoryItemPresent.GetItemCountByID(1020102);
        int requiredCircuit = 20;
        int currentWire = inventoryItemPresent.GetItemCountByID(1020103);
        int requiredWire = 30;
        int currentSteel = buildManager.steel;
        int requiredSteel = 3;

        string circuitColor = currentCircuit >= requiredCircuit ? "green" : "yellow";
        string wireColor = currentWire >= requiredWire ? "green" : "yellow";
        string steelColor = currentSteel >= requiredSteel ? "green" : "yellow";

        SattleliteCircuit.text = $"<color={circuitColor}>Circuits: {currentCircuit}/{requiredCircuit}</color>";
        SattleliteWire.text = $"<color={wireColor}>Wires: {currentWire}/{requiredWire}</color>";
        SattleliteSteel.text = $"<color={steelColor}>Steel: {currentSteel}/{requiredSteel}</color>";

        if (!SatelliteOnline && !isRepairing)
        {
            SattleliteStatusText.text =
                "The satellite is damaged. Once repaired, it can be used to call an airstrike during the night.";
        }
        else if (isRepairing && !buildManager.iselecticitiesactive)
        {
            SattleliteStatusText.text =
                "The satellite is currently being repaired, but there’s no power supply. Even if repaired, it cannot be used without electricity.";
        }
        else if (isRepairing && buildManager.iselecticitiesactive)
        {
            SattleliteStatusText.text =
                "The satellite is being repaired and we have power. It should be operational soon.";
        }
        else if (SatelliteOnline && !buildManager.iselecticitiesactive)
        {
            SattleliteStatusText.text =
                "The satellite is online, but there’s no electricity to power it. Restore power to use its functionality.";
        }
        else if (SatelliteOnline && buildManager.iselecticitiesactive)
        {
            SattleliteStatusText.text =
                "The satellite is fully operational and ready for use.";
        }
        else
        {
            SattleliteStatusText.text = string.Empty;
        }
    }

    private void UpdateRepairButton()
    {
        int currentCircuit = inventoryItemPresent.GetItemCountByID(1020102);
        int requiredCircuit = 20;
        int currentWire = inventoryItemPresent.GetItemCountByID(1020103);
        int requiredWire = 30;
        int currentSteel = buildManager.steel;
        int requiredSteel = 3;

        bool hasSufficientMaterials = currentCircuit >= requiredCircuit &&
                                      currentWire >= requiredWire &&
                                      currentSteel >= requiredSteel;

        bool hasRequiredSpecialist = HasRequiredSpecialist(SpecialistRoleNpc.Network);

        RepariButton.interactable = hasSufficientMaterials && hasRequiredSpecialist;
    }

    private void AssignSpecialistToUpgrade(SpecialistRoleNpc requiredSpecialist)
    {
        NpcClass specialistNpc = npcManager.listNpc.Find(npc => npc.roleNpc == requiredSpecialist);

        if (specialistNpc != null)
        {
            npcManager.listNpc.Remove(specialistNpc);
        }
    }

    private bool HasRequiredSpecialist(SpecialistRoleNpc requiredSpecialist)
    {
        return npcManager.listNpc.Exists(npc => npc.roleNpc == requiredSpecialist);
    }

    public void InitiateSupplyDropByIndex(int typeIndex)
    {
        SuuplyDropType supplyType = (SuuplyDropType)typeIndex;
        InitiateSupplyDrop(supplyType);
    }
    public void InitiateSupplyDrop(SuuplyDropType supplyType)
    {
        if (!SatelliteOnline || !buildManager.iselecticitiesactive) 
        {
            Debug.Log("Cannot initiate supply drop without satellite online and electricity.");
            return;
        }

        currentSupplyDropType = supplyType;
        supplyDropActive = true;

        // Clear any previous items
        supplyDropItems.Clear();

        // Set fixed countdown and items based on the supply type
        switch (supplyType)
        {
            case SuuplyDropType.FirePower:
                supplyDropCountdown = 3; 
                // Add the items that will be granted after countdown
                supplyDropItems.Add(new ItemData { idItem = 1020124, count = 200 }); // Example items
                supplyDropItems.Add(new ItemData { idItem = 1020125, count = 50 });
                supplyDropItems.Add(new ItemData { idItem = 1020126, count = 100 }); // Example items
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
        currentDay = dateTime.day;
        Debug.Log($"Supply drop initiated: {supplyType}. Countdown: {supplyDropCountdown} days.");
    }

    // This function should be called once per new day, similar to Checkrecon().
    // For example, you can call it in Update() if date has changed or in any daily tick method.
    public void CheckSupplyDropCountdown()
    {
        if (!supplyDropActive) return;

        // If a new day has started
        if (dateTime.day != currentDay && supplyDropCountdown > 0)
        {
            supplyDropCountdown--;
            currentDay = dateTime.day;

            if (supplyDropCountdown <= 0)
            {
                CompleteSupplyDrop();
            }
        }
    }

    // Called when the countdown finishes
    private void CompleteSupplyDrop()
    {
        supplyDropActive = false;
        supplyDropCountdown = 0;

        // Add the supplyDropItems to the DailyGive list
        if (dailyGive != null && supplyDropItems.Count > 0)
        {
            foreach (var item in supplyDropItems)
            {
                dailyGive.AddItemByID(item.idItem, item.count);
            }
            Debug.Log("Supply drop complete! Items added to DailyGive.");
        }
        else
        {
            Debug.LogWarning("No DailyGive reference or no items defined.");
        }

        // Clear the items since they have been given
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

