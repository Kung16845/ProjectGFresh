using UnityEngine;

public class MediumBed : BaseBuilding
{
    // Public for MediumBedUI to read
    public int currentBedContribution => GetContributionForLevel(upgradeBuilding.currentLevel).beds;

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
}
