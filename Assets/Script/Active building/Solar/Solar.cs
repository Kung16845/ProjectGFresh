using UnityEngine;

public class Solar : BaseBuilding
{
    public override bool HasOwnClickHandler => true;
    private UImanger uImanger;

    private TimeManager timeManager;
    private DateTime dateTime;
    private int currentDay;
    public int steelCost;

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
        timeManager = TimeManager.Instance;
        dateTime = timeManager.dateTime;
        currentDay = dateTime.day;
    }

    // No stat contribution — Solar toggles electricity on BuildManager
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
                ActivateElectricity();
            }
            else if (BuildManager.Instance.steel >= steelCost)
            {
                BuildManager.Instance.steel -= steelCost;
                ActivateElectricity();
            }
            else
            {
                DeactivateElectricity();
            }
            currentDay = dateTime.day;
        }
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.SolarUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                steelCost = 0;
                uImanger.DisableUIPanel(UImanger.UIPanel.SolarUpgradeUI);
            }
        }
    }



    private void ActivateElectricity()
    {
        BuildManager.Instance.iselecticitiesactive = true;
    }

    private void DeactivateElectricity()
    {
        BuildManager.Instance.iselecticitiesactive = false;
    }
}
