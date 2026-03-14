using UnityEngine;

public class GardenBuilding : BaseBuilding
{
    public override bool HasOwnClickHandler => true;
    private UImanger uImanger;

    private TimeManager timeManager;
    private DateTime dateTime;
    private int currentDay;

    public int foodGainPerDay;
    public int currentYield;
    public int yieldduration;

    // No stat contribution — Garden produces food directly
    protected override BuildingContribution GetContributionForLevel(int level)
    {
        return new BuildingContribution();
    }

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
        timeManager = TimeManager.Instance;
        dateTime = timeManager.dateTime;
        currentDay = dateTime.day - 1;
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
            uImanger.ToggleUIPanel(UImanger.UIPanel.SmallGardenUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.SmallGardenUpgradeButton);
            }
        }
    }



    private void FoodGain()
    {
        if (dateTime.day != currentDay)
        {
            if (yieldduration > 0)
            {
                foodGainPerDay += 3;
                yieldduration--;
            }
            BuildManager.Instance.food += foodGainPerDay;
            currentDay = dateTime.day;
        }
    }

    private void UpdateFoodGainPerDay()
    {
        switch (upgradeBuilding.currentLevel)
        {
            case 2:
                foodGainPerDay = 4;
                break;
            default:
                foodGainPerDay = 2;
                break;
        }
    }

    private void UpdateCurrentYield()
    {
        currentYield = yieldduration > 0 ? foodGainPerDay + 3 : foodGainPerDay;
    }
}
