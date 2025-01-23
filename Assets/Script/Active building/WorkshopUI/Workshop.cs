using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum CraftingResult
{
    Success,
    NotEnoughItems,
    NoAvailableSlots
}
public class Workshop : MonoBehaviour
{
    public float Actionspeedincrease;
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public int currentDay;
    public Building building;
    public UpgradeUi upgradeUi;
    public UpgradeBuilding upgradeBuilding;
    public UImanger uImanger;
    public Globalstat globalstat;
    public int Craftingslot;
    public bool Isapplyspeed;
    public List<CraftingItem> craftingItemsLevel1;
    public List<CraftingItem> craftingItemsLevel2;
    public int maxCraftingSlots = 3;
    public InventoryItemPresent inventoryItemPresent;
    public CraftManager craftManager;

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
        Actionspeedincrease = 0.25f;
        globalstat.UpdateCraftingSlots(maxCraftingSlots);

        
        Isapplyspeed = false;
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
            uImanger.ToggleUIPanel(UImanger.UIPanel.WorkshopUI);
            CheckUpgrade();
        }
    }
    void CheckUpgrade()
    {
        if(upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
        {
            maxCraftingSlots = 5;
            globalstat.UpdateCraftingSlots(maxCraftingSlots - globalstat.usedCraftingSlot);
            uImanger.DisableUIPanel(UImanger.UIPanel.WorkshopUpgradeUI);
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
            float IncreaseActionSpeed = 0.25f;
            if (!Isapplyspeed)
            {
                globalstat.CalculateActionSpeed(IncreaseActionSpeed);
                Isapplyspeed = true;
            }
        }
    }

    void IsElectricInactive()
    {
        if (building.isfinsih && !buildManager.iselecticitiesactive)
        {
            float DecreaseActionSpeed = 0.25f;
            if (Isapplyspeed)
            {
                globalstat.CalculateActionSpeed(-DecreaseActionSpeed);
                Isapplyspeed = false;
            }
        }
    }
    public CraftingResult AddCraftingJob(CraftingItem craftingItem)
    {
        return craftManager.AddCraftingJob(craftingItem, CraftingSource.Workshop);
    }

}
