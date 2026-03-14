using UnityEngine;

public class Waterpump : BaseBuilding
{
    public override bool HasOwnClickHandler => true;
    private UImanger uImanger;

    private TimeManager timeManager;
    private DateTime dateTime;
    private int currentDay;
    public int FuelCost;

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
        timeManager = TimeManager.Instance;
        dateTime = timeManager.dateTime;
        currentDay = dateTime.day;
    }

    // No stat contribution — Waterpump toggles water on BuildManager
    protected override BuildingContribution GetContributionForLevel(int level)
    {
        return new BuildingContribution();
    }

    protected override void Update()
    {
        base.Update();

        if (!building.isfinsih) return;

        // Daily resource check
        if (currentDay != dateTime.day)
        {
            if (upgradeBuilding != null && upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel)
            {
                ActivateWater();
            }
            else if (BuildManager.Instance.fuel >= FuelCost)
            {
                BuildManager.Instance.fuel -= FuelCost;
                ActivateWater();
            }
            else
            {
                DeactivateWater();
            }
            currentDay = dateTime.day;
        }
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.WaterPumpUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                FuelCost = 0;
                uImanger.DisableUIPanel(UImanger.UIPanel.WaterPumpUpgradeUi);
            }
        }
    }



    private void ActivateWater()
    {
        BuildManager.Instance.iswateractive = true;
    }

    private void DeactivateWater()
    {
        BuildManager.Instance.iswateractive = false;
    }
}
