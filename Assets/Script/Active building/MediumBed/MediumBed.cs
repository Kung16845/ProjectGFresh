using UnityEngine;

public class MediumBed : BaseBuilding
{
    private UImanger uImanger;
    private UpgradeUi upgradeUi;

    // Public for MediumBedUI to read
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
                return new BuildingContribution { beds = 7 };
            default:
                return new BuildingContribution { beds = 4 };
        }
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.MediumBedUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.MediumBedUpgradeUI);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }
}
