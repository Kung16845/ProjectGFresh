using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ChemcicalLab : MonoBehaviour
{
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public int currentDay;
    public Building building;
    public UpgradeUi upgradeUi;
    public UpgradeBuilding upgradeBuilding;
    public UImanger uImanger;
    public Globalstat globalstat;
    public bool Isapplyspeed;
    public List<CraftingItem> craftingItemsLevel1;
    public List<CraftingItem> craftingItemsLevel2;
    public int maxCraftingSlots = 3;
    public CraftManager craftManager;
    public InventoryItemPresent inventoryItemPresent;

    void Start()
    {
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        uImanger = FindObjectOfType<UImanger>();
        timeManager = FindObjectOfType<TimeManager>();
        globalstat = FindObjectOfType<Globalstat>();
        buildManager = FindObjectOfType<BuildManager>();
        building = FindObjectOfType<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();
        craftManager = FindObjectOfType<CraftManager>();
        dateTime = timeManager.dateTime;
        currentDay = dateTime.day;
        globalstat.UpdateChemicalCraftingSlot(maxCraftingSlots);
        maxCraftingSlots = 3;
    }

    void Update()
    {
        IsElectricActive();
        IsElectricInactive();
        craftManager.UpdateCraftingJobs();
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.ChemcicalLabWorkshopUI);
            CheckUpgrade();
        }
    }
    void CheckUpgrade()
    {
        if(upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
        {
            maxCraftingSlots = 5;
            globalstat.UpdateChemicalCraftingSlot(maxCraftingSlots - globalstat.usedChemicalCraftingSlot);
            uImanger.DisableUIPanel(UImanger.UIPanel.ChemcicalLabButtonUpgradeUI);
        }
    }
    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }
    void IsElectricActive()
    {
        if (building.isfinsih && buildManager.iselecticitiesactive)
        {
        }
    }

    void IsElectricInactive()
    {
        if (building.isfinsih && !buildManager.iselecticitiesactive)
        {
        }
    }
    public CraftingResult AddCraftingJob(CraftingItem craftingItem)
    {
        return craftManager.AddCraftingJob(craftingItem, CraftingSource.ChemicalLab);
    }
}
