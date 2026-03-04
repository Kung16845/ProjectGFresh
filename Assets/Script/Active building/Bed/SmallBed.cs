using UnityEngine;

public class SmallBed : BaseBuilding
{
    private UImanger uImanger;
    private UpgradeUi upgradeUi;

    // Public for SmallBedUI to read
    public int currentBedContribution => GetContributionForLevel(upgradeBuilding.currentLevel).beds;

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
    }

    protected override BuildingContribution GetContributionForLevel(int level)
    {
        switch (level)
        {
            case 2:
                return new BuildingContribution { beds = 4 };
            default:
                return new BuildingContribution { beds = 2 };
        }
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.SmallBedUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.SmallBedUpgradeButton);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }
}
