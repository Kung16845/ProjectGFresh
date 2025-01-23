using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moonshine : MonoBehaviour
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
    public int maxCraftingSlots;
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
        maxCraftingSlots = 2;
        globalstat.UpdateMoonshineCraftingSlot(maxCraftingSlots);
    }

    void Update()
    {
        IsElectricActive();
        IsElectricInactive();
        craftManager.UpdateCraftingJobs();
    }
    void CheckUpgrade()
    {
        if(upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
        {
            maxCraftingSlots = 5;
            globalstat.UpdateMoonshineCraftingSlot(maxCraftingSlots - globalstat.usedMoonshineCraftingSlot);
            uImanger.DisableUIPanel(UImanger.UIPanel.MoonshineUpgradeButton);
        }
    }
    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.MoonshineUI);
            CheckUpgrade();
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
        return craftManager.AddCraftingJob(craftingItem, CraftingSource.Moonshine);
    }
}
