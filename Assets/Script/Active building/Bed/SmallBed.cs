using UnityEngine;

public class SmallBed : BaseBuilding
{
    // Public for SmallBedUI to read
    public int currentBedContribution => GetContributionForLevel(upgradeBuilding.currentLevel).beds;

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
}
