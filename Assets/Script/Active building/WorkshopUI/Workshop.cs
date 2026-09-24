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

    private void Start()
    {
        inventoryItemPresent = FindFirstObjectByType<InventoryItemPresent>();
        uImanger = FindFirstObjectByType<UImanger>();
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        craftManager = FindFirstObjectByType<CraftManager>();

        // Always get components directly on this GameObject to prevent cross-talk
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day;
        }

        Actionspeedincrease = 0.25f;
        if (globalstat != null)
        {
            globalstat.UpdateCraftingSlots(maxCraftingSlots);
        }

        Isapplyspeed = false;
    }

    private void Update()
    {
        IsElectricActive();
        IsElectricInactive();

        if (craftManager != null)
        {
            craftManager.UpdateCraftingJobs();
        }
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.WorkshopUI);
            }
            CheckUpgrade();
        }
    }

    private void CheckUpgrade()
    {
        if (upgradeBuilding != null && upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
        {
            maxCraftingSlots = 5;
            if (globalstat != null)
            {
                globalstat.UpdateCraftingSlots(maxCraftingSlots - globalstat.usedCraftingSlot);
            }
            if (uImanger != null)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.WorkshopUpgradeUI);
            }
        }
    }

    public void AssignUpgradeData()
    {
        if (upgradeUi == null)
        {
            upgradeUi = FindFirstObjectByType<UpgradeUi>();
        }
        if (upgradeUi != null && upgradeBuilding != null)
        {
            upgradeUi.Initialize(upgradeBuilding);
        }
    }

    private void IsElectricActive()
    {
        if (building != null && building.isfinsih && buildManager != null && buildManager.iselecticitiesactive)
        {
            float increaseActionSpeed = 0.25f;
            if (!Isapplyspeed)
            {
                if (globalstat != null)
                {
                    globalstat.CalculateActionSpeed(increaseActionSpeed);
                }
                Isapplyspeed = true;
            }
        }
    }

    private void IsElectricInactive()
    {
        if (building != null && building.isfinsih && buildManager != null && !buildManager.iselecticitiesactive)
        {
            float decreaseActionSpeed = 0.25f;
            if (Isapplyspeed)
            {
                if (globalstat != null)
                {
                    globalstat.CalculateActionSpeed(-decreaseActionSpeed);
                }
                Isapplyspeed = false;
            }
        }
    }

    public CraftingResult AddCraftingJob(CraftingItem craftingItem)
    {
        if (craftManager != null)
        {
            return craftManager.AddCraftingJob(craftingItem, CraftingSource.Workshop);
        }
        return CraftingResult.NoAvailableSlots;
    }
}
