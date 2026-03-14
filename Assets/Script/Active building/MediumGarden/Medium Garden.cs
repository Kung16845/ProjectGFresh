using UnityEngine;

public class MediumGarden : BaseBuilding
{
    public override bool HasOwnClickHandler => true;
    private UImanger uImanger;

    private TimeManager timeManager;
    private DateTime dateTime;
    private int currentDay;

    public InventoryItemPresent inventoryItemPresent;
    public int foodGainPerDay;
    public int currentYield;
    public int yieldduration;

    public bool isHerbalPlanted = false;
    public int herbalItemID = 1;
    public int herbalDailyAmount = 5;

    // No stat contribution — MediumGarden produces food/herbal directly
    protected override BuildingContribution GetContributionForLevel(int level)
    {
        return new BuildingContribution();
    }

    protected override void Start()
    {
        base.Start();
        inventoryItemPresent = FindObjectOfType<InventoryItemPresent>();
        uImanger = FindObjectOfType<UImanger>();
        timeManager = TimeManager.Instance;
        dateTime = timeManager.dateTime;
        currentDay = dateTime.day;
    }

    protected override void Update()
    {
        base.Update();
        if (!building.isfinsih) return;

        UpdateFoodGainPerDay();
        FoodGain();
        UpdateCurrentYield();
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.MediumGardenUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.MediumGardenUpgradeButton);
            }
        }
    }



    private void FoodGain()
    {
        if (dateTime.day != currentDay)
        {
            int finalDailyYield = foodGainPerDay;

            if (yieldduration > 0)
            {
                finalDailyYield += 3;
                yieldduration--;
            }

            if (isHerbalPlanted && upgradeBuilding.currentLevel >= 2)
            {
                int herbalAmount = yieldduration > 0 ? 9 : 5;
                inventoryItemPresent.AddItemByID("1020105", herbalAmount);
            }
            else
            {
                BuildManager.Instance.food += finalDailyYield;
            }

            currentDay = dateTime.day;
        }
    }

    private void UpdateFoodGainPerDay()
    {
        switch (upgradeBuilding.currentLevel)
        {
            case 2:
                foodGainPerDay = 5;
                break;
            default:
                foodGainPerDay = 3;
                break;
        }
    }

    private void UpdateCurrentYield()
    {
        int baseYield = foodGainPerDay;
        if (yieldduration > 0)
            baseYield += 3;

        if (isHerbalPlanted && upgradeBuilding.currentLevel >= 2)
            currentYield = herbalDailyAmount;
        else
            currentYield = baseYield;
    }
}
