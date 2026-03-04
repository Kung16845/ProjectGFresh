using UnityEngine;

public class Lounge : BaseBuilding
{
    private UImanger uImanger;
    private UpgradeUi upgradeUi;

    // Public for LoungeUI to read
    public int currentBedContribution => GetContributionForLevel(upgradeBuilding.currentLevel).beds;
    public float currentDiscontentContribution => GetContributionForLevel(upgradeBuilding.currentLevel).discontent;

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
                return new BuildingContribution { beds = 3, discontent = 25f };
            case 3:
                return new BuildingContribution { beds = 5, discontent = 35f };
            default:
                return new BuildingContribution { beds = 1, discontent = 15f };
        }
    }

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.LoungeUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.LoungeUpgradeButton);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
    }

    // For LoungeUI to preview next level values
    public int GetBedValueBasedOnLevel(int level = -1)
    {
        int targetLevel = (level == -1) ? upgradeBuilding.currentLevel : level;
        return GetContributionForLevel(targetLevel).beds;
    }

    public float GetDiscontentValueBasedOnLevel(int level = -1)
    {
        int targetLevel = (level == -1) ? upgradeBuilding.currentLevel : level;
        return GetContributionForLevel(targetLevel).discontent;
    }
}
