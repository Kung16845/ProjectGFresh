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

    private void Start()
    {
        inventoryItemPresent = FindFirstObjectByType<InventoryItemPresent>();
        uImanger = FindFirstObjectByType<UImanger>();
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        craftManager = FindFirstObjectByType<CraftManager>();

        // Always get components directly on this GameObject
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day;
        }

        maxCraftingSlots = 2;
        if (globalstat != null)
        {
            globalstat.UpdateMoonshineCraftingSlot(maxCraftingSlots);
        }
    }

    private void Update()
    {
        if (craftManager != null)
        {
            craftManager.UpdateCraftingJobs();
        }
    }

    private void CheckUpgrade()
    {
        if (upgradeBuilding != null && upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
        {
            maxCraftingSlots = 5;
            if (globalstat != null)
            {
                globalstat.UpdateMoonshineCraftingSlot(maxCraftingSlots - globalstat.usedMoonshineCraftingSlot);
            }
            if (uImanger != null)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.MoonshineUpgradeButton);
            }
        }
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.MoonshineUI);
            }
            CheckUpgrade();
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

    public CraftingResult AddCraftingJob(CraftingItem craftingItem)
    {
        if (craftManager != null)
        {
            return craftManager.AddCraftingJob(craftingItem, CraftingSource.Moonshine);
        }
        return CraftingResult.NoAvailableSlots;
    }
}
